using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Grocery.App.ViewModels
{
    public partial class GroceryListViewModel : BaseViewModel
    {
        private readonly IGroceryListService _groceryListService;
        private readonly GlobalViewModel _global;

        public ObservableCollection<GroceryList> GroceryLists { get; private set; } = new();

        // Gebruikt door ToolbarItem binding
        public Client? Client => _global.Client;

        public GroceryListViewModel(IGroceryListService groceryListService, GlobalViewModel global)
        {
            Title = "Boodschappenlijst";
            _groceryListService = groceryListService;
            _global = global;
            _global.PropertyChanged += OnGlobalPropertyChanged;
            GroceryLists = new(_groceryListService.GetAll());
        }

        private void OnGlobalPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GlobalViewModel.Client))
                OnPropertyChanged(nameof(Client));
        }

        [RelayCommand]
        public async Task SelectGroceryList(GroceryList groceryList)
        {
            if (groceryList is null) return;
            var parameter = new Dictionary<string, object> { { nameof(GroceryList), groceryList } };
            await Shell.Current.GoToAsync($"{nameof(Views.GroceryListItemsView)}?Titel={groceryList.Name}", true, parameter);
        }

        [RelayCommand]
        public async Task ShowBoughtProducts()
        {
            if (Client?.Role == Role.Admin)
                await Shell.Current.GoToAsync(nameof(Views.BoughtProductsView));
        }

        [RelayCommand]
        public async Task ShowBestSellingProducts()
        {
            await Shell.Current.GoToAsync(nameof(Views.BestSellingProductsView));
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            GroceryLists = new(_groceryListService.GetAll());
            OnPropertyChanged(nameof(GroceryLists));
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
            GroceryLists.Clear();
        }
    }
}
