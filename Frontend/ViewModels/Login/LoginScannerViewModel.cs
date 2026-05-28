using Frontend.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Frontend.ViewModels.Login;

public class LoginScannerViewModel : PageViewModelBase
{
    public LoginScannerViewModel(ServiceProvider services) : base(services)
    {
        services.GetRequiredService<SmartCardService>().SetCardDetectedCallback(bytes =>
        {
            
        });
    }

    private void ScannerCallback(byte[] bytes)
    {
        
    }
}