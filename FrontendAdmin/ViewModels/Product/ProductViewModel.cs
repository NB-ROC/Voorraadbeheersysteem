using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Media.Imaging;
using FrontendAdmin.Grpc;
using FrontendAdmin.Models;
using FrontendAdmin.Services;
using Grpc.Core;
using Protos.Product;
using ReactiveUI;

namespace FrontendAdmin.ViewModels.Product;

public class ProductViewModel : ReactiveObject
{
    private readonly ProductModel _model;
    private readonly ProductPageViewModel _parentPage;

    public ProductViewModel(ProductModel model, NavigationService navigation, ProductPageViewModel parent)
    {
        _model = model;
        _parentPage = parent;

        EditCommand = ReactiveCommand.Create<ProductViewModel>(product =>
        {
            var formVm = new ProductFormViewModel(navigation, parent, product);
            navigation.NavigateTo(formVm);
        });
        DeleteCommand = ReactiveCommand.CreateFromTask(DeleteAsync);

        _ = LoadImageAsync();
    }

    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }

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

    public string Category
    {
        get => _model.Category;
        set
        {
            if (_model.Category == value) return;
            _model.Category = value;
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

    public int Amount
    {
        get => _model.Amount;
        set
        {
            if (_model.Amount == value) return;
            _model.Amount = value;
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
        if (success) _parentPage.Products.Remove(this);
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