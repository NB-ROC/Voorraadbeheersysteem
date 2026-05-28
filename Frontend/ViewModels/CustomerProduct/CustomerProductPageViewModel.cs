using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Frontend.Models;
using Frontend.Services;
using Frontend.ViewModels.Product;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace Frontend.ViewModels.CustomerProduct;

public class CustomerProductPageViewModel : PageViewModelBase
{
    private readonly BackendService _backend;
    private readonly ServiceProvider _services;

    private bool _isLoading;

    public CustomerProductPageViewModel(ServiceProvider services) : base(services)
    {
        _services = services;
        _backend = services.GetRequiredService<BackendService>();

        _ = LoadProducts();
    }

    public ObservableCollection<ProductViewModel> Products { get; } = [];

    public bool IsLoading
    {
        get => _isLoading;
        set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }

    private async Task LoadProducts()
    {
        await _backend.LogIn("testmail@roc-nijmegen.nl", "Placeholder1");
        try
        {
            IsLoading = true;

            (RequestResult result, List<ProductModel> models) = await _backend.Products.Page(1, 20);

            Products.Clear();
            foreach (ProductModel model in models) Products.Add(new ProductViewModel(Services, model));
            this.RaisePropertyChanged(nameof(Products));
        }
        finally
        {
            IsLoading = false;
        }
    }
}