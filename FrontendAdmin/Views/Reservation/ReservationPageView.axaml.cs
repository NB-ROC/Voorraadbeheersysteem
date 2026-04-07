using Avalonia.Controls;
using Avalonia.Interactivity;

namespace FrontendAdmin.Views.Reservation;

public partial class ReservationPageView : UserControl
{
    public ReservationPageView()
    {
        InitializeComponent();
    }

    private void OnAnnuleerClick(object? sender, RoutedEventArgs e)
    {
        MainWindow? mainWindow = TopLevel.GetTopLevel(this) as MainWindow;
    }

    private void OnBevestigClick(object? sender, RoutedEventArgs e)
    {
        // handle reservation confirmation here later
    }
}