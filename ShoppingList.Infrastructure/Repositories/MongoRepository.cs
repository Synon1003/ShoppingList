using MongoDB.Driver;
using ShoppingList.Core.Domain.Entities;
using ShoppingList.Core.Domain.RepositoryContracts;

namespace ShoppingList.Infrastructure.Repositories;

public class MongoRepository<T> : IRepository<T> where T : IEntity
{
    private readonly IMongoCollection<T> _dbCollection;
    private readonly FilterDefinitionBuilder<T> _filterBuilder = Builders<T>.Filter;

    public MongoRepository(IMongoDatabase database, string collectionName)
    {
        _dbCollection = database.GetCollection<T>(collectionName);
    }

    public async Task<IReadOnlyCollection<T>> GetAllAsync()
    {
        return await _dbCollection.Find(_filterBuilder.Empty).ToListAsync();
    }

    public async Task<IReadOnlyCollection<T>> GetByParametersAsync(string? name, int pageNumber, int pageSize)
    {
        FilterDefinition<T> filter = !string.IsNullOrWhiteSpace(name) 
        ? _filterBuilder.Eq("Name", name)
        : _filterBuilder.Empty;

        return await _dbCollection.Find(filter)
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();
    }
    

    public async Task<T?> GetByIdAsync(Guid id)
    {
        FilterDefinition<T> filter = _filterBuilder.Eq(entity => entity.Id, id);
        return await _dbCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<bool> ExistsAsync(string name)
    {
        FilterDefinition<T> filter = _filterBuilder.Eq(entity => entity.Name, name);
        var count = await _dbCollection.CountDocumentsAsync(filter);
        return count > 0;
    }

    public async Task<long> CountAsync(string? name)
    {
        FilterDefinition<T> filter = string.IsNullOrEmpty(name) ? _filterBuilder.Empty :
             _filterBuilder.Eq(entity => entity.Name, name);
        return await _dbCollection.CountDocumentsAsync(filter);
    }

    public async Task InsertAsync(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        await _dbCollection.InsertOneAsync(entity);
    }

    public async Task UpdateAsync(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        FilterDefinition<T> filter = _filterBuilder.Eq(existingEntity => existingEntity.Id, entity.Id);
        await _dbCollection.ReplaceOneAsync(filter, entity);
    }

    public async Task RemoveAsync(Guid id)
    {
        FilterDefinition<T> filter = _filterBuilder.Eq(entity => entity.Id, id);
        await _dbCollection.DeleteOneAsync(filter);
    }

    public async Task InitializeAsync(IReadOnlyCollection<T> initialData)
    {
        long existingCount = await _dbCollection.CountDocumentsAsync(_filterBuilder.Empty);
        if (existingCount == 0)
            await _dbCollection.InsertManyAsync(initialData);
    }

    public async Task ClearAsync()
    {
        await _dbCollection.DeleteManyAsync(_filterBuilder.Empty);
    }
}
