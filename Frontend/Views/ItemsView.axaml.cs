using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Frontend.Views;

public partial class ItemsView : UserControl
{
    public ItemsView() => InitializeComponent();

    private void OnReserveerClick(object? sender, RoutedEventArgs e)
    {
        var mainWindow = TopLevel.GetTopLevel(this) as MainWindow;
        mainWindow?.NavigateTo("Reservering");
    }
}