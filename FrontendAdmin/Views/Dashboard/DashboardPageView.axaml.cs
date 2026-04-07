using Avalonia.Controls;
using Avalonia.Interactivity;

namespace FrontendAdmin.Views.Dashboard;

public partial class DashboardPageView : UserControl
{
    public DashboardPageView()
    {
        InitializeComponent();
    }

    private void OnNieuwProductClick(object? sender, RoutedEventArgs e)
    {
        MainWindowView? mainWindow = TopLevel.GetTopLevel(this) as MainWindowView;
    }

    private void OnLeningClick(object? sender, RoutedEventArgs e)
    {
        MainWindowView? mainWindow = TopLevel.GetTopLevel(this) as MainWindowView;
    }
}