using Microsoft.Extensions.DependencyInjection;

namespace FrontendAdmin.ViewModels.Reservation;

public class ReservationPageViewModel : PageViewModelBase
{
    public ReservationPageViewModel(ServiceProvider services) : base(services)
    {
    }
}