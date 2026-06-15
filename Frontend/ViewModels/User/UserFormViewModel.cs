using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Threading;
using DynamicData;
using DynamicData.Binding;
using Frontend.Models;
using Frontend.Services;
using Frontend.ViewModels.Components;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace Frontend.ViewModels.User;

public class UserFormViewModel : FormViewModelBase<UserModel>, IDataErrorInfo
{
    private readonly IApiService _api;
    private readonly INavigationService _navigation;
    private readonly ISmartCardService _smartCard;
    private CompositeDisposable _disposables = new();

    public UserFormViewModel(HeaderViewModel header, FooterViewModel footer, IApiService api, INavigationService navigation, ISmartCardService smartCard) :
        base(header, footer)
    {
        _api = api;
        _navigation = navigation;
        _smartCard = smartCard;
        
        ResetCardIdCommand = ReactiveCommand.Create(ResetCardId);
    }

    #region Properties

    private int? _id;


    public byte[]? CardId
    {
        get => field;
        private set
        {
            this.RaiseAndSetIfChanged(ref field, value);
            this.RaisePropertyChanged(nameof(ResetCardVisible));
        }
    }
    
    [Required(ErrorMessage = "Voornaam is verplicht")]
    public string FirstName
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    [Required(ErrorMessage = "Achternaam is verplicht")]
    public string LastName
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    [Required(ErrorMessage = "E-Mail adres is verplicht")]
    [EmailAddress(ErrorMessage = "E-Mail adres moet valide zijn")]
    public string Email
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    [Required(ErrorMessage = "Persoonsnummer is verplicht")]
    [Range(100000, 9999999, ErrorMessage = "Persoonsnummer is verplicht")]
    public int? Number
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

    #region Scanner

    public string CardIdText
    {
        get;
        set
        {
            this.RaiseAndSetIfChanged(ref field, value);
            this.RaisePropertyChanged(nameof(ResetCardVisible));
        }
    } = string.Empty;

    private bool OnScan(byte[] uid)
    {
        Dispatcher.UIThread.Post(() =>
        {
            CardId = uid;
            CardIdText = "Kaartinformatie opgehaald! :D";
        });
        Console.WriteLine(IsFormValid);
        return true;
    }

    private void ResetCardId()
    {
        CardId = null;
        CardIdText = "Wachten op scanner-invoer...";
        _smartCard.SetCardDetectedCallback(OnScan);
    }

    public ICommand ResetCardIdCommand { get; }

    #endregion
    
    #region UI

    public ObservableCollection<RoleSelectionViewModel> Roles { get; } = [];
    
    public bool ResetCardVisible => CardId != null;

    public Thickness RolesErrorBorderThickness => string.IsNullOrWhiteSpace(RolesError)
        ? new Thickness(0)
        : new Thickness(1);

    public Thickness RolesErrorBorderMargin => string.IsNullOrWhiteSpace(RolesError)
        ? new Thickness(1)
        : new Thickness(0);

    #endregion
    
    #region Saving

    public ICommand? SaveCommand { get; set; }

    public async Task SaveUserAsync()
    {
        if (Number == null) return;

        UserModel user = new()
        {
            Id = _id ?? -1,
            FirstName = FirstName,
            LastName = LastName,
            Number = Number ??
                     throw new NullReferenceException(
                         "This error should not be able to instantiate and is only here to let the code compile."),
            Email = Email,
            Roles = SelectedRoleIds.Select(role => new RoleModel
            {
                Id = role
            }).ToList()
        };

        (RequestResult result, bool success) = await (_id == null
            ? _api.Users.Create(user)
            : _api.Users.Modify(user));

        if (result == RequestResult.Success && success) await _navigation.NavigateTo<UserPageViewModel>();
    }

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

    public string this[string columnName]
    {
        get
        {
            ValidationContext context = new(this) { MemberName = columnName };
            List<ValidationResult> results = [];
            object? value = GetType().GetProperty(columnName)?.GetValue(this);
            Console.WriteLine(columnName);

            if (!Validator.TryValidateProperty(value, context, results))
            {
                string? message = results.First().ErrorMessage;
                if (!string.IsNullOrEmpty(message)) return message;
            }

            return string.Empty;
        }
    }

    public string Error
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    private bool IsFormValid =>
        CardId != null  &&
        !string.IsNullOrWhiteSpace(FirstName) &&
        !string.IsNullOrWhiteSpace(LastName) &&
        Number is > 99999 and < 10000000 &&
        IsValidEmail() &&
        Roles.Any(x => x.IsSelected);
    
    private bool IsValidEmail()
    {
        try
        {
            _ = new MailAddress(Email);
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

        ResetCardId();
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
        
        IObservable<bool> canSave = this.WhenAnyValue(
                x => x.FirstName,
                x => x.LastName,
                x => x.Number,
                x => x.Email,
                x => x.CardId)
            .CombineLatest(
                Roles.ToObservableChangeSet()
                    .AutoRefresh(r => r.IsSelected)
                    .ToCollection()
                    .Select(roles => roles.Any(r => r.IsSelected))
                    .StartWith(false),
                (_, _) => IsFormValid);
        
        

        SaveCommand = ReactiveCommand.CreateFromTask(SaveUserAsync, canSave);
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

        (RequestResult result, List<RoleModel> roles) = await _api.Products.LenderRole();

        foreach (RoleModel role in roles)
            Roles.Add(new RoleSelectionViewModel(role.Id, role.Name, false));
    }

    private void ResetFields()
    {
        _id = null;
        CardId = null;
        FirstName = string.Empty;
        LastName = string.Empty;
        Email = string.Empty;
        Number = 0;

        foreach (RoleSelectionViewModel role in Roles)
            role.IsSelected = false;
    }

    #endregion

}