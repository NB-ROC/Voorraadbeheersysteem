using ReactiveUI;
using FrontendAdmin.ViewModels.Login;

namespace FrontendAdmin.ViewModels;

public class MainWindowViewModel : ReactiveObject
{
    public ViewModelBase? CurrentPage
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = null;
}