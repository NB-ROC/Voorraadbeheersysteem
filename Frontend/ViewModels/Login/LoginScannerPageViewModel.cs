using System;
using System.Threading.Tasks;
using Frontend.Services;
using Frontend.ViewModels.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Frontend.ViewModels.Login;

public class LoginScannerPageViewModel : PageViewModelBase
{
    private readonly ISmartCardService _smartCard;
    private readonly IApiService _api;
    private readonly INavigationService _navigation;
    
    public LoginScannerPageViewModel(HeaderViewModel header, FooterViewModel footer, ISmartCardService smartCard, IApiService api, INavigationService navigation) : base(header, footer)
    {
        _smartCard = smartCard;
        _api = api;
        _navigation = navigation;
    }

    private void ScannerCallback(byte[] bytes)
    {

        (RequestResult result, (string email, string name)? tuple) = _api.Users.LenderScan(bytes).Result;

        if (result != RequestResult.Success || tuple == null)
        {
            _smartCard.SetCardDetectedCallback(ScannerCallback);
            return;
        }

        _navigation.NavigateTo<LoginPageViewModel, LoginInfo>(new LoginInfo(tuple.Value.name, tuple.Value.email));
    }

    public override Task LoadAsync()
    {
        _smartCard.SetCardDetectedCallback(ScannerCallback);
        return Task.CompletedTask;
    }
}