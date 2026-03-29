using System.Collections.Generic;
using System.Collections.ObjectModel;
using FrontendAdmin.Models;
using ReactiveUI;

namespace FrontendAdmin.ViewModels.Product;

public class ProductPageViewModel : ReactiveObject
{
    public ObservableCollection<ProductViewModel> Products { get; } = new();

    public void Load(IEnumerable<ProductModel> models)
    {
        Products.Clear();

        foreach (var model in models)
            Products.Add(new ProductViewModel(model));
    }
}