    using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Linq;

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
            var items = _groceriesRepository.GetAll();
            FillProducts(items);
            return items;
        }

        public List<GroceryListItem> GetAllOnGroceryListId(int groceryListId)
        {
            var items = _groceriesRepository.GetAll()
                .Where(g => g.GroceryListId == groceryListId)
                .ToList();
            FillProducts(items);
            return items;
        }

        public GroceryListItem Add(GroceryListItem item)
        {
            var added = _groceriesRepository.Add(item);
            FillProducts(new() { added });
            RaiseItemsChanged();
            return added;
        }

        public GroceryListItem Delete(GroceryListItem item)
        {
            throw new NotImplementedException();
        }

        public GroceryListItem Get(int id)
        {
            var found = _groceriesRepository.Get(id);
            if (found == null) return new GroceryListItem(0,0,0,0);
            FillProducts(new() { found });
            return found;
        }

        public GroceryListItem Update(GroceryListItem item)
        {
            var updated = _groceriesRepository.Update(item);
            if (updated == null) return new GroceryListItem(0,0,0,0);

            RaiseItemsChanged();
            return updated;
        }

        public List<BestSellingProducts> GetBestSellingProducts(int topX = 5)
        {
            if (!IsValidTopCount(topX)) return new();
            var allItems = GetAllItems();
            if (allItems.Count == 0) return new();
            var grouped = GroupByProduct(allItems);
            if (grouped.Count == 0) return new();
            var productDict = BuildProductDictionary();
            var ordered = OrderAndTake(grouped, productDict, topX);
            return BuildResult(ordered, productDict);
        }

        private bool IsValidTopCount(int topX) => topX > 0;

        private List<GroceryListItem> GetAllItems() => _groceriesRepository.GetAll();

        private List<(int ProductId,int TotalSold)> GroupByProduct(List<GroceryListItem> items) =>
            items.GroupBy(i => i.ProductId)
                 .Select(g => (g.Key, g.Sum(x => x.Amount)))
                 .ToList();

        private Dictionary<int, Product> BuildProductDictionary() =>
            _productRepository.GetAll().ToDictionary(p => p.Id, p => p);

        private List<(int ProductId,int TotalSold)> OrderAndTake(
            List<(int ProductId,int TotalSold)> grouped,
            Dictionary<int, Product> products,
            int topX) =>
            grouped.OrderByDescending(g => g.TotalSold)
                   .ThenBy(g => products.TryGetValue(g.ProductId, out var p) ? p.Name : string.Empty)
                   .Take(topX)
                   .ToList();

        private List<BestSellingProducts> BuildResult(
            List<(int ProductId,int TotalSold)> ordered,
            Dictionary<int, Product> products)
        {
            var result = new List<BestSellingProducts>();
            int rank = 1;
            foreach (var entry in ordered)
            {
                products.TryGetValue(entry.ProductId, out var product);
                var name = product != null ? product.Name : "Unknown";
                var stock = product != null ? product.Stock : 0;
                result.Add(new BestSellingProducts(entry.ProductId, name, stock, entry.TotalSold, rank));
                rank++;
            }
            return result;
        }

        private void FillProducts(List<GroceryListItem> items)
        {
            foreach (var g in items)
            {
                var p = _productRepository.Get(g.ProductId);
                g.Product = p ?? new Product(0,"",0);
            }
        }

        private void RaiseItemsChanged() => ItemsChanged(this, EventArgs.Empty);
    }
}
