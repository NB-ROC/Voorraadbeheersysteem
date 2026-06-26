using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Frontend.Models;
using Frontend.Services;
using ReactiveUI;

namespace Frontend.ViewModels.Product;

public class ProductViewModel : ViewModelBase
{
    private readonly IApiService _api;
    private readonly ProductModel _model;

    public ProductViewModel(IApiService api, ProductModel model)
    {
        _model = model;
        _api = api;
    }

    public int Id => _model.Id;

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
        get => _model.CategoryModel;
        set
        {
            if (_model.CategoryModel == value) return;
            _model.CategoryModel = value;
            this.RaisePropertyChanged();
        }
    }

    public string CategoryName => _model.CategoryModel.Name;

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

    public async Task LoadImageAsync()
    {
        IsImageLoading = true;
        ImageFailed = false;

        (RequestResult result, (byte[] bytes, Bitmap bitmap)? image) = await _api.Products.Image(Image);

        if (result == RequestResult.Success) Thumbnail = image!.Value.bitmap;
        else ImageFailed = true;
        IsImageLoading = false;
    }
}