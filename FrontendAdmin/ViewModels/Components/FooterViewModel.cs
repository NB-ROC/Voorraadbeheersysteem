using System;
using System.Reactive;
using FrontendAdmin.Services;
using FrontendAdmin.ViewModels.Dashboard;
using FrontendAdmin.ViewModels.Loan;
using FrontendAdmin.ViewModels.Product;
using FrontendAdmin.ViewModels.User;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace FrontendAdmin.ViewModels.Components;

public class FooterViewModel : ViewModelBase
{
    public FooterViewModel(INavigationService navigationService)
    {
        NavigateDashboard = ReactiveCommand.CreateFromTask(navigationService.NavigateTo<DashboardPageViewModel>);
        NavigateProducts = ReactiveCommand.CreateFromTask(navigationService.NavigateTo<ProductPageViewModel>);
        NavigateLoans = ReactiveCommand.CreateFromTask(navigationService.NavigateTo<LoanPageViewModel>);
        NavigateUsers = ReactiveCommand.CreateFromTask(navigationService.NavigateTo<UserPageViewModel>);
    }

    public ReactiveCommand<Unit, Unit> NavigateDashboard { get; }
    public ReactiveCommand<Unit, Unit> NavigateProducts { get; }
    public ReactiveCommand<Unit, Unit> NavigateLoans { get; }
    public ReactiveCommand<Unit, Unit> NavigateUsers { get; }
}