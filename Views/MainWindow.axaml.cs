using Avalonia.Controls;
using Avalonia.Interactivity;
using StorageBorrowManagement.Tests;

namespace StorageBorrowManagement.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        TestDatabase.TestConnection();
    }
}