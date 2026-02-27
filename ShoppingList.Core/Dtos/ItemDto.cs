using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using System.Text.Json.Serialization;

namespace ShoppingList.Core.Dtos;

public record ItemDto
{
    [XmlElement("id")]
    [JsonPropertyName("id")]
    public Guid Id {get; set;}

    [XmlElement("name")]
    [JsonPropertyName("name")]
    public string Name {get; set;} = String.Empty;

    [XmlElement("description")]
    [JsonPropertyName("description")]
    public string Description {get; set;} = String.Empty;

    [XmlElement("price")]
    [JsonPropertyName("price")]
    public int Price {get; set;}

    [XmlElement("status")]
    [JsonPropertyName("status")]
    public string Status {get; set;} = String.Empty;

    [XmlElement("updatedAt")]
    [JsonPropertyName("updatedAt")]
    public DateTimeOffset UpdatedAt {get; set;}
}

public record CreateItemDto
{
    [MaxLength(50)]
    [XmlElement("name")]
    [JsonPropertyName("name")]
    public string Name {get; set;} = String.Empty;

    [MaxLength(200)]
    [XmlElement("description")]
    [JsonPropertyName("description")]
    public string Description {get; set;} = String.Empty;

    [Range(0, 100000)]
    [XmlElement("price")]
    [JsonPropertyName("price")]
    public int Price {get; set;}
}

public record GetItemsDto
{
    [XmlElement("name")]
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [XmlElement("pageNumber")]
    [JsonPropertyName("pageNumber")]
    public int PageNumber { get; set; } = 1;

    [XmlElement("pageSize")]
    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; } = 5;
}

public record ItemsPageDto
{
    [XmlElement("totalPages")]
    [JsonPropertyName("totalPages")]
    public int TotalPages { get; set; }

    [XmlElement("data")]
    [JsonPropertyName("data")]
    public List<ItemDto> Data { get; set; } = [];
}