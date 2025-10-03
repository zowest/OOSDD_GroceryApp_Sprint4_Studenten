using Grocery.Core.Models;
using System;

namespace Grocery.Core.Interfaces.Services
{
    public interface IGroceryListItemsService
    {
        List<GroceryListItem> GetAll();
        List<GroceryListItem> GetAllOnGroceryListId(int groceryListId);
        GroceryListItem Add(GroceryListItem item);
        GroceryListItem Delete(GroceryListItem item);
        GroceryListItem Get(int id);
        GroceryListItem Update(GroceryListItem item);
        List<BestSellingProducts> GetBestSellingProducts(int topX = 5);
        event EventHandler ItemsChanged;
    }
}
