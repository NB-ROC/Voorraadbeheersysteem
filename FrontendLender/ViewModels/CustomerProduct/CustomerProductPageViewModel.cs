using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Frontend.Models;
using Frontend.Services;
using Frontend.ViewModels.Components;
using Frontend.ViewModels.Product;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace Frontend.ViewModels.CustomerProduct;

public class CustomerProductPageViewModel : PageViewModelBase
{
    private readonly IApiService _api;

    public CustomerProductPageViewModel(HeaderViewModel header, FooterViewModel footer, IApiService api) : base(header, footer)
    {
        _api = api;
    }

    public ObservableCollection<ProductViewModel> Products { get; } = [];

    public override async Task LoadAsync()
    {
        await LoadProducts();
    }

    private async Task LoadProducts()
    {
        (RequestResult result, List<ProductModel> models) = await _api.Products.Page(1, 20);

        Products.Clear();
        foreach (ProductModel model in models) Products.Add(new ProductViewModel(_api, model));
        this.RaisePropertyChanged(nameof(Products));
    }
}