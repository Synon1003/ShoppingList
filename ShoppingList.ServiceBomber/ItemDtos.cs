using System.ComponentModel.DataAnnotations;

namespace ShoppingList.ServiceBomber;

public record CreateItemDto(
    [Required][StringLength(50)] string Name,
    [Range(1, 100000)] int Price,
    [Required][StringLength(200)] string Description
);

public record ItemDto(
    Guid Id,
    string Name,
    string Description,
    int Price,
    string Status,
    DateTime UpdatedAt);

public record ItemsPageDto(
    int TotalPages, 
    List<ItemDto> Data
);

public record GetItemsDto(
    int PageNumber = 1,
    int PageSize = 5,
    string? Name = null);