using System;
using System.Reactive;
using System.Threading.Tasks;
using Frontend.Services;
using Frontend.ViewModels.Components;
using Frontend.ViewModels.Dashboard;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace Frontend.ViewModels.Login;

public record LoginInfo(string Name, string Email);

public class LoginPageViewModel : FormViewModelBase<LoginInfo>
{
    private readonly IApiService _api;
    private readonly INavigationService _navigation;
    
    private string _email = string.Empty;

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

    public string Name
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
        bool success = await _api.LogIn(_email, Password);

        if (success) await _navigation.NavigateTo<DashboardPageViewModel>();
    }

    public override async Task LoadAsync(LoginInfo? existing)
    {
        if (existing == null)
        {
            await _navigation.NavigateTo<LoginScannerPageViewModel>();
            return;
        }
        
        _email = existing.Email;
        Name = existing.Name;
    }
}