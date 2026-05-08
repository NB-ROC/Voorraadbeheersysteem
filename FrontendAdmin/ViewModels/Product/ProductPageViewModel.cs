using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using FrontendAdmin.Models;
using FrontendAdmin.Services;
using Microsoft.Extensions.DependencyInjection;
using Protos.Product;
using ReactiveUI;

namespace FrontendAdmin.ViewModels.Product;

public class ProductPageViewModel : PageViewModelBase
{
    private readonly BackendService _backend;
    
    public ProductPageViewModel(ServiceProvider services) : base(services)
    {
        _backend = Services.GetService<BackendService>() ??
                   throw new NullReferenceException("Backend service not initialised");
        
        _ = LoadProducts();
        NavigateProductForm = ReactiveCommand.Create(() =>
        {
            Services.GetService<NavigationService>()?.NavigateTo(new ProductFormViewModel(Services));
        });
    }


    public ObservableCollection<ProductViewModel> Products { get; } = [];

    public ReactiveCommand<Unit, Unit> NavigateProductForm { get; }

    private async Task LoadProducts()
    {

        (RequestResult result, List<ProductModel> products) = await _backend.Products.Page(1, 20);
        
        Products.Clear();
        foreach (ProductModel product in products)
            Products.Add(
                new ProductViewModel(Services, product
                )
            );
    }
}