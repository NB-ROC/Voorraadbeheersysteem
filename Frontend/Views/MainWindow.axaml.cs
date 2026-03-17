using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Grpc.Net.Client;

namespace Frontend.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void ProductKlik(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            button.Background = Brushes.LightGreen;
        }
    }
    private void OpenBeheer(object? sender, RoutedEventArgs e)
    {
        var window = new ProductBeheer();
        window.Show();
    }
}