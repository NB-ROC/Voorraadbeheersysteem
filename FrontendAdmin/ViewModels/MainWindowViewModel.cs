using FrontendAdmin.Services;
using ReactiveUI;

namespace FrontendAdmin.ViewModels;

public class MainWindowViewModel : ReactiveObject
{
    public MainWindowViewModel(INavigationService navigation)
    {
        Navigation = navigation;
    }

    public INavigationService Navigation { get; }
}