using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using FrontendAdmin.Models;
using FrontendAdmin.Services;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace FrontendAdmin.ViewModels.User;

public class UserPageViewModel : PageViewModelBase
{
    private readonly BackendService _backend;

    public UserPageViewModel(ServiceProvider services) : base(services)
    {
        _backend = services.GetService<BackendService>() ??
                   throw new NullReferenceException("Backend service not initialised");
        NavigateUserForm = ReactiveCommand.Create(() =>
        {
            Services.GetService<NavigationService>()?.NavigateTo(new UserFormViewModel(Services));
        });

        _ = LoadUsersAsync();
    }


    public ObservableCollection<UserViewModel> Users { get; } = [];

    public ReactiveCommand<Unit, Unit> NavigateUserForm { get; }


    public async Task LoadUsersAsync()
    {
        (RequestResult result, List<UserModel> users) = await _backend.Users.Page(1, 20);

        Users.Clear();
        foreach (UserModel user in users)
            Users.Add(new UserViewModel(Services, user));
    }
}