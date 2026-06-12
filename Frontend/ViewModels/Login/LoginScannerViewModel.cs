using Frontend.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Frontend.ViewModels.Login;

public class LoginScannerViewModel : PageViewModelBase
{
    public LoginScannerViewModel(ServiceProvider services) : base(services)
    {
        services.GetRequiredService<SmartCardService>().SetCardDetectedCallback(ScannerCallback);
    }

    private void ScannerCallback(byte[] bytes)
    {
        (RequestResult result, (string name, string email)? tuple) =
            Services.GetRequiredService<ApiService>().Users.LenderScan(bytes).Result;

        if (result != RequestResult.Success || tuple == null)
        {
            Services.GetRequiredService<SmartCardService>().SetCardDetectedCallback(ScannerCallback);
            return;
        }

        Services.GetRequiredService<NavigationService>()
            .NavigateTo(new LoginPageViewModel(Services, tuple.Value.email, tuple.Value.name));
    }
}