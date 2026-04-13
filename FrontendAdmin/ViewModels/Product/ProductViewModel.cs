using System.IO;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Media.Imaging;
using FrontendAdmin.Grpc;
using FrontendAdmin.Models;
using FrontendAdmin.Services;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Protos.Product;
using ReactiveUI;

namespace FrontendAdmin.ViewModels.Product;

public class ProductViewModel : ViewModelBase
{
    private readonly ProductModel _model;

    public ProductViewModel(ServiceProvider services, ProductModel model) : base(services)
    {
        _model = model;

        EditCommand = ReactiveCommand.Create(() =>
        {
            Services.GetService<NavigationService>()?.NavigateTo(new ProductFormViewModel(Services, this));
        });
        DeleteCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await DeleteAsync();
            Services.GetService<NavigationService>()?.NavigateTo(new ProductPageViewModel(Services));
        });

        _ = LoadImageAsync();
    }

    public ReactiveCommand<Unit, Unit> EditCommand { get; }
    public ReactiveCommand<Unit, Unit> DeleteCommand { get; }

    public int Id => _model.Id;

    public string Name
    {
        get => _model.Name;
        set
        {
            if (_model.Name == value) return;
            _model.Name = value;
            this.RaisePropertyChanged();
        }
    }

    public int CategoryId
    {
        get => _model.CategoryId;
        set
        {
            if (_model.CategoryId == value) return;
            _model.CategoryId = value;
            this.RaisePropertyChanged();
        }
    }

    public string Description
    {
        get => _model.Description;
        set
        {
            if (_model.Description == value) return;
            _model.Description = value;
            this.RaisePropertyChanged();
        }
    }

    public string Image
    {
        get => _model.Image;
        set
        {
            if (_model.Image == value) return;
            _model.Image = value;
            this.RaisePropertyChanged();
        }
    }

    public bool ImageFailed
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public bool IsImageLoading
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public Bitmap? Thumbnail
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    private async Task DeleteAsync()
    {
        bool success = (await Client.Products.DeleteAsync(new ProductDeleteRequest { Id = Id })).Success;
    }

    private async Task LoadImageAsync()
    {
        IsImageLoading = true;

        try
        {
            using MemoryStream ms = new();

            using AsyncServerStreamingCall<ProductImageResponse>? call = Client.Products.Image(new ProductImageRequest
            {
                Name = Image
            });

            await foreach (ProductImageResponse chunk in call.ResponseStream.ReadAllAsync())
            {
                byte[] bytes = chunk.Raw.ToByteArray();
                await ms.WriteAsync(bytes);
            }

            ms.Seek(0, SeekOrigin.Begin);
            Thumbnail = new Bitmap(ms);
        }
        catch
        {
            ImageFailed = true;
        }
        finally
        {
            IsImageLoading = false;
        }
    }
}