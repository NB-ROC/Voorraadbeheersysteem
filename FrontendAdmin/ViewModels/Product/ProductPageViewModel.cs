using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using FrontendAdmin.Models;
using FrontendAdmin.Services;
using FrontendAdmin.ViewModels.Components;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace FrontendAdmin.ViewModels.Product;

public class ProductPageViewModel : PageViewModelBase
{
    private readonly IApiService _api;

    public ProductPageViewModel(HeaderViewModel header, FooterViewModel footer, IApiService api) : base(header, footer)
    {
        _api = api;
        NavigateProductForm = ReactiveCommand.Create(() =>
        {
            // TODO: Implement navigation to forms
            // Services.GetService<NavigationService>()?.NavigateTo(new ProductFormViewModel(Services));
        });
    }


    public ObservableCollection<ProductViewModel> Products { get; } = [];

    public ReactiveCommand<Unit, Unit> NavigateProductForm { get; }

    private void EditProduct(ProductViewModel product)
    {
        // TODO: Implement navigation to forms
    }

    private void DeleteProduct(ProductViewModel product)
    {
        // TODO: Implement navigation to forms
    }

    public override async Task LoadAsync()
    {
        Products.Clear();
        
        (RequestResult result, List<ProductModel> products) = await _api.Products.Page(1, 20);

        if (result != RequestResult.Success) return;
        
        foreach (ProductModel product in products)
        {
            Console.WriteLine(product.Id);
            ProductViewModel productViewModel = new ProductViewModel(_api, product, EditProduct, DeleteProduct);
            await productViewModel.LoadImageAsync();
            Products.Add(
                productViewModel
            );
        }
    }
}