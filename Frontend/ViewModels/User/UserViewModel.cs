using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using Frontend.Models;
using Frontend.Services;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace Frontend.ViewModels.User;

public class UserViewModel : ViewModelBase
{
    private readonly ApiService _api;
    private readonly UserModel _model;

    public UserViewModel(ServiceProvider services, UserModel model) : base(services)
    {
        _model = model;
        _api = services.GetService<ApiService>() ??
                   throw new NullReferenceException("Backend service not initialised");

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

    public int Id
    {
        get => _model.Id;
        set
        {
            if (_model.Id == value) return;
            _model.Id = value;
            this.RaisePropertyChanged();
        }
    }

    public byte[] CardId => _model.CardId;
    
    public string Name => FirstName + " " + LastName;

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

    public List<RoleModel> Roles
    {
        get => _model.Roles;
        set
        {
            if (_model.Roles == value) return;
            _model.Roles = value;
            this.RaisePropertyChanged();
        }
    }


    private async Task DeleteAsync()
    {
        await _api.Users.Delete(Id);
    }
}