// using System;
// using System.Collections.Generic;
// using System.Collections.ObjectModel;
// using System.IO;
// using System.Linq;
// using System.Threading.Tasks;
// using System.Windows.Input;
// using Avalonia;
// using Avalonia.Controls;
// using Avalonia.Controls.ApplicationLifetimes;
// using Avalonia.Media.Imaging;
// using Avalonia.Platform.Storage;
// using FrontendAdmin.Models;
// using FrontendAdmin.Services;
// using Microsoft.Extensions.DependencyInjection;
// using ReactiveUI;
//
// namespace FrontendAdmin.ViewModels.Product;
//
// public class ProductFormViewModel : ViewModelBase
// {
//     private readonly ApiService _api;
//     private readonly ProductViewModel? _existing;
//
//     public ProductFormViewModel(ServiceProvider services, ProductViewModel? existing = null)
//         : base(services)
//     {
//         _api = Services.GetService<ApiService>()
//                    ?? throw new NullReferenceException("Backend service not initialised");
//
//         _existing = existing;
//
//         this.WhenAnyValue(
//                 x => x.Name,
//                 x => x.CategoryModel,
//                 x => x.Description,
//                 x => x.ImageBytes,
//                 x => x.CustomCategory
//             )
//             .Subscribe(_ => Validate());
//         this.WhenAnyValue(x => x.CategoryModel)
//             .Subscribe(_ => this.RaisePropertyChanged(nameof(IsCustomCategory)));
//
//         GetImageCommand = ReactiveCommand.CreateFromTask(OpenImageFileAsync);
//
//         SaveCommand = ReactiveCommand.CreateFromTask(
//             SaveProductAsync,
//             this.WhenAnyValue(x => x.Error,
//                 error => string.IsNullOrWhiteSpace(error)));
//
//         _ = LoadLookupDataAsync(existing);
//         
//     }
//
//     #region Validation
//
//     private void Validate()
//     {
//         if (string.IsNullOrWhiteSpace(Name))
//         {
//             Error = "Naam is verplicht.";
//             return;
//         }
//
//         if (string.IsNullOrWhiteSpace(CategoryModel?.Name))
//         {
//             Error = "Categorie is verplicht.";
//             return;
//         }
//
//         if (string.IsNullOrWhiteSpace(Description))
//         {
//             Error = "Beschrijving is verplicht.";
//             return;
//         }
//
//         if (ImageBytes == null || ImageBytes.Length == 0)
//         {
//             Error = "Selecteer een afbeelding.";
//             return;
//         }
//
//         if (IsCustomCategory && string.IsNullOrWhiteSpace(CustomCategory))
//         {
//             Error = "Voer een naam in voor de nieuwe categorie.";
//             return;
//         }
//
//         if (!Roles.Any(x => x.IsSelected))
//         {
//             Error = "Minstens één rol is verplicht.";
//             return;
//         }
//
//         Error = string.Empty;
//     }
//
//     #endregion
//
//     #region Save
//
//     private async Task SaveProductAsync()
//     {
//         ProductModel model = new()
//         {
//             Id = Id,
//             Name = Name,
//             Category = CategoryModel?.Id == NewCategoryOption.Id
//                 ? new CategoryModel { Id = -1, Name = CustomCategory }
//                 : CategoryModel!,
//             Roles = SelectedRoleIds.Select(id => new RoleModel { Id = id }).ToList(),
//             Description = Description
//         };
//         
//         
//
//         (RequestResult result, bool success) =
//             await (_existing == null
//                 ? _api.Products.Create(model, ImageBytes)
//                 : _api.Products.Modify(model, ImageBytes));
//
//         if (result == RequestResult.Success && success)
//             Services.GetService<NavigationService>()
//                 ?.NavigateTo(new ProductPageViewModel(Services));
//     }
//
//     #endregion
//
//     #region Image Picker
//
//     private async Task OpenImageFileAsync()
//     {
//         TopLevel? topLevel =
//             TopLevel.GetTopLevel(
//                 Application.Current?.ApplicationLifetime
//                     is IClassicDesktopStyleApplicationLifetime desktop
//                     ? desktop.MainWindow
//                     : null);
//
//         if (topLevel == null)
//             return;
//
//         IReadOnlyList<IStorageFile> files =
//             await topLevel.StorageProvider.OpenFilePickerAsync(
//                 new FilePickerOpenOptions
//                 {
//                     Title = "Selecteer afbeelding",
//                     AllowMultiple = false,
//                     FileTypeFilter =
//                     [
//                         new FilePickerFileType("Images")
//                         {
//                             Patterns = ["*.jpg", "*.jpeg", "*.png"]
//                         }
//                     ]
//                 });
//
//         if (files.Count == 0)
//             return;
//
//         IStorageFile file = files[0];
//
//         await using Stream stream = await file.OpenReadAsync();
//
//         using MemoryStream ms = new();
//
//         await stream.CopyToAsync(ms);
//
//         ImageBytes = ms.ToArray();
//
//         ms.Position = 0;
//
//         PreviewImage = new Bitmap(ms);
//     }
//
//     #endregion
//
//     #region Properties
//
//     public int Id { get; set; }
//
//     public Bitmap? PreviewImage
//     {
//         get;
//         set => this.RaiseAndSetIfChanged(ref field, value);
//     }
//
//     public byte[]? ImageBytes
//     {
//         get;
//         set => this.RaiseAndSetIfChanged(ref field, value);
//     }
//
//     public string Name
//     {
//         get;
//         set => this.RaiseAndSetIfChanged(ref field, value);
//     } = string.Empty;
//
//     public CategoryModel CategoryModel
//     {
//         get;
//         set => this.RaiseAndSetIfChanged(ref field, value);
//     } = null!;
//
//     public string Description
//     {
//         get;
//         set => this.RaiseAndSetIfChanged(ref field, value);
//     } = string.Empty;
//
//     public string Error
//     {
//         get;
//         set => this.RaiseAndSetIfChanged(ref field, value);
//     } = string.Empty;
//
//     public string CustomCategory
//     {
//         get;
//         set => this.RaiseAndSetIfChanged(ref field, value);
//     } = string.Empty;
//     
//     public static readonly CategoryModel NewCategoryOption = new() { Id = -2, Name = "＋ Nieuwe categorie..." };
//
//     public ObservableCollection<CategoryModel> Categories { get; } = [];
//
//     public ObservableCollection<RoleSelectionViewModel> Roles { get; } = [];
//
//     public int[] SelectedRoleIds =>
//         Roles
//             .Where(x => x.IsSelected)
//             .Select(x => x.Id)
//             .ToArray();
//
//     public ICommand SaveCommand { get; }
//     public ICommand GetImageCommand { get; }
//
//     #endregion
//
//     #region Lookup Data
//
//     private async Task LoadLookupDataAsync(ProductViewModel? existing = null)
//     {
//         (RequestResult categoryResult, List<CategoryModel> categories) =
//             await _api.Products.Category();
//
//         if (categoryResult == RequestResult.Success)
//         {
//             Categories.Clear();
//
//             foreach (CategoryModel category in categories)
//                 Categories.Add(category);
//         }
//
//         Categories.Add(NewCategoryOption);
//
//         Roles.Add(new RoleSelectionViewModel(Services, 3, "Student", false));
//         Roles.Add(new RoleSelectionViewModel(Services, 4, "Personnel", false));
//         Roles.Add(new RoleSelectionViewModel(Services, 5, "Guest", false));
//
//         if (existing != null)
//         {
//             Console.WriteLine(string.Join(", ", existing.Roles.Select(r => r.Name)));
//             LoadExistingProduct(existing);
//         }
//     }
//     
//     public bool IsCustomCategory =>
//         CategoryModel?.Id == NewCategoryOption.Id;
//
//     #endregion
//
//     #region Product Loading
//
//     private void LoadExistingProduct(ProductViewModel existing)
//     {
//         Id = existing.Id;
//         Name = existing.Name;
//
//         CategoryModel = Categories.FirstOrDefault(c => c.Id == existing.CategoryModel.Id)
//                         ?? NewCategoryOption;
//
//         Description = existing.Description;
//
//         if (!string.IsNullOrWhiteSpace(existing.ImageName))
//             _ = LoadExistingImageAsync(existing.ImageName);
//
//         foreach (RoleSelectionViewModel role in Roles)
//             role.IsSelected = existing.Roles.Select(r => r.Id).Contains(role.Id);
//     }
//
//     private async Task LoadExistingImageAsync(string imageName)
//     {
//         (RequestResult result, (byte[] bytes, Bitmap bitmap)? image) =
//             await _api.Products.Image(imageName);
//
//         if (result != RequestResult.Success || image == null)
//             return;
//
//         ImageBytes = image.Value.bytes;
//         PreviewImage = image.Value.bitmap;
//     }
//
//     #endregion
// }