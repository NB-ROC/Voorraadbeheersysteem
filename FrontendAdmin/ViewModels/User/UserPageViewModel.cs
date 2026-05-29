using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using FrontendAdmin.Models;
using FrontendAdmin.Services;
using FrontendAdmin.ViewModels.Notifications;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace FrontendAdmin.ViewModels.User;

public class UserPageViewModel : PageViewModelBase
{
    private readonly BackendService _backend;
    private int _notificationCount;

    public UserPageViewModel(ServiceProvider services) : base(services)
    {
        _backend = services.GetService<BackendService>() ??
                   throw new NullReferenceException("Backend service not initialised");

        NavigateUserForm = ReactiveCommand.Create(() =>
        {
            Services.GetService<NavigationService>()?
                .NavigateTo(new UserFormViewModel(Services));
        });

        NavigateNotifications = ReactiveCommand.Create(() =>
        {
            Services.GetService<NavigationService>()?
                .NavigateTo(new NotificationPageViewModel(Services));
        });
        _ = LoadNotificationsAsync();
        _ = LoadUsersAsync();
    }

    public ObservableCollection<UserViewModel> Users { get; } = [];

    public ReactiveCommand<Unit, Unit> NavigateUserForm { get; }

    public ReactiveCommand<Unit, Unit> NavigateNotifications { get; }

    public int NotificationCount
    {
        get => _notificationCount;
        set => this.RaiseAndSetIfChanged(ref _notificationCount, value);
    }

    public async Task LoadUsersAsync()
    {
        (RequestResult result, List<UserModel> users) =
            await _backend.Users.Page(1, 20);

        Users.Clear();

        foreach (UserModel user in users)
            Users.Add(new UserViewModel(Services, user));
    }
    private async Task LoadNotificationsAsync()
    {
        (RequestResult result, List<NotificationModel> notifications) =
            await _backend.Notifications.Page();

        if (result != RequestResult.Success)
            return;

        NotificationCount = notifications.Count;
    }
}