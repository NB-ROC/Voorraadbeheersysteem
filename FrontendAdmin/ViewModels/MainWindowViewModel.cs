using System.Windows.Input;
using Avalonia.Controls.Templates;
using FrontendAdmin.ViewModels.Dashboard;
using FrontendAdmin.ViewModels.Loan;
using FrontendAdmin.ViewModels.Product;
using FrontendAdmin.ViewModels.Profile;
using FrontendAdmin.ViewModels.Reservation;
using ReactiveUI;

namespace FrontendAdmin.ViewModels;

public class MainWindowViewModel : ReactiveObject
{
    private object _currentPage;
    public object CurrentPage
    {
        get => _currentPage;
        set => this.RaiseAndSetIfChanged(ref _currentPage, value);
    }

    private readonly DashboardPageViewModel _dashboard;
    private readonly ProductPageViewModel _products;
    private readonly LoanPageViewModel _loans;
    private readonly ProfilePageViewModel _profile;
    private readonly ReservationPageViewModel _reservering;

    public MainWindowViewModel(
        DashboardPageViewModel dashboard,
        ProductPageViewModel products,
        LoanPageViewModel loans,
        ProfilePageViewModel profile,
        ReservationPageViewModel reservering)
    {
        _dashboard = dashboard;
        _products = products;
        _loans = loans;
        _profile = profile;
        _reservering = reservering;

        // Commands
        NavigateToDashboard = ReactiveCommand.Create(() => NavigateTo(_dashboard));
        NavigateToProducts = ReactiveCommand.Create(() => NavigateTo(_products));
        NavigateToLoans = ReactiveCommand.Create(() => NavigateTo(_loans));
        NavigateToProfile = ReactiveCommand.Create(() => NavigateTo(_profile));
        NavigateToReservering = ReactiveCommand.Create(() => NavigateTo(_reservering));

        CurrentPage = _dashboard; // default
    }

    public ICommand NavigateToDashboard { get; }
    public ICommand NavigateToProducts { get; }
    public ICommand NavigateToLoans { get; }
    public ICommand NavigateToProfile { get; }
    public ICommand NavigateToReservering { get; }

    private void NavigateTo(object page) => CurrentPage = page;
}