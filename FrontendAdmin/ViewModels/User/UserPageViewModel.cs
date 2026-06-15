using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using FrontendAdmin.Models;
using FrontendAdmin.Services;
using FrontendAdmin.ViewModels.Components;
using FrontendAdmin.ViewModels.Notifications;
using ReactiveUI;

namespace FrontendAdmin.ViewModels.User;

public class UserPageViewModel : PageViewModelBase
{
    private readonly IApiService _api;
    private readonly INavigationService _navigation;

    public UserPageViewModel(IApiService api, INavigationService navigation, HeaderViewModel header,
        FooterViewModel footer) : base(header, footer)
    {
        _api = api;
        _navigation = navigation;

        NavigateUserForm =
            ReactiveCommand.CreateFromTask(async () => await _navigation.NavigateTo<UserFormViewModel, UserModel>());

        NavigateNotifications = ReactiveCommand.CreateFromTask(_navigation.NavigateTo<NotificationPageViewModel>);
    }

    public ObservableCollection<UserViewModel> Users { get; } = [];

    public ReactiveCommand<Unit, Unit> NavigateUserForm { get; }

    public ReactiveCommand<Unit, Unit> NavigateNotifications { get; }

    public int NotificationCount
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public override async Task LoadAsync()
    {
        await LoadUsersAsync();
        await LoadNotificationsAsync();
    }

    private async Task LoadUsersAsync()
    {
        (RequestResult result, List<UserModel> users) =
            await _api.Users.Page(1, 20);

        Users.Clear();

        foreach (UserModel user in users)
            Users.Add(new UserViewModel(user, EditUser, DeleteUser));
    }

    private async Task EditUser(UserModel user)
    {
        await _navigation.NavigateTo<UserFormViewModel, UserModel>(user);
    }

    private async Task DeleteUser(UserViewModel user)
    {
        (RequestResult result, bool success) = await _api.Users.Delete(user.Id);

        if (result == RequestResult.Success && success) Users.Remove(user);
    }

    private async Task LoadNotificationsAsync()
    {
        (RequestResult result, List<NotificationModel> notifications) =
            await _api.Notifications.Page();

        if (result != RequestResult.Success)
            return;

        NotificationCount = notifications.Count;
    }
}