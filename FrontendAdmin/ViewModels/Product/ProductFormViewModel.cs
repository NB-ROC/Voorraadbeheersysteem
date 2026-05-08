using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using FrontendAdmin.Models;
using FrontendAdmin.Services;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace FrontendAdmin.ViewModels.Product;

public class ProductFormViewModel : ViewModelBase
{
    private readonly BackendService _backend;
    private readonly ProductViewModel? _existing;

    public ProductFormViewModel(ServiceProvider services, ProductViewModel? existing = null)
        : base(services)
    {
        _backend = Services.GetService<BackendService>()
                   ?? throw new NullReferenceException("Backend service not initialised");

        _existing = existing;

        this.WhenAnyValue(
                x => x.Name,
                x => x.CategoryModel,
                x => x.Description,
                x => x.ImageBytes
            )
            .Subscribe(_ => Validate());

        GetImageCommand = ReactiveCommand.CreateFromTask(OpenImageFileAsync);

        SaveCommand = ReactiveCommand.CreateFromTask(
            SaveProductAsync,
            this.WhenAnyValue(x => x.Error,
                error => string.IsNullOrWhiteSpace(error)));

        _ = LoadLookupDataAsync();

        if (existing != null)
            LoadExistingProduct(existing);
    }
    #region Validation

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            Console.WriteLine(Name);
            Error = "Naam is verplicht.";
            return;
        }

        if (string.IsNullOrWhiteSpace(CategoryModel?.Name))
        {
            Error = "Categorie is verplicht.";
            return;
        }

        if (string.IsNullOrWhiteSpace(Description))
        {
            Error = "Beschrijving is verplicht.";
            return;
        }

        if (ImageBytes == null || ImageBytes.Length == 0)
        {
            Error = "Selecteer een afbeelding.";
            return;
        }
        
        if ((CategoryModel == null || string.IsNullOrWhiteSpace(CategoryModel.Name)) &&
            string.IsNullOrWhiteSpace(CustomCategory))
        {
            Error = "Categorie is verplicht.";
            return;
        }

        Error = string.Empty;
    }

    #endregion

    #region Save

    private async Task SaveProductAsync()
    {
        ProductModel model = new()
        {
            Id = _existing?.Id ?? 0,
            Name = Name,
            CategoryModel = !string.IsNullOrWhiteSpace(CustomCategory)
                ? new CategoryModel
                {
                    Name = CustomCategory
                }
                : CategoryModel!,
            RoleModel = RoleModel,
            Description = Description
        };

        (RequestResult result, bool success) =
            await (_existing == null
                ? _backend.Products.Create(model, ImageBytes)
                : _backend.Products.Modify(model, ImageBytes));

        if (result == RequestResult.Success && success)
            Services.GetService<NavigationService>()
                ?.NavigateTo(new ProductPageViewModel(Services));
    }

    #endregion

    #region Image Picker

    private async Task OpenImageFileAsync()
    {
        TopLevel? topLevel =
            TopLevel.GetTopLevel(
                Application.Current?.ApplicationLifetime
                    is IClassicDesktopStyleApplicationLifetime desktop
                    ? desktop.MainWindow
                    : null);

        if (topLevel == null)
            return;

        IReadOnlyList<IStorageFile> files =
            await topLevel.StorageProvider.OpenFilePickerAsync(
                new FilePickerOpenOptions
                {
                    Title = "Selecteer afbeelding",
                    AllowMultiple = false,
                    FileTypeFilter =
                    [
                        new FilePickerFileType("Images")
                        {
                            Patterns = ["*.jpg", "*.jpeg", "*.png"]
                        }
                    ]
                });

        if (files.Count == 0)
            return;

        IStorageFile file = files[0];

        await using Stream stream = await file.OpenReadAsync();

        using MemoryStream ms = new();

        await stream.CopyToAsync(ms);

        ImageBytes = ms.ToArray();

        ms.Position = 0;

        PreviewImage = new Bitmap(ms);
    }

    #endregion

    #region Properties

    public Bitmap? PreviewImage
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public byte[]? ImageBytes
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public string Name
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    public CategoryModel CategoryModel
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = null!;

    public string Description
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    public RoleModel RoleModel
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = null!;

    public string Error
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;
    
    public string CustomCategory
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    public ObservableCollection<CategoryModel> Categories { get; } = [];
    public ObservableCollection<RoleModel> Roles { get; } = [];
    
    private async Task LoadLookupDataAsync()
    {
        (RequestResult categoryResult, List<CategoryModel> categories) =
            await _backend.Products.Category();

        if (categoryResult == RequestResult.Success)
        {
            Categories.Clear();

            foreach (CategoryModel category in categories)
                Categories.Add(category);
        }

        (RequestResult roleResult, List<RoleModel> roles) =
            await _backend.Products.Role();

        if (roleResult == RequestResult.Success)
        {
            Roles.Clear();

            foreach (RoleModel role in roles)
                Roles.Add(role);
        }
    }

    public ICommand SaveCommand { get; }
    public ICommand GetImageCommand { get; }

    #endregion

    #region Product Loading

    private void LoadExistingProduct(ProductViewModel existing)
    {
        Name = existing.Name;
        CategoryModel = existing.CategoryModel;
        Description = existing.Description;
        RoleModel = existing.RoleModel;

        if (!string.IsNullOrWhiteSpace(existing.ImageName))
            _ = LoadExistingImageAsync(existing.ImageName);
    }

    private async Task LoadExistingImageAsync(string imageName)
    {
        (RequestResult result, (byte[] bytes, Bitmap bitmap)? image) = await _backend.Products.Image(imageName);

        if (result != RequestResult.Success || image == null)
            return;

        ImageBytes = image.Value.bytes;

        PreviewImage = image.Value.bitmap;
    }

    #endregion
}