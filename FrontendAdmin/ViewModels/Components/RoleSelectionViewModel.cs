using ReactiveUI;

namespace FrontendAdmin.ViewModels.Product;

public class RoleSelectionViewModel : ViewModelBase
{
    public RoleSelectionViewModel(int id, string name, bool isSelected)
    {
        Id = id;
        IsSelected = isSelected;
        Name = name;
    }

    public int Id { get; init; }

    public string Name { get; init; }

    public bool IsSelected
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
}