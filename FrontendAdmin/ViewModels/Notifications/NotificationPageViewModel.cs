using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using FrontendAdmin.Models;
using FrontendAdmin.Services;
using FrontendAdmin.ViewModels.Components;

namespace FrontendAdmin.ViewModels.Notifications;

public class NotificationPageViewModel : PageViewModelBase
{
    private readonly IApiService _api;
    private readonly INavigationService _navigation;

    public NotificationPageViewModel(HeaderViewModel header, FooterViewModel footer, IApiService api,
        INavigationService navigationService) : base(header, footer)
    {
        _api = api;
        _navigation = navigationService;
        Notifications = [];
    }

    public ObservableCollection<NotificationViewModel> Notifications { get; }

    public override async Task LoadAsync()
    {
        (RequestResult result, List<NotificationModel> notifications)
            = await _api.Notifications.Page();

        if (result != RequestResult.Success)
            return;

        Notifications.Clear();

        foreach (NotificationModel notification in notifications)
            Notifications.Add(new NotificationViewModel(_navigation, notification));
    }
}