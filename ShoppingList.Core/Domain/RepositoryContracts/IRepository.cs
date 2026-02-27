using ShoppingList.Core.Domain.Entities;

namespace ShoppingList.Core.Domain.RepositoryContracts;

public interface IRepository<T> where T : IEntity
{
    Task<IReadOnlyCollection<T>> GetAllAsync();
    Task<IReadOnlyCollection<T>> GetByParametersAsync(string? name, int pageNumber, int pageSize);
    Task<T?> GetByIdAsync(Guid id);
    Task<bool> ExistsAsync(string Name);
    Task<long> CountAsync(string? name);
    Task InsertAsync(T entity);
    Task UpdateAsync(T entity);
    Task RemoveAsync(Guid id);
    Task InitializeAsync(IReadOnlyCollection<T> entities);
    Task ClearAsync();
}
