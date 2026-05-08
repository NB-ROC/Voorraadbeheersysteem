using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Frontend.Grpc;
using Microsoft.Extensions.DependencyInjection;
using Protos.Product;
using ReactiveUI;

namespace Frontend.ViewModels.CustomerProduct;

public class CustomerProductPageViewModel : PageViewModelBase
{
    private readonly ServiceProvider _services;
    
    // public ObservableCollection<ProductViewModel> Products { get; } = [];

    private bool _isLoading;

    public bool IsLoading
    {
        get => _isLoading;
        set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }

    public CustomerProductPageViewModel(ServiceProvider services) : base(services)
    {
        _services = services;

        _ = LoadProducts();
    }

    private async Task LoadProducts()
    {
        try
        {
            IsLoading = true;

            ProductPageResponse? result =
                await Client.Products.PageAsync(
                    new ProductPageRequest
                    {
                        Page = 1,
                        PageSize = 20
                    }
                );

            // Products.Clear();

            
        }
        finally
        {
            IsLoading = false;
        }
    }
}