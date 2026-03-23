using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Google.Protobuf;
using Grpc.Net.Client;
using Protos.Product;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FrontendAdmin.ViewModels;

public partial class ProductViewModel : ObservableObject
{
    private readonly Products.ProductsClient _client;
    private readonly Window? _window;

    private int _page = 1;
    private const int PageSize = 10;

    private byte[]? _selectedImageBytes;

    [ObservableProperty]
    private ObservableCollection<ProductItemViewModel> products = new();

    [ObservableProperty]
    private ProductItemViewModel? selectedProduct;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string category = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private string amount = string.Empty;

    [ObservableProperty]
    private string imageFileName = string.Empty;

    [ObservableProperty]
    private string existingImageName = string.Empty;

    [ObservableProperty]
    private string statusMessage = string.Empty;

    public string PageInfo => $"Pagina {_page}";

    public ProductViewModel(Window? window = null)
    {
        _window = window;

        var channel = GrpcChannel.ForAddress("https://localhost:5001");
        _client = new Products.ProductsClient(channel);

        _ = LoadProducts();
    }

    partial void OnSelectedProductChanged(ProductItemViewModel? value)
    {
        if (value == null)
        {
            ClearForm();
            OnPropertyChanged(nameof(PageInfo));
            return;
        }

        Name = value.Name;
        Category = value.Category;
        Description = value.Description;
        Amount = value.Amount.ToString();
        ExistingImageName = value.Image;
        ImageFileName = string.Empty;
        _selectedImageBytes = null;
    }

    [RelayCommand]
    private async Task LoadProducts()
    {
        try
        {
            var response = await _client.PageAsync(new ProductPageRequest
            {
                Page = _page,
                PageSize = PageSize
            });

            Products.Clear();

            foreach (var product in response.Products)
            {
                Products.Add(new ProductItemViewModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Category = product.Category,
                    Description = product.Description,
                    Amount = product.Amount,
                    Image = product.Image
                });
            }

            StatusMessage = $"Producten geladen. Aantal: {Products.Count}";
            OnPropertyChanged(nameof(PageInfo));
        }
        catch (Exception ex)
        {
            StatusMessage = $"Fout bij laden van producten: {ex.Message}";
        }
    }

    [RelayCommand]
    private void New()
    {
        SelectedProduct = null;
        ClearForm();
        StatusMessage = "Nieuw product invoeren.";
    }

    [RelayCommand]
    private async Task PickImage()
    {
        try
        {
            if (_window?.StorageProvider == null)
            {
                StatusMessage = "Geen venster beschikbaar voor file picker.";
                return;
            }

            var files = await _window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Kies een productafbeelding",
                AllowMultiple = false
            });

            var file = files.FirstOrDefault();
            if (file == null)
            {
                StatusMessage = "Geen afbeelding geselecteerd.";
                return;
            }

            await using var stream = await file.OpenReadAsync();
            using var memory = new MemoryStream();
            await stream.CopyToAsync(memory);

            _selectedImageBytes = memory.ToArray();
            ImageFileName = file.Name;
            StatusMessage = $"Afbeelding gekozen: {file.Name}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Fout bij kiezen afbeelding: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task Save()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                StatusMessage = "Naam is verplicht.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Category))
            {
                StatusMessage = "Categorie is verplicht.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Description))
            {
                StatusMessage = "Beschrijving is verplicht.";
                return;
            }

            if (!int.TryParse(Amount, out var parsedAmount))
            {
                StatusMessage = "Aantal moet een geldig getal zijn.";
                return;
            }

            if (SelectedProduct == null)
            {
                if (_selectedImageBytes == null)
                {
                    StatusMessage = "Kies een afbeelding voor een nieuw product.";
                    return;
                }

                var createRequest = new ProductCreateRequest
                {
                    Name = Name,
                    Category = Category,
                    Description = Description,
                    Amount = parsedAmount,
                    Image = ByteString.CopyFrom(_selectedImageBytes)
                };

                var createResponse = await _client.CreateAsync(createRequest);

                StatusMessage = createResponse.Success
                    ? "Product succesvol toegevoegd."
                    : "Product toevoegen mislukt.";
            }
            else
            {
                var modifyRequest = new ProductModifyRequest
                {
                    Id = SelectedProduct.Id
                };

                modifyRequest.Name = Name;
                modifyRequest.Category = Category;
                modifyRequest.Description = Description;
                modifyRequest.Amount = parsedAmount;

                if (_selectedImageBytes != null)
                {
                    modifyRequest.Image = ByteString.CopyFrom(_selectedImageBytes);
                }

                var modifyResponse = await _client.ModifyAsync(modifyRequest);

                StatusMessage = modifyResponse.Success
                    ? "Product succesvol aangepast."
                    : "Product aanpassen mislukt.";
            }

            await LoadProducts();
            ClearForm();
            SelectedProduct = null;
        }
        catch (Exception ex)
        {
            StatusMessage = $"Fout bij opslaan: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task Delete()
    {
        try
        {
            if (SelectedProduct == null)
            {
                StatusMessage = "Selecteer eerst een product.";
                return;
            }

            var response = await _client.DeleteAsync(new ProductDeleteRequest
            {
                Id = SelectedProduct.Id
            });

            StatusMessage = response.Success
                ? "Product verwijderd."
                : "Verwijderen mislukt.";

            await LoadProducts();
            ClearForm();
            SelectedProduct = null;
        }
        catch (Exception ex)
        {
            StatusMessage = $"Fout bij verwijderen: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task PrevPage()
    {
        if (_page <= 1)
            return;

        _page--;
        await LoadProducts();
    }

    [RelayCommand]
    private async Task NextPage()
    {
        _page++;
        await LoadProducts();
    }

    private void ClearForm()
    {
        Name = string.Empty;
        Category = string.Empty;
        Description = string.Empty;
        Amount = string.Empty;
        ImageFileName = string.Empty;
        ExistingImageName = string.Empty;
        _selectedImageBytes = null;
    }
}