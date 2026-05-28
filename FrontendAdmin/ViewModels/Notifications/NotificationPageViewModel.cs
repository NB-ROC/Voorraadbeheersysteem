using System.Collections.ObjectModel;
using FrontendAdmin.Models;
using Microsoft.Extensions.DependencyInjection;

namespace FrontendAdmin.ViewModels.Notifications;

public class NotificationPageViewModel : PageViewModelBase
{
    public NotificationPageViewModel(ServiceProvider services)
        : base(services)
    {
        Notifications.Add(new NotificationViewModel(Services,
            new NotificationModel
            {
                Title = "Nieuwe registratie",
                Description = "Jan Jansen heeft zich geregistreerd."
            }));

        Notifications.Add(new NotificationViewModel(Services,
            new NotificationModel
            {
                Title = "Nieuwe registratie",
                Description = "gandalf the grey heeft zich geregistreerd."
            }));
    }

    public ObservableCollection<NotificationViewModel> Notifications { get; } = [];
}