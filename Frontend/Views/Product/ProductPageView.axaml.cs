using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Frontend.Views.Product;

public partial class ProductPageView : UserControl
{
    public ProductPageView()
    {
        InitializeComponent();
    }

    private void OnEditClick(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void OnDeleteClick(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void OnAddProductClick(object? sender, RoutedEventArgs e)
    {
        MainWindowView? mainWindow = TopLevel.GetTopLevel(this) as MainWindowView;
    }
}