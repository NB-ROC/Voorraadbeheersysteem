using Avalonia.Media;
using Frontend.Models;
using Frontend.Services;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace Frontend.ViewModels.Components;

public class HeaderViewModel : ViewModelBase
{
    public HeaderViewModel(ServiceProvider services) : base(services)
    {
        SmartCardService smartCardService = services.GetRequiredService<SmartCardService>();

        StatusColor = smartCardService.HasAvailableReader
            ? Brushes.Chartreuse
            : Brushes.Orange;
        NfcText = smartCardService.HasAvailableReader
            ? "Er zijn scanners aanwezig!"
            : "Er zijn geen scanners aanwezig :(";

        smartCardService.ReadersAvailableChanged += OnReadersAvailableChanged;


        UserModel? loggedInUser = services.GetRequiredService<ApiService>().LoggedInUser;

        Greet = loggedInUser == null
            ? ""
            : "Welkom, " + loggedInUser.FirstName + " " + loggedInUser.LastName;
    }

    public string Greet
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public IBrush StatusColor
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public string NfcText
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    private void OnReadersAvailableChanged(bool available)
    {
        StatusColor = available
            ? Brushes.Chartreuse
            : Brushes.Orange;
        NfcText = available
            ? "Er zijn scanners aanwezig!"
            : "Er zijn geen scanners aanwezig :(";
    }
}