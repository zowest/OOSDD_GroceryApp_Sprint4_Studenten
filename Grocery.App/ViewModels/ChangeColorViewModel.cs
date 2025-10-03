using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using Grocery.Core.Services;

namespace Grocery.App.ViewModels
{
    // Maakt query-parameter binding mogelijk voor Navigation.
    [QueryProperty(nameof(GroceryList), nameof(GroceryList))]
    public partial class ChangeColorViewModel : BaseViewModel
    {
        private readonly IGroceryListService _groceryListService;

        // Observable property voor databinding; start met een lege/standaard lijst
        [ObservableProperty]
        GroceryList groceryList = new(0, "", DateOnly.MinValue, "", 0);

        // Dependency injection van de service
        public ChangeColorViewModel(IGroceryListService groceryListService)
        {
            _groceryListService = groceryListService;
        }

        // Wordt aangeroepen wanneer GroceryList verandert.
        partial void OnGroceryListChanged(GroceryList value)
        {
            GroceryList = _groceryListService.Update(value);
        }

        // Command dat vanuit de UI wordt aangeroepen om de kleur te wijzigen
        [RelayCommand]
        private async Task ChangeColor(string color)
        {
            GroceryList.Color = color;          
            OnGroceryListChanged(GroceryList);  
            await Shell.Current.GoToAsync(".."); 
        }
    }
}
