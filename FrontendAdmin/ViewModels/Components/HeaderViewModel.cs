using Microsoft.Extensions.DependencyInjection;

namespace FrontendAdmin.ViewModels.Components;

public class HeaderViewModel : ViewModelBase
{
    public HeaderViewModel(ServiceProvider services) : base(services)
    {
    }
}