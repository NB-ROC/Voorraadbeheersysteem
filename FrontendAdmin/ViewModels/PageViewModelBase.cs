using FrontendAdmin.Services;

namespace FrontendAdmin.ViewModels;

public class PageViewModelBase : ViewModelBase
{
    public PageViewModelBase(NavigationService navigationService)
    {
        NavigationService = navigationService;
    }

    protected NavigationService NavigationService { get; }
}