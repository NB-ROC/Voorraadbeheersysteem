using ReactiveUI;
using FrontendAdmin.Services;

namespace FrontendAdmin.ViewModels;

public class MainWindowViewModel : ReactiveObject
{
    public INavigationService Navigation { get; }

    public MainWindowViewModel(INavigationService navigation)
    {
        Navigation = navigation;
    }
}