using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;
using FrontendAdmin.ViewModels;

namespace FrontendAdmin.Views;

public partial class ProductView : UserControl
{
    public ProductView()
    {
        InitializeComponent();

        this.AttachedToVisualTree += (_, _) =>
        {
            var window = this.GetVisualRoot() as Window;
            DataContext = new ProductViewModel(window);
        };
    }
}