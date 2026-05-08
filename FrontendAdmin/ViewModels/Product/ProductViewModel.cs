using System;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using FrontendAdmin.Models;
using FrontendAdmin.Services;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace FrontendAdmin.ViewModels.Product;

public class ProductViewModel : ViewModelBase
{
    private readonly BackendService _backend;
    private readonly ProductModel _model;

    public ProductViewModel(ServiceProvider services, ProductModel model) : base(services)
    {
        _backend = services.GetService<BackendService>() ??
                   throw new NullReferenceException("Backend service not initialised");
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

    public RoleModel Role
    {
        get => _model.Role;
        set
        {
            if (_model.Role == value) return;
            _model.Role = value;
            this.RaisePropertyChanged();
        }
    }

    public string ImageName
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

    private async Task DeleteAsync()
    {
        await _backend.Products.Delete(Id);
    }

    private async Task LoadImageAsync()
    {
        IsImageLoading = true;
        ImageFailed = false;

        (RequestResult result, (byte[] bytes, Bitmap bitmap)? image) = await _backend.Products.Image(ImageName);

        if (result == RequestResult.Success) Thumbnail = image!.Value.bitmap;
        else ImageFailed = true;
        IsImageLoading = false;
    }
}