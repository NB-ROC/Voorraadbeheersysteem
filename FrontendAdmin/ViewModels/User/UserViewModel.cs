using System.Threading.Tasks;
using System.Windows.Input;
using FrontendAdmin.Grpc;
using FrontendAdmin.Models;
using FrontendAdmin.Services;
using Google.Protobuf;
using Protos.User;
using ReactiveUI;

namespace FrontendAdmin.ViewModels.User;

public class UserViewModel : ReactiveObject
{
    private readonly UserModel _model;
    private readonly UserPageViewModel _parentPage;

    public UserViewModel(UserModel model, NavigationService navigation, UserPageViewModel parent)
    {
        _model = model;
        _parentPage = parent;

        EditCommand = ReactiveCommand.Create<UserViewModel>(user =>
        {
            var formVm = new UserFormViewModel(navigation, parent, user);
            navigation.NavigateTo(formVm);
        });
        DeleteCommand = ReactiveCommand.CreateFromTask(DeleteAsync);
    }

    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }

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
        if (success) _parentPage.Users.Remove(this);
    }
}