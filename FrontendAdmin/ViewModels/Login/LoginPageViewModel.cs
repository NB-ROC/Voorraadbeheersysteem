using System;
using System.Reactive;
using System.Threading.Tasks;
using FrontendAdmin.Services;
using FrontendAdmin.ViewModels.Components;
using FrontendAdmin.ViewModels.Dashboard;
using ReactiveUI;

namespace FrontendAdmin.ViewModels.Login;

public class LoginPageViewModel : PageViewModelBase
{
    private readonly IApiService _api;
    private readonly INavigationService _navigation;

    public LoginPageViewModel(HeaderViewModel header, FooterViewModel footer, IApiService api, INavigationService navigation) : base(header, footer)
    {
        _api = api;
        _navigation = navigation;
        IObservable<bool> canLogin = this.WhenAnyValue(
            x => x.Password,
            p => !string.IsNullOrWhiteSpace(p)
        );

        LoginCommand = ReactiveCommand.CreateFromTask(Login, canLogin);
    }

    public string Password
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    public string Email
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    public string ErrorMessage
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    public ReactiveCommand<Unit, Unit> LoginCommand { get; }

    private async Task Login()
    {
        bool success = await _api.LogIn(Email, Password);

        if (success) await _navigation.NavigateTo<DashboardPageViewModel>();
    }

    public override Task LoadAsync()
    {
        return Task.CompletedTask;
    }
}