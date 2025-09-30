using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.ObjectModel;

namespace Grocery.App.ViewModels
{
    public partial class GroceryListViewModel : BaseViewModel
    {
        public ObservableCollection<GroceryList> GroceryLists { get; set; }

        private readonly IGroceryListService _groceryListService;

        // Client wordt gebruikt voor Toolbar binding (Client.Name) en rolcontrole
        [ObservableProperty]
        private Client client = new(0, "Onbekend", "unknown@example.com", "", Role.None);

        public GroceryListViewModel(IGroceryListService groceryListService)
        {
            Title = "Boodschappenlijst";
            _groceryListService = groceryListService;
            GroceryLists = new(_groceryListService.GetAll());
        }

        [RelayCommand]
        public async Task SelectGroceryList(GroceryList groceryList)
        {
            Dictionary<string, object> paramater = new() { { nameof(GroceryList), groceryList } };
            await Shell.Current.GoToAsync($"{nameof(Views.GroceryListItemsView)}?Titel={groceryList.Name}", true, paramater);
        }

        [RelayCommand]
        public async Task ShowBoughtProducts()
        {
            // Alleen navigeren indien admin
            if (Client?.Role == Role.Admin)
            {
                await Shell.Current.GoToAsync(nameof(Views.BoughtProductsView));
            }
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            GroceryLists = new(_groceryListService.GetAll());
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
            GroceryLists.Clear();
        }
    }
}
