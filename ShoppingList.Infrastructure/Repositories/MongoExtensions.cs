using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using ShoppingList.Core.Domain.Entities;
using ShoppingList.Core.Domain.RepositoryContracts;
using System.Text.Json;

namespace ShoppingList.Infrastructure.Repositories;

public static class MongoExtensions
{
    public static IServiceCollection AddItemRepository(this IServiceCollection services, string collectionName)
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
        BsonSerializer.RegisterSerializer(new EnumSerializer<ItemStatus>(BsonType.String));
        BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

        services.AddSingleton<IRepository<Item>>(serviceProvider =>
        {
            var mongoClient = serviceProvider.GetRequiredService<IMongoClient>();
            return new MongoRepository<Item>(mongoClient.GetDatabase("ShoppingListDb"), collectionName);
        });

        return services;
    }
}
