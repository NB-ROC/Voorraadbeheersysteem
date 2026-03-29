using FrontendAdmin.Models;
using ReactiveUI;

namespace FrontendAdmin.ViewModels.Product;

public class ProductViewModel : ReactiveObject
{
    private readonly ProductModel _model;

    public ProductViewModel(ProductModel model)
    {
        _model = model;
    }

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

    public string Image
    {
        get => _model.Image;
        set
        {
            if (_model.Image == value) return;
            _model.Image = value;
            this.RaisePropertyChanged();
        }
    }

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
}