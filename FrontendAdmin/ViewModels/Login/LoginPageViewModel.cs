using System;
using System.Reactive;
using FrontendAdmin.Services;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace FrontendAdmin.ViewModels.Login;

public class LoginPageViewModel : PageViewModelBase
{
    private string _username = string.Empty;
    private string _password = string.Empty;
    private string _email = string.Empty;
    private string _errorMessage = string.Empty;

    public LoginPageViewModel(ServiceProvider services) : base(services)
    {
        var canLogin = this.WhenAnyValue(
            x => x.Username,
            x => x.Password,
            (u, p) => !string.IsNullOrWhiteSpace(u) && !string.IsNullOrWhiteSpace(p)
            );
        
        LoginCommand = ReactiveCommand.Create(Login, canLogin);
    }

    public string Username
    {
        get => _username;
        set => this.RaiseAndSetIfChanged(ref _username, value);
    }

    public string Password
    {
        get => _password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }

    public string Email
    {
        get => _email;
        set => this.RaiseAndSetIfChanged(ref _email, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
    }
    
    public ReactiveCommand<Unit, Unit> LoginCommand { get; }

    private void Login()
    {
        Console.WriteLine($"Login clicked: {Username}/ {Password}");
    }
    
}