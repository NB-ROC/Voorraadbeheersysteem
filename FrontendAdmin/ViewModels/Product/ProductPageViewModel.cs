using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using FrontendAdmin.Grpc;
using FrontendAdmin.Models;
using FrontendAdmin.Services;
using Microsoft.Extensions.DependencyInjection;
using Protos.Product;
using ReactiveUI;

namespace FrontendAdmin.ViewModels.Product;

public class ProductPageViewModel : PageViewModelBase
{
    public ProductPageViewModel(ServiceProvider services) : base(services)
    {
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
        ProductPageResponse? result = await Client.Products.PageAsync(new ProductPageRequest
        {
            Page = 1,
            PageSize = 20
        });

        Products.Clear();
        foreach (MetaProduct? product in result.Products)
            Products.Add(
                new ProductViewModel(Services, new ProductModel
                    {
                        Id = product.Id,
                        Name = product.Name,
                        CategoryId = product.CategoryId,
                        Description = product.Description,
                        Image = product.Image
                    }
                )
            );
    }
}