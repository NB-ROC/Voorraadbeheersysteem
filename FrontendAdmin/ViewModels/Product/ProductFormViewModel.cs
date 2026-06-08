using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using DynamicData;
using DynamicData.Binding;
using FrontendAdmin.Models;
using FrontendAdmin.Services;
using FrontendAdmin.ViewModels.Components;
using ReactiveUI;

namespace FrontendAdmin.ViewModels.Product;

public class ProductFormViewModel : FormViewModelBase<ProductModel>, IDataErrorInfo
{
    private readonly IApiService _api;
    private readonly INavigationService _navigation;
    private CompositeDisposable _disposables = new();

    public ProductFormViewModel(IApiService api, INavigationService navigation, HeaderViewModel header,
        FooterViewModel footer)
        : base(header, footer)
    {
        _api = api;
        _navigation = navigation;


        this.WhenAnyValue(x => x.CategoryModel)
            .Subscribe(_ => this.RaisePropertyChanged(nameof(IsCustomCategory)));
        this.WhenAnyValue(x => x.ImageBytes)
            .Subscribe(_ => ImageError = ImageBytes == null
                ? "Selecteer een afbeelding."
                : string.Empty);

        IObservable<bool> canSave = this.WhenAnyValue(
                x => x.Name,
                x => x.CategoryModel,
                x => x.CustomCategory,
                x => x.Description,
                x => x.ImageBytes,
                (name, category, custom, desc, image) => true)
            .CombineLatest(
                Roles.ToObservableChangeSet()
                    .AutoRefresh(r => r.IsSelected)
                    .ToCollection()
                    .Select(roles => roles.Any(r => r.IsSelected)),
                (_, anyRole) => anyRole)
            .Select(_ => IsFormValid);

        SaveCommand = ReactiveCommand.CreateFromTask(SaveProductAsync, canSave);
        GetImageCommand = ReactiveCommand.CreateFromTask(OpenImageFileAsync);
    }

    #region Save

    private async Task SaveProductAsync()
    {
        if (CategoryModel == null) return;

        ProductModel model = new()
        {
            Id = _id ?? -1,
            Name = Name,
            CategoryModel = CategoryModel.Id == NewCategoryOption.Id
                ? new CategoryModel { Id = -1, Name = CustomCategory }
                : CategoryModel,
            Roles = SelectedRoleIds.Select(id => new RoleModel { Id = id }).ToList(),
            Description = Description
        };


        (RequestResult result, bool success) =
            await (_id == null
                ? _api.Products.Create(model, ImageBytes)
                : _api.Products.Modify(model, ImageBytes));

        if (result == RequestResult.Success && success)
            await _navigation.NavigateTo<ProductPageViewModel>();
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

    private int? _id;

    private byte[]? ImageBytes
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    [Required(ErrorMessage = "Naam is verplicht.")]
    public string Name
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    [Required(ErrorMessage = "Je moet een categorie kiezen")]
    public CategoryModel? CategoryModel
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = null!;

    [Required(ErrorMessage = "Beschrijving is verplicht.")]
    public string Description
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    [Required(ErrorMessage = "Naam voor de nieuwe categorie is verplicht")]
    public string CustomCategory
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    private int[] SelectedRoleIds =>
        Roles
            .Where(x => x.IsSelected)
            .Select(x => x.Id)
            .ToArray();

    #endregion

    #region UI

    private static readonly CategoryModel NewCategoryOption = new() { Id = -1, Name = "＋ Nieuwe categorie..." };

    public Bitmap? PreviewImage
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public ObservableCollection<CategoryModel> Categories { get; } = [];

    public ObservableCollection<RoleSelectionViewModel> Roles { get; } = [];

    public Thickness RolesErrorBorderThickness => string.IsNullOrWhiteSpace(RolesError)
        ? new Thickness(0)
        : new Thickness(1);

    public Thickness RolesErrorBorderMargin => string.IsNullOrWhiteSpace(RolesError)
        ? new Thickness(1)
        : new Thickness(0);

    public Thickness ImageErrorBorderThickness => string.IsNullOrWhiteSpace(ImageError)
        ? new Thickness(0)
        : new Thickness(1);

    public Thickness ImageErrorBorderMargin => string.IsNullOrWhiteSpace(ImageError)
        ? new Thickness(1)
        : new Thickness(0);

    public ICommand SaveCommand { get; }
    public ICommand GetImageCommand { get; }

    public bool IsCustomCategory =>
        CategoryModel?.Id == NewCategoryOption.Id;

    #endregion

    #region Validation

    public string Error
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    public string RolesError
    {
        get;
        private set
        {
            this.RaiseAndSetIfChanged(ref field, value);
            this.RaisePropertyChanged(nameof(RolesErrorBorderThickness));
            this.RaisePropertyChanged(nameof(RolesErrorBorderMargin));
        }
    } = string.Empty;

    public string ImageError
    {
        get;
        set
        {
            this.RaiseAndSetIfChanged(ref field, value);
            this.RaisePropertyChanged(nameof(ImageErrorBorderThickness));
            this.RaisePropertyChanged(nameof(ImageErrorBorderMargin));
        }
    } = string.Empty;

    public string this[string columnName]
    {
        get
        {
            ValidationContext context = new(this) { MemberName = columnName };
            List<ValidationResult> results = [];
            object? value = GetType().GetProperty(columnName)?.GetValue(this);

            if (!Validator.TryValidateProperty(value, context, results))
            {
                string? message = results.First().ErrorMessage;
                if (!string.IsNullOrEmpty(message)) return message;
            }

            return string.Empty;
        }
    }

    private bool IsFormValid =>
        !string.IsNullOrWhiteSpace(Name) &&
        CategoryModel != null &&
        (!IsCustomCategory || !string.IsNullOrWhiteSpace(CustomCategory)) &&
        !string.IsNullOrWhiteSpace(Description) &&
        ImageBytes is { Length: > 0 } &&
        Roles.Any(x => x.IsSelected);

    #endregion

    #region Loading

    public override async Task LoadAsync(ProductModel? existing)
    {
        await LoadCategoriesAsync();
        await LoadRolesAsync();

        if (existing != null)
            await LoadExistingProduct(existing);
        else
            ResetFields();

        LoadSubscriptions();

        CustomCategory = string.Empty;
    }

    private async Task LoadCategoriesAsync()
    {
        (RequestResult categoryResult, List<CategoryModel> categories) =
            await _api.Products.Category();

        if (categoryResult == RequestResult.Success)
        {
            Categories.Clear();

            foreach (CategoryModel category in categories)
                Categories.Add(category);
        }

        Categories.Add(NewCategoryOption);
    }

    private async Task LoadRolesAsync()
    {
        Roles.Clear();

        Roles.Add(new RoleSelectionViewModel(3, "Student", false));
        Roles.Add(new RoleSelectionViewModel(4, "Personnel", false));
        Roles.Add(new RoleSelectionViewModel(5, "Guest", false));
    }

    private async Task LoadExistingProduct(ProductModel existing)
    {
        _id = existing.Id;
        Name = existing.Name;

        CategoryModel = Categories.FirstOrDefault(c => c.Id == existing.CategoryModel.Id)
                        ?? NewCategoryOption;

        Description = existing.Description;

        foreach (RoleSelectionViewModel role in Roles)
            role.IsSelected = existing.Roles.Select(r => r.Id).Contains(role.Id);

        if (!string.IsNullOrWhiteSpace(existing.ImageName))
            await LoadExistingImageAsync(existing.ImageName);
    }

    private async Task LoadExistingImageAsync(string imageName)
    {
        (RequestResult result, (byte[] bytes, Bitmap bitmap)? image) =
            await _api.Products.Image(imageName);

        if (result != RequestResult.Success || image == null)
            return;

        ImageBytes = image.Value.bytes;
        PreviewImage = image.Value.bitmap;
    }

    private void LoadSubscriptions()
    {
        _disposables.Dispose();
        _disposables = new CompositeDisposable();

        foreach (RoleSelectionViewModel role in Roles)
            role.WhenAnyValue(x => x.IsSelected)
                .Subscribe(_ => RolesError = Roles.Any(x => x.IsSelected)
                    ? string.Empty
                    : "Minstens één rol is verplicht.")
                .DisposeWith(_disposables);
    }

    private void ResetFields()
    {
        _id = null;
        Name = string.Empty;
        Description = string.Empty;
        Error = string.Empty;
        ImageBytes = null;
        PreviewImage = null;
        CustomCategory = string.Empty;
    }

    #endregion
}