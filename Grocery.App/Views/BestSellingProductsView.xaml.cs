using Grocery.App.ViewModels;

namespace Grocery.App.Views;

public partial class BestSellingProductsView : ContentPage
{
    public BestSellingProductsView(BestSellingProductsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}