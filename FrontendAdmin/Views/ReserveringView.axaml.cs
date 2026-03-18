using Avalonia.Controls;
using Avalonia.Interactivity;

namespace FrontendAdmin.Views;

public partial class ReserveringView : UserControl
{
    public ReserveringView()
    {
        InitializeComponent();
    }

    private void OnAnnuleerClick(object? sender, RoutedEventArgs e)
    {
        MainWindow? mainWindow = TopLevel.GetTopLevel(this) as MainWindow;
        mainWindow?.NavigateTo("Items");
    }

    private void OnBevestigClick(object? sender, RoutedEventArgs e)
    {
        // handle reservation confirmation here later
    }
}