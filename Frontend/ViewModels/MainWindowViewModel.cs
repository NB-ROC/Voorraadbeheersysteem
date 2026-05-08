using ReactiveUI;

namespace Frontend.ViewModels;

public partial class MainWindowViewModel : ReactiveObject
{    
    private string _role = "gast";
    public ViewModelBase? CurrentPage
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = null;


    public string Role
    {
        get => _role;
        set => this.RaiseAndSetIfChanged(ref _role, value);
    }
    public void Login()
    {
        Role = "Admin";
    }
}
