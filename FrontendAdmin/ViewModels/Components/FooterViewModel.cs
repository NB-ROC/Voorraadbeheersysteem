using System;
using System.Reactive;
using FrontendAdmin.Services;
using FrontendAdmin.ViewModels.AuditLog;
using FrontendAdmin.ViewModels.Dashboard;
using FrontendAdmin.ViewModels.Loan;
using FrontendAdmin.ViewModels.Product;
using FrontendAdmin.ViewModels.User;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace FrontendAdmin.ViewModels.Components;

public class FooterViewModel : ViewModelBase
{
    public FooterViewModel(ServiceProvider services) : base(services)
    {
        NavigationService navigation = services.GetService<NavigationService>() ?? throw new NullReferenceException();

        NavigateDashboard = ReactiveCommand.Create(() => navigation.NavigateTo(new DashboardPageViewModel(services)));
        NavigateProducts = ReactiveCommand.Create(() => navigation.NavigateTo(new ProductPageViewModel(services)));
        NavigateLoans = ReactiveCommand.Create(() => navigation.NavigateTo(new LoanPageViewModel(services)));
        NavigateUsers = ReactiveCommand.Create(() => navigation.NavigateTo(new UserPageViewModel(services)));
        NavigateAuditLog = ReactiveCommand.Create(() => navigation.NavigateTo(new AuditLogPageViewModel(services)));
    }

    public ReactiveCommand<Unit, Unit> NavigateDashboard { get; }
    public ReactiveCommand<Unit, Unit> NavigateProducts { get; }
    public ReactiveCommand<Unit, Unit> NavigateLoans { get; }
    public ReactiveCommand<Unit, Unit> NavigateUsers { get; }
    public ReactiveCommand<Unit, Unit> NavigateAuditLog { get; }
}