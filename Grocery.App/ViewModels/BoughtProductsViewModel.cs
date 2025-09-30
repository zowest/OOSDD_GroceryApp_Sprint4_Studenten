using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.ObjectModel;
using System.Linq;


namespace Grocery.App.ViewModels
{
    public partial class BoughtProductsViewModel : BaseViewModel
    {
        private readonly IBoughtProductsService _boughtProductsService;

        [ObservableProperty]
        private Product? selectedProduct; // nullable, geen auto-select

        private string _infoMessage = "Geen product geselecteerd.";
        public string InfoMessage
        {
            get => _infoMessage;
            set => SetProperty(ref _infoMessage, value);
        }

        public ObservableCollection<BoughtProducts> BoughtProductsList { get; } = [];
        public ObservableCollection<Product> Products { get; }

        public bool HasSelection => SelectedProduct is not null;

        public BoughtProductsViewModel(IBoughtProductsService boughtProductsService, IProductService productService)
        {
            Title = "Gekochte producten (Admin)";
            _boughtProductsService = boughtProductsService;
            Products = new(productService.GetAll());
        }

        partial void OnSelectedProductChanged(Product? oldValue, Product? newValue)
        {
            BoughtProductsList.Clear();
            if (newValue is null)
            {
                InfoMessage = "Geen product geselecteerd.";
                OnPropertyChanged(nameof(HasSelection));
                return;
            }

            var items = _boughtProductsService.Get(newValue.Id);
            foreach (var bp in items)
                BoughtProductsList.Add(bp);

            if (items.Count == 0)
            {
                InfoMessage = $"Geen aankopen gevonden voor {newValue.Name}.";
            }
            else
            {
                var distinctClients = items.Select(i => i.Client.Id).Distinct().Count();
                var dates = items.Select(i => i.GroceryList.Date).OrderBy(d => d).ToList();
                var firstDate = dates.First();
                var lastDate = dates.Last();
                InfoMessage = $"{newValue.Name}: {items.Count} keer gekocht door {distinctClients} klant(en) tussen {firstDate} en {lastDate}.";
            }
            OnPropertyChanged(nameof(HasSelection));
        }

        [RelayCommand]
        public void NewSelectedProduct(Product product)
        {
            SelectedProduct = product;
        }
    }
}
