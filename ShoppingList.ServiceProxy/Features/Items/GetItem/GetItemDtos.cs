using System.Text.Json;
using Newtonsoft.Json;

namespace ShoppingList.ServiceProxy.Features.Items.GetItem;

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
