using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.ObjectModel;


namespace Grocery.App.ViewModels
{
    public partial class BoughtProductsViewModel : BaseViewModel
    {
        private readonly IBoughtProductsService _boughtProductsService;

        [ObservableProperty]
        private Product? selectedProduct; // nullable, geen auto-select

        public ObservableCollection<BoughtProducts> BoughtProductsList { get; } = [];
        public ObservableCollection<Product> Products { get; }

        public BoughtProductsViewModel(IBoughtProductsService boughtProductsService, IProductService productService)
        {
            Title = "Gekochte producten";
            _boughtProductsService = boughtProductsService;
            Products = new(productService.GetAll());
        }

        partial void OnSelectedProductChanged(Product? oldValue, Product? newValue)
        {
            BoughtProductsList.Clear();
            if (newValue is null) return;

            var items = _boughtProductsService.Get(newValue.Id);
            foreach (var bp in items)
                BoughtProductsList.Add(bp);
        }

        [RelayCommand]
        public void NewSelectedProduct(Product product)
        {
            SelectedProduct = product;
        }
    }
}
