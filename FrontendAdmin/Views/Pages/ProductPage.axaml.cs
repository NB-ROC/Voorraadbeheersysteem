using Avalonia.Controls;
using Avalonia.Interactivity;

namespace FrontendAdmin.Views.Pages;

public partial class ProductPage : UserControl
{
    public ProductPage()
    {
        InitializeComponent();
    }

    private void OnReserveerClick(object? sender, RoutedEventArgs e)
    {
        MainWindow? mainWindow = TopLevel.GetTopLevel(this) as MainWindow;
        mainWindow?.NavigateTo("Reservering");
    }

    private void OnAddProductClick(object? sender, RoutedEventArgs e)
    {
        MainWindow? mainWindow = TopLevel.GetTopLevel(this) as MainWindow;
        mainWindow?.NavigateTo("Product");
    }
}