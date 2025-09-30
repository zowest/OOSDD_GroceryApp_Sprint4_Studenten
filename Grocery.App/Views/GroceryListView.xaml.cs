using Grocery.App.ViewModels;

namespace Grocery.App.Views;

public partial class GroceryListView : ContentPage
{
    public GroceryListView(GroceryListViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}