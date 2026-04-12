using System.Reactive;
using System.Threading.Tasks;
using FrontendAdmin.Grpc;
using FrontendAdmin.Models;
using FrontendAdmin.Services;
using Google.Protobuf;
using Microsoft.Extensions.DependencyInjection;
using Protos.User;
using ReactiveUI;

namespace FrontendAdmin.ViewModels.User;

public class UserViewModel : ViewModelBase
{
    private readonly UserModel _model;

    public UserViewModel(ServiceProvider services, UserModel model) : base(services)
    {
        _model = model;

        EditCommand = ReactiveCommand.Create(() =>
        {
            Services.GetService<NavigationService>()?.NavigateTo(new UserFormViewModel(Services, this));
        });
        DeleteCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await DeleteAsync();
            Services.GetService<NavigationService>()?.NavigateTo(new UserPageViewModel(Services));
        });
    }

    public ReactiveCommand<Unit, Unit> EditCommand { get; }
    public ReactiveCommand<Unit, Unit> DeleteCommand { get; }

    public byte[] Id => _model.Id;

    public string Name
    {
        get => _model.Name;
        set
        {
            if (_model.Name == value) return;
            _model.Name = value;
            this.RaisePropertyChanged();
        }
    }

    public string Email
    {
        get => _model.Email;
        set
        {
            if (_model.Email == value) return;
            _model.Email = value;
            this.RaisePropertyChanged();
        }
    }

    public uint Number
    {
        get => _model.Number;
        set
        {
            if (_model.Number == value) return;
            _model.Number = value;
            this.RaisePropertyChanged();
        }
    }

    public bool Staff
    {
        get => _model.Staff;
        set
        {
            if (_model.Staff == value) return;
            _model.Staff = value;
            this.RaisePropertyChanged();
        }
    }

    private async Task DeleteAsync()
    {
        bool success = (await Client.Users.DeleteAsync(new UserDeleteRequest { Id = ByteString.CopyFrom(Id) })).Success;
    }
}