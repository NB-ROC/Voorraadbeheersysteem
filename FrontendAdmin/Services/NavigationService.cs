using System;
using System.Threading.Tasks;
using FrontendAdmin.ViewModels;

namespace FrontendAdmin.Services;

public interface INavigationService
{
    public Task NavigateTo<TViewModel>() where TViewModel : PageViewModelBase;

    public Task NavigateTo<TViewModel, TModel>(TModel? model = null)
        where TViewModel : FormViewModelBase<TModel>
        where TModel : class;
}

public class NavigationService : INavigationService
{
    private readonly MainWindowViewModel _mainWindow;
    private readonly Func<Type, ViewModelBase?> _vmFactory;

    public NavigationService(MainWindowViewModel mainWindow, Func<Type, ViewModelBase?> vmFactory)
    {
        _mainWindow = mainWindow;
        _vmFactory = vmFactory;
    }

    public async Task NavigateTo<TViewModel>()
        where TViewModel : PageViewModelBase
    {
        PageViewModelBase vm = _vmFactory(typeof(TViewModel)) as TViewModel ??
                               throw new NullReferenceException("Page not found in service provider");
        await vm.LoadAsync();

        _mainWindow.CurrentPage = vm;
    }

    public async Task NavigateTo<TViewModel, TModel>(TModel? model = null)
        where TViewModel : FormViewModelBase<TModel>
        where TModel : class
    {
        FormViewModelBase<TModel> vm = _vmFactory(typeof(TViewModel)) as TViewModel ??
                                       throw new NullReferenceException("Form not found in service provider");
        await vm.LoadAsync(model);

        _mainWindow.CurrentPage = vm;
    }
}