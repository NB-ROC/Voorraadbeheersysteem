using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using FrontendAdmin.Grpc;
using FrontendAdmin.Services;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Protos.Product;
using ReactiveUI;

namespace FrontendAdmin.ViewModels.Product;

public class ProductFormViewModel : ViewModelBase
{
    public ProductFormViewModel(ServiceProvider services, ProductViewModel? existing = null) : base(services)
    {
        this.WhenAnyValue(
            x => x.Name,
            x => x.Category,
            x => x.Description,
            x => x.Amount,
            x => x.ImageBytes
        ).Subscribe(_ => Validate());

        GetImageCommand = ReactiveCommand.CreateFromTask(OpenImageFileAsync);
        SaveCommand =
            ReactiveCommand.CreateFromTask(SaveProductAsync, this.WhenAnyValue(x => x.Error, string.IsNullOrEmpty));

        if (existing != null) LoadExistingProduct(existing);

        return;

        async Task SaveProductAsync()
        {
            bool success;
            if (existing != null)
                success = (await Client.Products.ModifyAsync(new ProductModifyRequest
                {
                    Id = existing.Id,
                    Name = Name,
                    Category = Category,
                    Description = Description,
                    Amount = Amount,
                    Image = ByteString.CopyFrom(ImageBytes!)
                })).Success;
            else
                success = (await Client.Products.CreateAsync(new ProductCreateRequest
                {
                    Name = Name,
                    Category = Category,
                    Description = Description,
                    Amount = Amount,
                    Image = ByteString.CopyFrom(ImageBytes!)
                })).Success;

            if (success) Services.GetService<NavigationService>()?.NavigateTo(new ProductPageViewModel(Services));
        }
    }

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

    public string Category
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    public string Description
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    public int Amount
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public string Error
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    public ICommand SaveCommand { get; }
    public ICommand GetImageCommand { get; }

    #endregion

    #region Private Methods

    private void Validate()
    {
        Error = string.IsNullOrWhiteSpace(Name)
            ? "Naam is verplicht."
            : string.IsNullOrWhiteSpace(Category)
                ? "Categorie is verplicht."
                : string.IsNullOrWhiteSpace(Description)
                    ? "Beschrijving is verplicht."
                    : Amount <= 0
                        ? "Aantal moet groter zijn dan 0."
                        : ImageBytes == null
                            ? "Selecteer een afbeelding."
                            : string.Empty;
    }

    private void LoadExistingProduct(ProductViewModel existing)
    {
        Name = existing.Name;
        Category = existing.Category;
        Description = existing.Description;
        Amount = existing.Amount;

        _ = LoadExistingImageAsync(existing.Image);
    }

    private async Task LoadExistingImageAsync(string imageName)
    {
        using MemoryStream ms = new();
        using AsyncServerStreamingCall<ProductImageResponse>? call = Client.Products.Image(new ProductImageRequest
            { Name = imageName });

        await foreach (ProductImageResponse chunk in call.ResponseStream.ReadAllAsync())
            await ms.WriteAsync(chunk.Raw.ToByteArray());

        ms.Seek(0, SeekOrigin.Begin);
        ImageBytes = ms.ToArray();
        PreviewImage = new Bitmap(ms);
    }

    public async Task OpenImageFileAsync()
    {
        TopLevel? topLevel =
            TopLevel.GetTopLevel(
                Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
                    ? desktop.MainWindow
                    : null);
        if (topLevel == null) return;
        IReadOnlyList<IStorageFile> files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Selecteer afbeelding", AllowMultiple = false,
            FileTypeFilter = [new FilePickerFileType("Images") { Patterns = ["*.jpg", "*.jpeg", "*.png"] }]
        });
        if (files.Count == 0) return;
        IStorageFile file = files[0];
        await using Stream stream = await file.OpenReadAsync();
        MemoryStream ms = new();
        await stream.CopyToAsync(ms);
        ImageBytes = ms.ToArray();
        ms.Seek(0, SeekOrigin.Begin);
        PreviewImage = new Bitmap(ms);
        ms.Dispose();
    }

    #endregion
}