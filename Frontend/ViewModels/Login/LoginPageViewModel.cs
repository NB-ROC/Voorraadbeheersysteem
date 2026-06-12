using System;
using System.Reactive;
using System.Threading.Tasks;
using Frontend.Services;
using Frontend.ViewModels.Dashboard;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace Frontend.ViewModels.Login;

public class LoginPageViewModel : PageViewModelBase
{
    private readonly string _email;

    public LoginPageViewModel(ServiceProvider services, string name, string email) : base(services)
    {
        IObservable<bool> canLogin = this.WhenAnyValue(
            x => x.Password,
            p => !string.IsNullOrWhiteSpace(p)
        );
        Name = name;
        _email = email;
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
    }

    public string ErrorMessage
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    public ReactiveCommand<Unit, Unit> LoginCommand { get; }

    private async Task Login()
    {
        bool success = await Services.GetService<ApiService>()!.LogIn(_email, Password);

        if (success) Services.GetService<NavigationService>()?.NavigateTo(new DashboardPageViewModel(Services));
    }
}