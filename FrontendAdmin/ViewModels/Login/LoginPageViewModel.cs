using System;
using System.Reactive;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace FrontendAdmin.ViewModels.Login;

public class LoginPageViewModel : PageViewModelBase
{
    public LoginPageViewModel(ServiceProvider services) : base(services)
    {
        LoginCommand = ReactiveCommand.Create(() =>
        {
            Console.WriteLine("command executed");
        });
    }
    
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public ReactiveCommand<Unit, Unit> LoginCommand { get; }
        
    public string ErrorMessage { get; set; } = string.Empty;

    public void Login()
    {
            
    }

}