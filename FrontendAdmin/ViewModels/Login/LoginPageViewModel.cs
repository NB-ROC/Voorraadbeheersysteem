using System;
using System.Reactive;
using System.Threading.Tasks;
using FrontendAdmin.Services;
using FrontendAdmin.ViewModels.Dashboard;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace FrontendAdmin.ViewModels.Login;

public class LoginPageViewModel : PageViewModelBase
{
    public LoginPageViewModel(ServiceProvider services) : base(services)
    {
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
    } = "Placeholder1";

    public string Email
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "testmail@roc-nijmegen.nl";

    public string ErrorMessage
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    public ReactiveCommand<Unit, Unit> LoginCommand { get; }

    private async Task Login()
    {
        bool success = await Services.GetService<BackendService>()!.LogIn(Email, Password);

        if (success) Services.GetService<NavigationService>()?.NavigateTo(new DashboardPageViewModel(Services));
    }
}