using Microsoft.Extensions.DependencyInjection;

namespace FrontendAdmin.ViewModels.Dashboard;

public class DashboardPageViewModel : ViewModelBase
{
    public DashboardPageViewModel(ServiceProvider services) : base(services)
    {
    }
}