using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using FrontendAdmin.Grpc;
using FrontendAdmin.Models;
using FrontendAdmin.Services;
using Microsoft.Extensions.DependencyInjection;
using Protos.User;
using ReactiveUI;

namespace FrontendAdmin.ViewModels.User;

public class UserPageViewModel : PageViewModelBase
{
    public UserPageViewModel(ServiceProvider services) : base(services)
    {
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
        UserPageResponse? result = await Client.Users.PageAsync(new UserPageRequest
        {
            Page = 1,
            PageSize = 20
        });

        Users.Clear();
        foreach (MetaUser? user in result.Users)
            Users.Add(new UserViewModel(Services, new UserModel
            {
                Id = user.Id.ToByteArray(),
                Name = user.Name,
                Email = user.Email,
                Number = user.Number,
                Staff = user.Staff
            }));
    }
}