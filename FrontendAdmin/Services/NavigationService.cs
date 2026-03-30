using System;
using System.Collections.Generic;
using System.Windows.Input;
using Avalonia.Controls;
using ReactiveUI;

namespace FrontendAdmin.Services;

public enum Page
{
    Dashboard,
    Products,
    Loans,
    Users,
    Reservation,
    ProductForm,
    UserForm
}

public interface INavigationService
{
    object CurrentPage { get; }
    public ICommand NavigateCommand { get; }
    void NavigateTo(ReactiveObject page);
    void NavigateTo(Page page);
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

    public void NavigateTo(ReactiveObject page)
    {
        CurrentPage = page;
    }

    public void NavigateTo(Page page)
    {
        CurrentPage = _pages[page]();
    }

    public ICommand NavigateCommand { get; }
}