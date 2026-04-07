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

public class UserPageViewModel : ViewModelBase
{
    public UserPageViewModel(ServiceProvider services) : base(services)
    {
        LoadUsersCommand = ReactiveCommand.CreateFromTask(LoadUsersAsync);
        LoadUsersCommand.Execute();
    }


    public ObservableCollection<UserViewModel> Users { get; } = [];

    public ReactiveCommand<Unit, Unit> LoadUsersCommand { get; }

    public async Task LoadUsersAsync()
    {
        UserPageResponse? result = await Client.Users.PageAsync(new UserPageRequest
        {
            Page = 1,
            PageSize = 20
        });

        Users.Clear();
        foreach (MetaUser? user in result.Users)
            Users.Add(new UserViewModel(new UserModel
            {
                Id = user.Id.ToByteArray(),
                Name = user.Name,
                Email = user.Email,
                Number = user.Number,
                Staff = user.Staff,
            }, Services., this));
    }
}