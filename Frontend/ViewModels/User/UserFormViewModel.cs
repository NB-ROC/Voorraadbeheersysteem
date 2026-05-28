using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Windows.Input;
using Frontend.Models;
using Frontend.Services;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace Frontend.ViewModels.User;

public class UserFormViewModel : ViewModelBase
{
    private readonly BackendService _backend;

    public UserFormViewModel(ServiceProvider services, UserViewModel? existing = null) :
        base(services)
    {
        _backend = services.GetService<BackendService>() ??
                   throw new NullReferenceException("Backend service not initialised");
        if (existing == null)
            CardId = Enumerable.Range(0, 7)
                .Select(_ => (byte)Random.Shared.Next(256))
                .ToArray();
        CardId = Enumerable.Range(0, 7).Select(_ => (byte)Random.Shared.Next(256)).ToArray();
        this.WhenAnyValue(
            x => x.FirstName,
            x => x.LastName,
            x => x.Email,
            x => x.Number
        ).Subscribe(_ => Validate());

        SaveCommand =
            ReactiveCommand.CreateFromTask(SaveProductAsync, this.WhenAnyValue(x => x.Error, string.IsNullOrEmpty));

        // If editing existing product, prefill values
        if (existing != null) LoadExistingProduct(existing);

        return;

        async Task SaveProductAsync()
        {
            UserModel model = new()
            {
                Id = Id,
                CardId = CardId,
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                Number = Number
            };

            (RequestResult result, bool success) = await (existing == null
                ? _backend.Users.Create(model)
                : _backend.Users.Modify(model));

            if (result == RequestResult.Success && success)
                Services.GetService<NavigationService>()?.NavigateTo(new UserPageViewModel(Services));
        }
    }

    #region Properties

    public int Id { get; set; }

    public byte[] CardId
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

    public int RoleId
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

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
        Error = CardId is not { Length: UserModel.IdLength }
            ? "Ongeldig ID."
            : string.IsNullOrWhiteSpace(FirstName)
                ? "Voornaam is verplicht."
                : string.IsNullOrWhiteSpace(LastName)
                    ? "Achternaam is verplicht."
                    : string.IsNullOrWhiteSpace(Email) || !IsValidEmail(Email)
                        ? "Ongeldig e-mailadres."
                        : RoleId <= 0
                            ? "Rol is verplicht."
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

    private void LoadExistingProduct(UserViewModel existing)
    {
        Id = existing.Id;
        CardId = existing.CardId;
        FirstName = existing.FirstName;
        LastName = existing.LastName;
        Email = existing.Email;
        Number = existing.Number;
    }

    #endregion
}