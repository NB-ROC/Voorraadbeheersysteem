using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Input;
using FrontendAdmin.Grpc;
using FrontendAdmin.Models;
using FrontendAdmin.Services;
using Protos.Product;
using ReactiveUI;

namespace FrontendAdmin.ViewModels.Product;

public class ProductPageViewModel : ReactiveObject
{
    public ProductPageViewModel(INavigationService navigation)
    {
        Navigation = navigation;
        LoadProductsCommand = ReactiveCommand.CreateFromTask(LoadProducts);
        LoadProductsCommand.Execute();
    }

    public INavigationService Navigation { get; }


    public ObservableCollection<ProductViewModel> Products { get; } = [];

    public ReactiveCommand<Unit, Unit> LoadProductsCommand { get; }

    private async Task LoadProducts()
    {
        ProductPageResponse? result = await Client.Products.PageAsync(new ProductPageRequest
        {
            Page = 1,
            PageSize = 20
        });

        Products.Clear();
        foreach (MetaProduct? product in result.Products)
            Products.Add(new ProductViewModel(new ProductModel
            {
                Id = product.Id,
                Name = product.Name,
                Category = product.Category,
                Description = product.Description,
                Amount = product.Amount,
                Image = product.Image
            }, Navigation, this));
    }
}