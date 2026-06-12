using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using Frontend.Models;
using Frontend.Services;
using Frontend.ViewModels.Components;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace Frontend.ViewModels.User;

public class UserPageViewModel : PageViewModelBase
{
    private readonly IApiService _api;
    private readonly INavigationService _navigation;

    public UserPageViewModel(HeaderViewModel header, FooterViewModel footer, IApiService api, INavigationService navigation) :  base(header, footer)
    {
        _api = api;
        _navigation = navigation;

        NavigateUserForm = ReactiveCommand.CreateFromTask(async () => await _navigation.NavigateTo<UserFormViewModel, UserModel>());

    }

    public ObservableCollection<UserViewModel> Users { get; } = [];

    public ReactiveCommand<Unit, Unit> NavigateUserForm { get; }

    public int NotificationCount
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = 2;

    public override async Task LoadAsync()
    {
        await LoadUsersAsync();
    }

    private async Task EditAction(UserModel user)
    {
        await _navigation.NavigateTo<UserFormViewModel, UserModel>(user);
    }

    private async Task DeleteAction(UserViewModel user)
    {
        (RequestResult result, bool success) = await _api.Users.Delete(user.Id);
        
        if (result == RequestResult.Success && success) Users.Remove(user);
    }

    private async Task LoadUsersAsync()
    {
        (RequestResult result, List<UserModel> users) =
            await _api.Users.LenderPage(1, 20);

        Users.Clear();

        foreach (UserModel user in users)
            Users.Add(new UserViewModel(user, EditAction, DeleteAction));
    }
}