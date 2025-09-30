using CommunityToolkit.Mvvm.ComponentModel;
using Grocery.Core.Models;

namespace Grocery.App.ViewModels
{
    public partial class GlobalViewModel : BaseViewModel
    {
        // Wordt gezet na succesvolle login; start als null (geen hardcoded fallback).
        [ObservableProperty]
        private Client? client;
    }
}
