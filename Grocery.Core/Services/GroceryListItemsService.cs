using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Linq;
using System;

namespace Grocery.Core.Services
{
    public class GroceryListItemsService : IGroceryListItemsService
    {
        private readonly IGroceryListItemsRepository _groceriesRepository;
        private readonly IProductRepository _productRepository;

        public event EventHandler ItemsChanged = delegate { };

        public GroceryListItemsService(IGroceryListItemsRepository groceriesRepository, IProductRepository productRepository)
        {
            _groceriesRepository = groceriesRepository;
            _productRepository = productRepository;
        }

        public List<GroceryListItem> GetAll()
        {
            List<GroceryListItem> groceryListItems = _groceriesRepository.GetAll();
            FillService(groceryListItems);
            return groceryListItems;
        }

        public List<GroceryListItem> GetAllOnGroceryListId(int groceryListId)
        {
            List<GroceryListItem> groceryListItems = _groceriesRepository.GetAll().Where(g => g.GroceryListId == groceryListId).ToList();
            FillService(groceryListItems);
            return groceryListItems;
        }

        public GroceryListItem Add(GroceryListItem item)
        {
            var added = _groceriesRepository.Add(item);
            FillService([added]);
            RaiseItemsChanged();
            return added;
        }

        public GroceryListItem? Delete(GroceryListItem item)
        {
            throw new NotImplementedException();
        }

        public GroceryListItem? Get(int id)
        {
            return _groceriesRepository.Get(id);
        }

        public GroceryListItem? Update(GroceryListItem item)
        {
            var updated = _groceriesRepository.Update(item);
            if (updated != null)
            {
                FillService([updated]);
                RaiseItemsChanged();
            }
            return updated;
        }

        public List<BestSellingProducts> GetBestSellingProducts(int topX = 5)
        {
            if (topX <= 0) return [];

            var allItems = _groceriesRepository.GetAll();
            if (allItems.Count == 0) return [];

            var grouped = allItems
                .GroupBy(i => i.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalSold = g.Sum(x => x.Amount)
                })
                .ToList();

            if (grouped.Count == 0) return [];

            var productDict = _productRepository
                .GetAll()
                .ToDictionary(p => p.Id, p => p);

            var ordered = grouped
                .OrderByDescending(g => g.TotalSold)
                .ThenBy(g =>
                {
                    if (productDict.TryGetValue(g.ProductId, out var prod) && prod != null)
                        return prod.Name;
                    return string.Empty;
                })
                .Take(topX)
                .ToList();

            List<BestSellingProducts> result = [];
            int rank = 1;
            foreach (var entry in ordered)
            {
                productDict.TryGetValue(entry.ProductId, out var product);
                string name = product?.Name ?? "Unknown";
                int stock = product?.Stock ?? 0;

                result.Add(new BestSellingProducts(entry.ProductId, name, stock, entry.TotalSold, rank));
                rank++;
            }

            return result;
        }

        private void FillService(List<GroceryListItem> groceryListItems)
        {
            foreach (GroceryListItem g in groceryListItems)
            {
                g.Product = _productRepository.Get(g.ProductId) ?? new(0, "", 0);
            }
        }

        private void RaiseItemsChanged() => ItemsChanged?.Invoke(this, EventArgs.Empty);
    }
}
