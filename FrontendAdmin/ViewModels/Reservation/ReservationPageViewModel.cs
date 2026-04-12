using FrontendAdmin.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FrontendAdmin.ViewModels.Reservation;

public class ReservationPageViewModel : ViewModelBase
{
    public ReservationPageViewModel(ServiceProvider services) : base(services)
    {
    }
}