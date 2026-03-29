using Avalonia.Controls;
using Avalonia.Interactivity;
using FrontendAdmin.Views.Pages;

namespace FrontendAdmin.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        NavigateTo("Dashboard"); // default page on launch
    }

    public void NavigateTo(string page)
    {
        ContentControl? content = this.FindControl<ContentControl>("PageContent");
        content!.Content = page switch
        {
            "Dashboard" => new DashboardPageView(),
            "Items" => new ProductPageView(),
            "Leningen" => new LoanViewPage(),
            "Profiel" => new ProfilePageView(),
            "Reservering" => new ReserveringView(),
            "Product" => new ProductView(),
            _ => new DashboardPageView()
        };
    }

    private void OnDashboardClick(object? sender, RoutedEventArgs e)
    {
        NavigateTo("Dashboard");
    }

    private void OnItemsClick(object? sender, RoutedEventArgs e)
    {
        NavigateTo("Items");
    }

    private void OnLeningenClick(object? sender, RoutedEventArgs e)
    {
        NavigateTo("Leningen");
    }

    private void OnProfielClick(object? sender, RoutedEventArgs e)
    {
        NavigateTo("Profiel");
    }
}