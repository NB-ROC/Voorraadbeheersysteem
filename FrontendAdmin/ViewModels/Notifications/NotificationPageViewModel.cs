using System.Collections.Generic;
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
        Load();
    }

    private async void Load()
    {
        BackendService? backend =
            Services.GetService<BackendService>();

        if (backend == null)
            return;

        (RequestResult result, List<NotificationModel> notifications)
            = await backend.Notifications.Page();

        if (result != RequestResult.Success)
            return;

        Notifications.Clear();

        foreach (NotificationModel notification in notifications)
        {
            Notifications.Add(new NotificationViewModel(
                Services,
                notification));
        }
    }
}