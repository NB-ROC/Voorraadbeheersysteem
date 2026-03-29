using System;
using System.Collections.Generic;
using System.Windows.Input;
using ReactiveUI;

namespace FrontendAdmin.Services;

public enum Page
{
    Dashboard,
    Products,
    Loans,
    Profile,
    Reservation,
    ProductForm,
}

public interface INavigationService
{
    object CurrentPage { get; }
    void NavigateTo(Page page);
    public ICommand NavigateCommand { get; }
}

public class NavigationService : ReactiveObject, INavigationService
{
    private readonly Dictionary<Page, Func<object>> _pages;

    public NavigationService(Dictionary<Page, Func<object>> pages)
    {
        _pages = pages;
        NavigateCommand = ReactiveCommand.Create<Page>(NavigateTo);
        CurrentPage = _pages[Page.Dashboard]();
    }

    public object CurrentPage
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public void NavigateTo(Page page) => CurrentPage = _pages[page]();
    public ICommand NavigateCommand { get; }
}