using Avalonia.Controls;
using Avalonia.Interactivity;

namespace FrontendAdmin.Views;

public partial class DashboardView : UserControl
{
    public DashboardView()
    {
        InitializeComponent();
    }

    private void OnNieuwItemClick(object? sender, RoutedEventArgs e)
    {
        MainWindow? mainWindow = TopLevel.GetTopLevel(this) as MainWindow;
        mainWindow?.NavigateTo("Items");
    }

    private void OnLeningClick(object? sender, RoutedEventArgs e)
    {
        MainWindow? mainWindow = TopLevel.GetTopLevel(this) as MainWindow;
        mainWindow?.NavigateTo("Leningen");
    }
}