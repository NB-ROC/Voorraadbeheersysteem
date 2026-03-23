using CommunityToolkit.Mvvm.ComponentModel;

namespace FrontendAdmin.ViewModels;

public partial class ProductItemViewModel : ObservableObject
{
    [ObservableProperty]
    private int id;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string category = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private string image = string.Empty;

    [ObservableProperty]
    private int amount;
}