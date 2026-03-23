using Avalonia.Controls;
using Avalonia.Interactivity;

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
            "Dashboard" => new DashboardView(),
            "Items" => new ItemsView(),
            "Leningen" => new LeningenView(),
            "Profiel" => new ProfielView(),
            "Reservering" => new ReserveringView(),
            "Product" => new ProductView(),
            _ => new DashboardView()
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