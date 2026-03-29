using Avalonia.Controls;
using Avalonia.Interactivity;

namespace FrontendAdmin.Views.Pages;

public partial class DashboardPageView : UserControl
{
    public DashboardPageView()
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