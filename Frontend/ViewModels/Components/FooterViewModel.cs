using System;
using System.Reactive;
using Frontend.Services;
using Frontend.ViewModels.CustomerProduct;
using Frontend.ViewModels.Dashboard;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace Frontend.ViewModels.Components;

public class FooterViewModel : ViewModelBase
{
    public FooterViewModel(ServiceProvider services) : base(services)
    {
        NavigationService navigation = services.GetService<NavigationService>() ?? throw new NullReferenceException();

        NavigateDashboard = ReactiveCommand.Create(() => navigation.NavigateTo(new DashboardPageViewModel(services)));
        NavigateProducts = ReactiveCommand.Create(() => navigation.NavigateTo(new CustomerProductPageViewModel(services)));
        // NavigateLoans = ReactiveCommand.Create(() => navigation.NavigateTo(new LoanPageViewModel(services)));
        // NavigateUsers = ReactiveCommand.Create(() => navigation.NavigateTo(new UserPageViewModel(services)));
    }

    public ReactiveCommand<Unit, Unit> NavigateDashboard { get; }
    public ReactiveCommand<Unit, Unit> NavigateProducts { get; }
    public ReactiveCommand<Unit, Unit> NavigateLoans { get; }
    public ReactiveCommand<Unit, Unit> NavigateUsers { get; }
}