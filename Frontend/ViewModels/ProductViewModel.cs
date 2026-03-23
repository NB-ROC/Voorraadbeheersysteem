using Backend.Models;
using Backend.Services;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Frontend.ViewModels;

public class ProductViewModel
{
    private readonly ProductService _service;

    public ObservableCollection<Product> Products { get; set; } = [];

    public Product SelectedProduct { get; set; } = new();

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public ICommand LoadCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand NextPageCommand { get; }
    public ICommand PrevPageCommand { get; }

    public ProductViewModel(ProductService service)
    {
        _service = service;

        LoadCommand = new RelayCommand(async _ => await Load());
        SaveCommand = new RelayCommand(async _ => await Save());
        DeleteCommand = new RelayCommand(async _ => await Delete());
        NextPageCommand = new RelayCommand(async _ =>
        {
            Page++;
            await Load();
        });

        PrevPageCommand = new RelayCommand(async _ =>
        {
            if (Page > 1)
                Page--;
            await Load();
        });

        _ = Load();

    }

    public async Task Load()
    {
        Products.Clear();

        var items = await _service.GetProducts(Page, PageSize);

        foreach (var item in items)
            Products.Add(item);
    }

    public async Task Save()
    {
        if (SelectedProduct.Id == 0)
            await _service.Create(SelectedProduct);
        else
            await _service.Update(SelectedProduct);

        SelectedProduct = new Product();

        await Load();
    }

    public async Task Delete()
    {
        await _service.Delete(SelectedProduct);
        await Load();
    }
}