using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.ObjectModel;
using Microsoft.Maui.ApplicationModel;

namespace Grocery.App.ViewModels
{
    public partial class BestSellingProductsViewModel : BaseViewModel
    {
        private readonly IGroceryListItemsService _groceryListItemsService;
        public ObservableCollection<BestSellingProducts> Products { get; set; } = new ObservableCollection<BestSellingProducts>();

        public BestSellingProductsViewModel(IGroceryListItemsService groceryListItemsService)
        {
            _groceryListItemsService = groceryListItemsService;
            Title = "Best verkochte producten";
            _groceryListItemsService.ItemsChanged += OnItemsChanged;
            Load();
        }

        private void OnItemsChanged(object sender, System.EventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() => Load());
        }

            public override void Load()
        {
            var list = _groceryListItemsService.GetBestSellingProducts();
            Products.Clear();
            foreach (var item in list)
                Products.Add(item);
        }

        public override void OnAppearing()
        {
            Load();
        }

        public override void OnDisappearing()
        {
        }
    }
}
