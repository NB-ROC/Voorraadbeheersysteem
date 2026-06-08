using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using FrontendAdmin.Models;
using FrontendAdmin.Services;
using FrontendAdmin.ViewModels.Components;
using ReactiveUI;

namespace FrontendAdmin.ViewModels.Product;

public class ProductPageViewModel : PageViewModelBase
{
    private readonly IApiService _api;
    private readonly INavigationService _navigation;

    public ProductPageViewModel(HeaderViewModel header, FooterViewModel footer, IApiService api,
        INavigationService navigationService) : base(header, footer)
    {
        _api = api;
        _navigation = navigationService;

        NavigateProductForm = ReactiveCommand.CreateFromTask(async () =>
            await _navigation.NavigateTo<ProductFormViewModel, ProductModel>());
    }


    public ObservableCollection<ProductViewModel> Products { get; } = [];

    public ReactiveCommand<Unit, Unit> NavigateProductForm { get; }

    private async Task EditProduct(ProductModel product)
    {
        await _navigation.NavigateTo<ProductFormViewModel, ProductModel>(product);
    }

    private async Task DeleteProduct(ProductViewModel product)
    {
        (RequestResult, bool) valueTuple = await _api.Products.Delete(product.Id);

        if (valueTuple is { Item1: RequestResult.Success, Item2: true }) Products.Remove(product);
    }

    public override async Task LoadAsync()
    {
        Products.Clear();

        (RequestResult result, List<ProductModel> products) = await _api.Products.Page(1, 20);

        if (result != RequestResult.Success) return;

        foreach (ProductModel product in products)
        {
            Console.WriteLine(product.Id);
            ProductViewModel productViewModel = new(_api, product, EditProduct, DeleteProduct);
            await productViewModel.LoadImageAsync();
            Products.Add(
                productViewModel
            );
        }
    }
}