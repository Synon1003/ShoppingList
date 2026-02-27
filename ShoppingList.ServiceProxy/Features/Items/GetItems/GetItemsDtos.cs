using Newtonsoft.Json;

namespace ShoppingList.ServiceProxy.Features.Items.GetItems;

public record ItemDto
{
    [JsonProperty("id")]
    public Guid Id {get; set;}

    [JsonProperty("name")]
    public string Name {get; set;} = String.Empty;

    [JsonProperty("description")]
    public string Description {get; set;} = String.Empty;

    [JsonProperty("price")]
    public int Price {get; set;}

    [JsonProperty("status")]
    public string Status {get; set;} = String.Empty;

    [JsonProperty("updatedAt")]
    public DateTimeOffset UpdatedAt {get; set;}
}

public record GetItemsDto
{
    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("pageNumber")]
    public int PageNumber { get; set; } = 1;

    [JsonProperty("pageSize")]
    public int PageSize { get; set; } = 5;
}

public record ItemsPageDto
{
    [JsonProperty("totalPages")]
    public int TotalPages { get; set; }

    [JsonProperty("data")]
    public List<ItemDto> Data { get; set; } = [];
}