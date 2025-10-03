using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.App.Views;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.ObjectModel;
using System.Text.Json;
using System;
using System.Linq;

namespace Grocery.App.ViewModels
{
    [QueryProperty(nameof(GroceryList), nameof(GroceryList))]
    public partial class GroceryListItemsViewModel : BaseViewModel
    {
        private readonly IGroceryListItemsService _groceryListItemsService;
        private readonly IProductService _productService;
        private readonly IFileSaverService _fileSaverService;

        private const string ExportFileName = "Boodschappen.json";

        // Cache van alle producten om herhaalde service calls te vermijden
        private readonly List<Product> _allProducts;

        // Huidige zoektekst om beschikbare producten te filteren
        private string searchText = string.Empty;

        // Items die al aan de huidige boodschappenlijst zijn toegevoegd
        public ObservableCollection<GroceryListItem> MyGroceryListItems { get; set; } = [];

        // Producten die nog toegevoegd kunnen worden
        public ObservableCollection<Product> AvailableProducts { get; set; } = [];

        // Actieve boodschappenlijst
        [ObservableProperty]
        GroceryList groceryList = new(0, "None", DateOnly.MinValue, string.Empty, 0);

        // Bericht voor UI
        [ObservableProperty]
        string myMessage = string.Empty;

        public GroceryListItemsViewModel(
            IGroceryListItemsService groceryListItemsService,
            IProductService productService,
            IFileSaverService fileSaverService)
        {
            _groceryListItemsService = groceryListItemsService;
            _productService = productService;
            _fileSaverService = fileSaverService;

            _allProducts = _productService.GetAll();

            // Eerste load
            Load(GroceryList.Id);
        }

        // Laadt lijstitems en ververst de beschikbare producten
        private void Load(int id)
        {
            FillMyItems(id);
            RefreshAvailableProducts();
        }

        // Vult de collectie met items die horen bij de opgegeven lijst
        private void FillMyItems(int id)
        {
            MyGroceryListItems.Clear();
            foreach (var item in _groceryListItemsService.GetAllOnGroceryListId(id))
                MyGroceryListItems.Add(item);
        }

        // Stelt de lijst samen van nog selecteerbare producten
        private void RefreshAvailableProducts()
        {
            AvailableProducts.Clear();
            foreach (var p in _allProducts.Where(IsProductSelectable))
                AvailableProducts.Add(p);
        }

        // Controle of een product toegevoegd mag worden
        private bool IsProductSelectable(Product p) =>
            !MyGroceryListItems.Any(g => g.ProductId == p.Id)
            && p.Stock > 0
            && (string.IsNullOrWhiteSpace(searchText) || p.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase));

        // Wordt aangeroepen wanneer GroceryList via binding verandert
        partial void OnGroceryListChanged(GroceryList value) => Load(value.Id);

        // Navigatie naar kleur-wijzig scherm
        [RelayCommand]
        public async Task ChangeColor()
        {
            var param = new Dictionary<string, object> { { nameof(GroceryList), GroceryList } };
            await Shell.Current.GoToAsync($"{nameof(ChangeColorView)}?Name={GroceryList.Name}", true, param);
        }

        [RelayCommand]
        public void AddProduct(Product? product)
        {
            if (product is null) return;
            if (product.Stock <= 0) return;
            if (!IsProductSelectable(product)) return;

            product.Stock--;
            _productService.Update(product);

            var item = new GroceryListItem(0, GroceryList.Id, product.Id, 1)
            {
                Product = product
            };
            _groceryListItemsService.Add(item);
            MyGroceryListItems.Add(item);

            AvailableProducts.Remove(product);
        }

        // Exporteert de huidige lijst naar JSON en slaat deze op
        [RelayCommand]
        public async Task ShareGroceryList(CancellationToken token)
        {
            if (MyGroceryListItems.Count == 0) return;
            var json = JsonSerializer.Serialize(MyGroceryListItems);
            try
            {
                await _fileSaverService.SaveFileAsync(ExportFileName, json, token);
                await Toast.Make("Boodschappenlijst is opgeslagen.").Show(token);
            }
            catch (Exception ex)
            {
                await Toast.Make("Opslaan mislukt: " + ex.Message).Show(token);
            }
        }

        // Past de zoekfilter aan en ververst de beschikbare producten
        [RelayCommand]
        public void PerformSearch(string search)
        {
            searchText = search ?? string.Empty;
            RefreshAvailableProducts();
        }

        // Verhoogt de hoeveelheid van een bestaand item 
        [RelayCommand]
        public void IncreaseAmount(int productId)
        {
            var item = MyGroceryListItems.FirstOrDefault(x => x.ProductId == productId);
            if (item == null) return;
            if (item.Product.Stock <= 0) return;

            item.Amount++;
            item.Product.Stock--;
            _productService.Update(item.Product);
            _groceryListItemsService.Update(item);

            // Voor nu herladen voor UI updates
            RefreshAfterMutation();
        }

        // Verlaagt de hoeveelheid
        [RelayCommand]
        public void DecreaseAmount(int productId)
        {
            var item = MyGroceryListItems.FirstOrDefault(x => x.ProductId == productId);
            if (item == null) return;
            if (item.Amount <= 0) return;

            item.Amount--;
            item.Product.Stock++;
            _productService.Update(item.Product);
            _groceryListItemsService.Update(item);

            RefreshAfterMutation();
        }

        // Methode om na mutaties te herladen 
        private void RefreshAfterMutation() => OnGroceryListChanged(GroceryList);
    }
}
