using Microsoft.AspNetCore.HttpLogging;
using Polly;
using ShoppingList.ServiceProxy.ResilienceHandlers;
using ShoppingList.ServiceProxy.Features.Items;
using ShoppingList.ServiceProxy.Client;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddProblemDetails();

builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.RequestMethod |
                            HttpLoggingFields.RequestPath |
                            HttpLoggingFields.ResponseStatusCode |
                            HttpLoggingFields.Duration;
    options.CombineLogs = true;
});

builder.Services.AddOpenApi();
builder.Services.AddHttpClient<ShoppingListServiceClient>(client =>
{
    client.BaseAddress = new Uri("https://apiservice/");
})
.AddResilienceHandler("waiter", config =>
{
    config.AddTimeout(TimeSpan.FromSeconds(45));
    config.AddRetry(RetryStrategyFactory.GetDefaultRetryOptions());
    config.AddCircuitBreaker(CircuitBreakerStrategyFactory.GetDefaultCircuitBreakerOptions());
});

var app = builder.Build();
app.UseHttpLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapDefaultEndpoints();
app.MapItems();
app.UseStatusCodePages();
app.Run();