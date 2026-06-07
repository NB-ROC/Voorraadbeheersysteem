using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Mail;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using FrontendAdmin.Models;
using FrontendAdmin.Services;
using FrontendAdmin.ViewModels.Components;
using FrontendAdmin.ViewModels.Product;
using ReactiveUI;

namespace FrontendAdmin.ViewModels.User;

public class UserFormViewModel : FormViewModelBase<UserModel>, IDataErrorInfo
{
    private readonly IApiService _api;
    private readonly INavigationService _navigation;
    private CompositeDisposable _disposables;

    public UserFormViewModel(HeaderViewModel header, FooterViewModel footer, IApiService api, INavigationService navigation)
        : base(header, footer)
    {
        _api = api;
        _navigation = navigation;
        _disposables = new CompositeDisposable();
    }

    #region Properties

    private int? _id;

    public string FirstName
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    public string LastName
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    public int RoleId
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public string Email
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    public int Number
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = 0;
    
    private int[] SelectedRoleIds =>
        Roles
            .Where(x => x.IsSelected)
            .Select(x => x.Id)
            .ToArray();

    #endregion

    #region UI

    public ObservableCollection<RoleSelectionViewModel> Roles { get; } = [];
    
    public Thickness RolesErrorBorderThickness => string.IsNullOrWhiteSpace(RolesError) 
        ? new Thickness(0) 
        : new Thickness(1);
    
    public Thickness RolesErrorBorderMargin => string.IsNullOrWhiteSpace(RolesError) 
        ? new Thickness(1) 
        : new Thickness(0);

    #endregion

    #region Saving

    public ICommand SaveCommand { get; }
    

    #endregion

    #region Validation
    
    public string RolesError
    {
        get;
        private set
        {
            this.RaiseAndSetIfChanged(ref field, value); 
            this.RaisePropertyChanged(nameof(RolesErrorBorderThickness));
            this.RaisePropertyChanged(nameof(RolesErrorBorderMargin));
        }
    } = string.Empty;

    public string this[string column]
    {
        get
        {
            Console.WriteLine("Implement");
            return string.Empty;
        }
    }

    public string Error
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            _ = new MailAddress(email);
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    #endregion

    #region Loading

    public override async Task LoadAsync(UserModel? existing)
    {
        ResetFields();
        await LoadRolesAsync();
        if (existing != null) LoadUser(existing);
        
        LoadSubscriptions();
    }
    
    private void LoadSubscriptions()
    {
        _disposables.Dispose();
        _disposables = new CompositeDisposable();
        
        foreach (RoleSelectionViewModel role in Roles)
            role.WhenAnyValue(x => x.IsSelected)
                .Subscribe(_ => RolesError = Roles.Any(x => x.IsSelected)
                    ? string.Empty
                    : "Minstens één rol is verplicht.")
                .DisposeWith(_disposables);
    }

    private void LoadUser(UserModel user)
    {
        _id = user.Id;
        FirstName = user.FirstName;
        LastName = user.LastName;
        Email = user.Email;
        Number = user.Number;
        
        foreach (RoleSelectionViewModel role in Roles)
            role.IsSelected = user.Roles.Any(urm => urm.Id == role.Id);
    }

    private async Task LoadRolesAsync()
    {
        Roles.Clear();

        (RequestResult result, List<RoleModel> roles) = await _api.Products.Role();

        foreach (RoleModel role in roles)
            Roles.Add(new RoleSelectionViewModel(role.Id, role.Name, false));
    }

    private void ResetFields()
    {
        _id = null;
        FirstName = string.Empty;
        LastName = string.Empty;
        Email = string.Empty;
        Number = 0;
        
        foreach (RoleSelectionViewModel role in Roles)
            role.IsSelected = false;
    }

    #endregion

}

