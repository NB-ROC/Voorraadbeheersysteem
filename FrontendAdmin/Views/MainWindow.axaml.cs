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
            "Dashboard" => new DashboardPage(),
            "Items" => new Pages.ProductPage(),
            "Leningen" => new Pages.LoanPage(),
            "Profiel" => new Pages.ProfilePage(),
            "Reservering" => new ReserveringView(),
            "Product" => new Forms.ProductForm(),
            _ => new DashboardPage()
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