using FrontendAdmin.Services;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace FrontendAdmin.ViewModels.Dashboard;

public class DashboardPageViewModel : ViewModelBase
{
    public DashboardPageViewModel(ServiceProvider services) : base(services)
    {
    }
}