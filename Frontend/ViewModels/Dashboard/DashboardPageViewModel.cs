using System.Threading.Tasks;
using Frontend.ViewModels.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Frontend.ViewModels.Dashboard;

public class DashboardPageViewModel : PageViewModelBase
{
    public DashboardPageViewModel(HeaderViewModel header, FooterViewModel footer) : base(header, footer)
    {
    }

    public override Task LoadAsync()
    {
        return Task.CompletedTask;
    }
}