using System;
using System.Reactive;
using System.Threading.Tasks;
using FrontendAdmin.Models;
using ReactiveUI;

namespace FrontendAdmin.ViewModels.User;

public class UserViewModel : ViewModelBase
{
    private readonly UserModel _model;

    public UserViewModel(UserModel model, Func<UserModel, Task> editAction, Func<UserViewModel, Task> deleteAction)
    {
        _model = model;

        EditCommand = ReactiveCommand.CreateFromTask(() => editAction(_model));
        DeleteCommand = ReactiveCommand.CreateFromTask(() => deleteAction(this));
    }

    public ReactiveCommand<Unit, Unit> EditCommand { get; }
    public ReactiveCommand<Unit, Unit> DeleteCommand { get; }

    public int Id { get; set; }

    public byte[] CardId => _model.CardId;

    public string FirstName
    {
        get => _model.FirstName;
        set
        {
            if (_model.FirstName == value) return;
            _model.FirstName = value;
            this.RaisePropertyChanged();
        }
    }

    public string LastName
    {
        get => _model.LastName;
        set
        {
            if (_model.LastName == value) return;
            _model.LastName = value;
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

    public int Number
    {
        get => _model.Number;
        set
        {
            if (_model.Number == value) return;
            _model.Number = value;
            this.RaisePropertyChanged();
        }
    }
}