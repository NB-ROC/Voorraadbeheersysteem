using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Frontend.ViewModels;

namespace Frontend.Services;

public interface INavigationService
{
    public Task NavigateTo<TViewModel>() where TViewModel : PageViewModelBase;

    public Task NavigateTo<TViewModel, TModel>(TModel? model = null)
        where TViewModel : FormViewModelBase<TModel>
        where TModel : class;

    public Task NavigateTo<TViewModel, TSelectable>(Action<TSelectable?> callback, List<TSelectable>? data = null)
        where TViewModel : SelectionViewModelBase<TSelectable>
        where TSelectable : class;
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

    public async Task NavigateTo<TViewModel, TSelectable>(Action<TSelectable?> callback, List<TSelectable>? data = null) 
        where TViewModel : SelectionViewModelBase<TSelectable> 
        where TSelectable : class
    {
        SelectionViewModelBase<TSelectable> vm = _vmFactory(typeof(TViewModel)) as TViewModel ??
                                                 throw new NullReferenceException("Selector not found in service provider");

        ViewModelBase currentPage = _mainWindow.CurrentPage?? throw new NullReferenceException("Current page is not set");
        Action<TSelectable?> intermediaryCallback = selectable =>
        {
            callback(selectable);
            _mainWindow.CurrentPage = currentPage;
        };
        
        await vm.LoadAsync(intermediaryCallback, data);
        
        _mainWindow.CurrentPage = vm;
    }
}