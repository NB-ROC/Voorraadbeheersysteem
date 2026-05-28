using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Frontend.Models;
using Frontend.Services;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace Frontend.ViewModels.Product;

public class ProductViewModel : ViewModelBase
{
    private readonly BackendService _backend;
    private readonly ProductModel _model;

    public ProductViewModel(ServiceProvider services, ProductModel model) : base(services)
    {
        _model = model;
        _backend = services.GetRequiredService<BackendService>();

        _ = LoadImageAsync();
    }

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

    public CategoryModel Category
    {
        get => _model.Category;
        set
        {
            if (_model.Category == value) return;
            _model.Category = value;
            this.RaisePropertyChanged();
        }
    }

    public string CategoryName => _model.Category.Name;

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
        get => _model.ImageName;
        set
        {
            if (_model.ImageName == value) return;
            _model.ImageName = value;
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

    private async Task LoadImageAsync()
    {
        IsImageLoading = true;
        ImageFailed = false;

        (RequestResult result, (byte[] bytes, Bitmap bitmap)? image) = await _backend.Products.Image(Image);

        if (result == RequestResult.Success) Thumbnail = image!.Value.bitmap;
        else ImageFailed = true;
        IsImageLoading = false;
    }
}