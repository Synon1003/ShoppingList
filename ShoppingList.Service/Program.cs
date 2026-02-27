using Microsoft.AspNetCore.HttpLogging;
using MongoDB.Driver;
using ShoppingList.Core.Domain.Entities;
using ShoppingList.Core.Domain.RepositoryContracts;
using ShoppingList.Infrastructure.Repositories;
using Microsoft.Extensions.Caching.Hybrid;
using System.Threading.Channels;
using System.Collections.Concurrent;
using ShoppingList.Service.BackgroundServices;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddProblemDetails();
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.RequestProperties
        | HttpLoggingFields.ResponsePropertiesAndHeaders;
});

builder.AddMongoDBClient("mongodb");
builder.Services.AddItemRepository("items");
builder.Services.AddScoped<IRepository<Item>>(
    sp => new MongoRepository<Item>(sp.GetRequiredService<IMongoDatabase>(), "items"));
builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;
    options.RespectBrowserAcceptHeader = true;
    options.SuppressAsyncSuffixInActionNames = false;
}).AddXmlSerializerFormatters();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// builder.Services.AddOpenApi();
builder.Services.AddHybridCache(options =>
{
    options.DefaultEntryOptions = new HybridCacheEntryOptions
    {
        LocalCacheExpiration = TimeSpan.FromMinutes(5),
        Expiration = TimeSpan.FromMinutes(5)
    };
});

builder.AddRedisDistributedCache("redis");

builder.Services.AddSingleton(_ =>
{
    var channel = Channel.CreateBounded<GenerationJob>(
        new BoundedChannelOptions(10){ FullMode = BoundedChannelFullMode.Wait
    });

    return channel;
});
builder.Services.AddSingleton<ConcurrentDictionary<string, GenerationStatus>>();
builder.Services.AddHostedService<GenerationService>();

var app = builder.Build();

app.UseHttpLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
        c.RoutePrefix = "swagger";
    });

}
// app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(b => b.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());


app.MapControllers();
app.MapDefaultEndpoints();

app.Run();