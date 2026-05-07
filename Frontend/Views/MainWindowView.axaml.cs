using Avalonia.Controls;
using Avalonia.Interactivity;
using Frontend.ViewModels;

namespace Frontend.Views;

public partial class MainWindowView : Window
{
    public MainWindowView()
    {
        InitializeComponent();
        
    }
    private void Login_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
        {
            vm.Login();
        }
    }
}