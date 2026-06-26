using System;
using System.Reactive;
using Frontend.Services;
using Frontend.ViewModels.Dashboard;
using Frontend.ViewModels.Loan;
using Frontend.ViewModels.User;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace Frontend.ViewModels.Components;

public class FooterViewModel : ViewModelBase
{
    public FooterViewModel(INavigationService navigation)
    {

        NavigateDashboard = ReactiveCommand.CreateFromTask(navigation.NavigateTo<DashboardPageViewModel>);
        NavigateLoans = ReactiveCommand.CreateFromTask(navigation.NavigateTo<LoanPageViewModel>);
        NavigateUsers = ReactiveCommand.CreateFromTask(navigation.NavigateTo<UserPageViewModel>);
    }

    public ReactiveCommand<Unit, Unit> NavigateDashboard { get; }
    public ReactiveCommand<Unit, Unit> NavigateLoans { get; }
    public ReactiveCommand<Unit, Unit> NavigateUsers { get; }
}