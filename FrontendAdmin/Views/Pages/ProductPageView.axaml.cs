using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace FrontendAdmin.Views.Pages;

public partial class ProductPageView : UserControl
{
    public ProductPageView()
    {
        InitializeComponent();
    }

    private void OnEditClick(object sender, RoutedEventArgs e) => throw new NotImplementedException();
    private void OnDeleteClick(object sender, RoutedEventArgs e) => throw new NotImplementedException();

    private void OnAddProductClick(object? sender, RoutedEventArgs e)
    {
        MainWindow? mainWindow = TopLevel.GetTopLevel(this) as MainWindow;
    }
}