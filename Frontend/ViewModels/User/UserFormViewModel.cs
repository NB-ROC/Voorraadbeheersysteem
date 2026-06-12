using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Windows.Input;
using Frontend.Models;
using Frontend.Services;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace Frontend.ViewModels.User;

public class UserFormViewModel : PageViewModelBase
{
    private readonly ApiService _api;

    public UserFormViewModel(ServiceProvider services, UserViewModel? existing = null) :
        base(services)
    {
        _api = services.GetService<ApiService>() ??
                   throw new NullReferenceException("Backend service not initialised");

        if (existing == null)
            _cardId = Enumerable.Range(0, 7)
                .Select(_ => (byte)Random.Shared.Next(256))
                .ToArray();

        Roles.Add(new RoleSelectionViewModel(services, 3, "Student", false));
        Roles.Add(new RoleSelectionViewModel(services, 4, "Personnel", false));
        Roles.Add(new RoleSelectionViewModel(services, 5, "Guest", false));

        foreach (RoleSelectionViewModel role in Roles)
            role.WhenAnyValue(x => x.IsSelected).Subscribe(_ => Validate());

        this.WhenAnyValue(
            x => x.FirstName,
            x => x.LastName,
            x => x.Email,
            x => x.Number
        ).Subscribe(_ => Validate());

        Validate();

        SaveCommand =
            ReactiveCommand.CreateFromTask(SaveAsync, this.WhenAnyValue(x => x.Error, string.IsNullOrEmpty));

        if (existing != null)
        {
            LoadExistingUser(existing);
            CardIdText = "Kaart is al gescand";
        }
        else
        {
            services.GetRequiredService<SmartCardService>().SetCardDetectedCallback(ScanCallback);
            CardIdText = "Wachten op scan...";
        }

        return;

        async Task SaveAsync()
        {
            UserModel model = new()
            {
                Id = Id,
                CardId = _cardId ?? throw new NullReferenceException(),
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                Number = Number,
                Roles = SelectedRoleIds.Select(id => new RoleModel { Id = id }).ToList()
            };

            (RequestResult result, bool success) = await (existing == null
                ? _api.Users.Create(model)
                : _api.Users.Modify(model));

            if (result == RequestResult.Success && success)
                Services.GetService<NavigationService>()?.NavigateTo(new UserPageViewModel(Services));
        }
    }

    #region Properties

    public int Id { get; set; }

    private byte[]? _cardId;

    public string CardIdText
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

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

    public ObservableCollection<RoleSelectionViewModel> Roles { get; } = [];

    public int[] SelectedRoleIds =>
        Roles
            .Where(x => x.IsSelected)
            .Select(x => x.Id)
            .ToArray();

    public bool IsBlocked
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


    public string Error
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    public ICommand SaveCommand { get; }

    #endregion

    #region Private Methods

    private void Validate()
    {
        Error = _cardId is not { Length: UserModel.IdLength }
            ? "Ongeldig ID."
            : string.IsNullOrWhiteSpace(FirstName)
                ? "Voornaam is verplicht."
                : string.IsNullOrWhiteSpace(LastName)
                    ? "Achternaam is verplicht."
                    : string.IsNullOrWhiteSpace(Email) || !IsValidEmail(Email)
                        ? "Ongeldig e-mailadres."
                        : !Roles.Any(x => x.IsSelected)
                            ? "Minstens één rol is verplicht."
                            : Number <= 0
                                ? "Ongeldig nummer."
                                : string.Empty;
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

    private static int GetNumberLength(uint? num)
    {
        return num switch
        {
            null => -1,
            0 => 1,
            _ => (int)Math.Floor(Math.Log10((double)num)) + 1
        };
    }

    private void LoadExistingUser(UserViewModel existing)
    {
        Id = existing.Id;
        _cardId = existing.CardId;
        FirstName = existing.FirstName;
        LastName = existing.LastName;
        Email = existing.Email;
        Number = existing.Number;

        foreach (RoleSelectionViewModel role in Roles)
            role.IsSelected = existing.Roles.Select(r => r.Id).Contains(role.Id);
    }

    private void ScanCallback(byte[] cardId)
    {
        _cardId = cardId;
        CardIdText = "Kaart gescand!";
    }

    #endregion
}