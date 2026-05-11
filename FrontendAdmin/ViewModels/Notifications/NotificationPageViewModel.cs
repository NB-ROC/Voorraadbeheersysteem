using System.Collections.ObjectModel;
using FrontendAdmin.Models;
using FrontendAdmin.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FrontendAdmin.ViewModels.Notifications;

public class NotificationPageViewModel : PageViewModelBase
{
    public ObservableCollection<NotificationViewModel> Notifications { get; } = [];
    public NotificationPageViewModel(ServiceProvider services)
        : base(services)
    {
        Notifications.Add(new NotificationViewModel(Services,
            new NotificationModel
            {
                Title = "Nieuwe registratie",
                Description = "Jan Jansen wacht op goedkeuring."
            }));

        Notifications.Add(new NotificationViewModel(Services,
            new NotificationModel
            {
                Title = "Nieuwe registratie",
                Description = "gandalf the grey wacht op goedkeuring."
            }));
    }
}