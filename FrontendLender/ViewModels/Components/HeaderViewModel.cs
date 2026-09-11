using Avalonia.Media;
using Frontend.Models;
using Frontend.Services;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace Frontend.ViewModels.Components;

public class HeaderViewModel : ViewModelBase
{
    public HeaderViewModel(ISmartCardService smartCard, IApiService api)
    {

        OnReadersAvailableChanged(smartCard.HasAvailableReader);

        smartCard.ReadersAvailableChanged += OnReadersAvailableChanged;


        UserModel? loggedInUser = api.LoggedInUser;

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
    } = Brushes.Red;

    public string NfcText
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

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