using Avalonia.Controls;
using Avalonia.Interactivity;

namespace FrontendAdmin.Views;

public partial class ProductPageView : UserControl
{
    public ProductPageView()
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