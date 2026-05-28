using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Frontend.Models;
using Frontend.Services;
using Frontend.ViewModels.Product;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace Frontend.ViewModels.CustomerProduct;

public class CustomerProductPageViewModel : PageViewModelBase
{
    private BackendService _backend;
    private readonly ServiceProvider _services;
    
    public ObservableCollection<ProductViewModel> Products { get; } = [];

    private bool _isLoading;

    public bool IsLoading
    {
        get => _isLoading;
        set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }

    public CustomerProductPageViewModel(ServiceProvider services) : base(services)
    {
        _services = services;
        _backend = services.GetRequiredService<BackendService>();
        _services.GetRequiredService<ScannerService>().SetCallback((bytes =>
        {
            Console.WriteLine(string.Join(",", bytes));
        }));

        _ = LoadProducts();
    }

    private async Task LoadProducts()
    {
        await _backend.LogIn("testmail@roc-nijmegen.nl", "Placeholder1");
        try
        {
            IsLoading = true;

             (RequestResult result, List<ProductModel> models) = await _backend.Products.Page(1, 20);

            Products.Clear();
            foreach (ProductModel model in models)
            {
                Products.Add(new ProductViewModel(Services,  model));
            }
            this.RaisePropertyChanged(nameof(Products));
        }
        finally
        {
            IsLoading = false;
        }
    }
}