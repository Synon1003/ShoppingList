using ShoppingList.ServiceProxy.Client;

namespace ShoppingList.ServiceProxy.Features.Items.GetItem;

public static class GetItemEndpoint
{
    public static void MapGetItem(this IEndpointRouteBuilder app)
    {
        // GET /itemsproxy/122233-434d-43434....
        app.MapGet("/{id}", async (
            Guid id,
            ShoppingListServiceClient client,
            ILogger<Program> logger) =>
        {
            logger.LogInformation("ServiceProxy GetItem called with {id}", id);
            return await client.ItemsGET2Async(id);
        })
        .WithName("GetItem")
        .Produces<ItemDto>()
        .Produces(StatusCodes.Status404NotFound);
    }
}
