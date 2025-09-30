using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.ObjectModel;
using Microsoft.Maui.ApplicationModel;

namespace Grocery.App.ViewModels
{
    public partial class BestSellingProductsViewModel : BaseViewModel
    {
        private readonly IGroceryListItemsService _groceryListItemsService;
        public ObservableCollection<BestSellingProducts> Products { get; } = new();

        public BestSellingProductsViewModel(IGroceryListItemsService groceryListItemsService)
        {
            _groceryListItemsService = groceryListItemsService;
            Title = "Best verkochte producten";
            _groceryListItemsService.ItemsChanged += OnItemsChanged;
            Load();
        }

        private void OnItemsChanged(object sender, EventArgs e) =>
            MainThread.BeginInvokeOnMainThread(Load);

        public override void Load()
        {
            Products.Clear();
            foreach (var item in _groceryListItemsService.GetBestSellingProducts())
                Products.Add(item);
        }

        public override void OnAppearing() => Load();
    }
}
