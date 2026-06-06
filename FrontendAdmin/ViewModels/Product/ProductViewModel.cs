using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using FrontendAdmin.Models;
using FrontendAdmin.Services;
using ReactiveUI;

namespace FrontendAdmin.ViewModels.Product;

public class ProductViewModel : ViewModelBase
{
    private readonly IApiService _api;
    private readonly ProductModel _model;

    public ProductViewModel(IApiService api, ProductModel model,
        Func<ProductModel, Task> editAction,
        Func<ProductViewModel, Task> deleteAction)
    {
        _api = api;
        _model = model;

        EditCommand = ReactiveCommand.CreateFromTask(() => editAction(_model));
        DeleteCommand = ReactiveCommand.CreateFromTask(() => deleteAction(this));
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

    public CategoryModel CategoryModel
    {
        get => _model.CategoryModel;
        set
        {
            if (_model.CategoryModel.Equals(value)) return;
            _model.CategoryModel = value;
            this.RaisePropertyChanged();
        }
    }

    public string CategoryName => CategoryModel.Name;

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

    public List<RoleModel> Roles
    {
        get => _model.Roles;
        set
        {
            if (_model.Roles == value) return;
            _model.Roles = value;
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

    public async Task LoadImageAsync()
    {
        IsImageLoading = true;
        ImageFailed = false;

        (RequestResult result, (byte[] bytes, Bitmap bitmap)? image) = await _api.Products.Image(ImageName);

        if (result == RequestResult.Success) Thumbnail = image!.Value.bitmap;
        else ImageFailed = true;
        IsImageLoading = false;
    }
}