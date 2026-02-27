using ShoppingList.ServiceProxy.Features.Items.GetItem;
using ShoppingList.ServiceProxy.Features.Items.GetItems;

namespace ShoppingList.ServiceProxy.Features.Items;

public static class ItemsEndpoints
{
    public static void MapItems(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/itemsproxy");

        group.MapGetItems();
        group.MapGetAllItems();
        group.MapGetItem();
    }
}
