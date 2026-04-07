using FrontendAdmin.Services;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace FrontendAdmin.ViewModels.Loan;

public class LoanPageViewModel : ViewModelBase
{
    public LoanPageViewModel(ServiceProvider services) : base(services)
    {
    }
}