using ShoppingList.ServiceProxy.Client;

namespace ShoppingList.ServiceProxy.Features.Items.GetItems;

public static class GetItemsEndpoint
{
    public static void MapGetItems(this IEndpointRouteBuilder app)
    {
        // GET /items
        app.MapGet("/", async (
            [AsParameters] GetItemsDto request,
            ShoppingListServiceClient client,
            ILogger<Program> logger) =>
        {
            logger.LogInformation("ServiceProxy GetItems called with name {name}, pageNumber {pageNumber}, pageSize {pageSize}", request.Name, request.PageNumber, request.PageSize);
            return await client.ItemsGETAsync(request.Name, request.PageNumber, request.PageSize);
        })
        .Produces<ItemsPageDto>();
    }

    public static void MapGetAllItems(this IEndpointRouteBuilder app)
    {
        // GET /items
        app.MapGet("/all", async (
            ShoppingListServiceClient client,
            ILogger<Program> logger) =>
        {
            logger.LogInformation("ServiceProxy GetAllItems called");
            return await client.AllAsync();
        })
        .Produces<List<ItemDto>>();
    }
}
