namespace ShoppingList.Core.Domain.Entities;

public interface IEntity
{
    Guid Id { get; set; }
    string Name { get; set; }
}
