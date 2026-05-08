using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
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
                x => x.Category,
                x => x.Description,
                x => x.ImageBytes
            )
            .Subscribe(_ => Validate());

        GetImageCommand = ReactiveCommand.CreateFromTask(OpenImageFileAsync);

        SaveCommand = ReactiveCommand.CreateFromTask(
            SaveProductAsync,
            this.WhenAnyValue(x => x.Error,
                error => string.IsNullOrWhiteSpace(error)));

        if (existing != null)
            LoadExistingProduct(existing);
    }

    #region Properties

    private Bitmap? _previewImage;
    public Bitmap? PreviewImage
    {
        get => _previewImage;
        set => this.RaiseAndSetIfChanged(ref _previewImage, value);
    }

    private byte[]? _imageBytes;
    public byte[]? ImageBytes
    {
        get => _imageBytes;
        set => this.RaiseAndSetIfChanged(ref _imageBytes, value);
    }

    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    private string _category = string.Empty;
    public string Category
    {
        get => _category;
        set => this.RaiseAndSetIfChanged(ref _category, value);
    }

    private string _description = string.Empty;
    public string Description
    {
        get => _description;
        set => this.RaiseAndSetIfChanged(ref _description, value);
    }

    private string _error = string.Empty;
    public string Error
    {
        get => _error;
        set => this.RaiseAndSetIfChanged(ref _error, value);
    }

    public ICommand SaveCommand { get; }
    public ICommand GetImageCommand { get; }

    #endregion

    #region Validation

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            Error = "Naam is verplicht.";
            return;
        }

        if (string.IsNullOrWhiteSpace(Category))
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

        Error = string.Empty;
    }

    #endregion

    #region Product Loading

    private void LoadExistingProduct(ProductViewModel existing)
    {
        Name = existing.Name;
        Category = existing.Category;
        Description = existing.Description;

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

    #region Save

    private async Task SaveProductAsync()
    {
        ProductModel model = new()
        {
            Id = _existing?.Id ?? 0,
            Name = Name,
            Category = Category,
            Description = Description
        };

        (RequestResult result, bool success) =
            await (_existing == null
                ? _backend.Products.Create(model, ImageBytes)
                : _backend.Products.Modify(model, ImageBytes));

        if (result == RequestResult.Success && success)
        {
            Services.GetService<NavigationService>()
                ?.NavigateTo(new ProductPageViewModel(Services));
        }
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
}