using Microsoft.Extensions.DependencyInjection;

namespace FrontendAdmin.ViewModels.Loan;

public class LoanPageViewModel : PageViewModelBase
{
    public LoanPageViewModel(ServiceProvider services) : base(services)
    {
    }
}