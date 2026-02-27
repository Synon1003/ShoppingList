using ShoppingList.Core.Domain.Entities;
using ShoppingList.Core.Dtos;

namespace ShoppingList.Core.Mappings;

public static class ItemExtensions
{
    public static ItemDto ToDto(this Item item)
    {

        return new ItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            Price = item.Price,
            Status = item.Status.ToString(),
            UpdatedAt = item.UpdatedAt
        };
    }
}
