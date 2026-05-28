using Microsoft.Extensions.DependencyInjection;

namespace Frontend.ViewModels.Dashboard;

public class DashboardPageViewModel : PageViewModelBase
{
    public DashboardPageViewModel(ServiceProvider services) : base(services)
    {
    }
}