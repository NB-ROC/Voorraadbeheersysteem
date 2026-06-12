using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using Frontend.Models;
using Frontend.Services;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace Frontend.ViewModels.User;

public class UserPageViewModel : PageViewModelBase
{
    private readonly ApiService _api;

    private int _notificationCount = 2;

    public UserPageViewModel(ServiceProvider services) : base(services)
    {
        _api = services.GetService<ApiService>() ??
                   throw new NullReferenceException("Backend service not initialised");

        NavigateUserForm = ReactiveCommand.Create(() =>
        {
            Services.GetService<NavigationService>()?
                .NavigateTo(new UserFormViewModel(Services));
        });

        _ = LoadUsersAsync();
    }

    public ObservableCollection<UserViewModel> Users { get; } = [];

    public ReactiveCommand<Unit, Unit> NavigateUserForm { get; }

    public int NotificationCount
    {
        get => _notificationCount;
        set => this.RaiseAndSetIfChanged(ref _notificationCount, value);
    }

    public async Task LoadUsersAsync()
    {
        (RequestResult result, List<UserModel> users) =
            await _api.Users.LenderPage(1, 20);

        Users.Clear();

        foreach (UserModel user in users)
            Users.Add(new UserViewModel(Services, user));
    }
}