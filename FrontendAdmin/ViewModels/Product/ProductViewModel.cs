using System.ComponentModel;
using System.Runtime.CompilerServices;
using FrontendAdmin.Models;

namespace FrontendAdmin.ViewModels;

public class ProductViewModel : INotifyPropertyChanged
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
            if (_model.Name != value)
            {
                _model.Name = value;
                OnPropertyChanged();
            }
        }
    }

    public string Category
    {
        get => _model.Category;
        set
        {
            if (_model.Category != value)
            {
                _model.Category = value;
                OnPropertyChanged();
            }
        }
    }

    public string Description
    {
        get => _model.Description;
        set
        {
            if (_model.Description != value)
            {
                _model.Description = value;
                OnPropertyChanged();
            }
        }
    }

    public string Image
    {
        get => _model.Image;
        set
        {
            if (_model.Image != value)
            {
                _model.Image = value;
                OnPropertyChanged();
            }
        }
    }

    public int Amount
    {
        get => _model.Amount;
        set
        {
            if (_model.Amount != value)
            {
                _model.Amount = value;
                OnPropertyChanged();
            }
        }
    }

    // Example of UI-only property (very common)
    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            OnPropertyChanged();
        }
    }

    // Notify UI
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}