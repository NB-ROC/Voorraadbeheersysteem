using System;
using System.Threading.Tasks;
using FrontendAdmin.ViewModels;

namespace FrontendAdmin.Services;

public interface INavigationService
{
    public Task NavigateTo<TViewModel>() where TViewModel : ViewModelBase;
}

public class NavigationService : INavigationService
{
    private readonly Func<Type, PageViewModelBase> _vmFactory;
    private readonly MainWindowViewModel _mainWindow;

    public NavigationService(MainWindowViewModel mainWindow, Func<Type, PageViewModelBase> vmFactory)
    {
        _mainWindow = mainWindow;
        _vmFactory = vmFactory;
    }

    public async Task NavigateTo<TViewModel>() where TViewModel : ViewModelBase
    {
        PageViewModelBase vm = _vmFactory(typeof(TViewModel));
        await vm.LoadAsync();
        
        _mainWindow.CurrentPage = vm;
    }
}