using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Windows.Input;
using FrontendAdmin.Grpc;
using FrontendAdmin.Models;
using FrontendAdmin.Services;
using Google.Protobuf;
using Microsoft.Extensions.DependencyInjection;
using Protos.User;
using ReactiveUI;

namespace FrontendAdmin.ViewModels.User;

public class UserFormViewModel : ViewModelBase
{
    public UserFormViewModel(ServiceProvider services, UserViewModel? existing = null) :
        base(services)
    {
        if (existing == null)
            Id = Enumerable.Range(0, 7)
                .Select(_ => (byte)Random.Shared.Next(256))
                .ToArray();
        Id = Enumerable.Range(0, 7).Select(_ => (byte)Random.Shared.Next(256)).ToArray();
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
            bool success;
            if (existing != null)
                success = (await Client.Users.ModifyAsync(new UserModifyRequest
                {
                    Id = ByteString.CopyFrom(existing.Id),
                    FirstName = FirstName,
                    LastName = LastName,
                    Email = Email,
                    Number = Number,
                    RoleId = RoleId,
                    IsBlocked = IsBlocked
                })).Success;
            else
                success = (await Client.Users.CreateAsync(new UserCreateRequest
                {
                    Id = ByteString.CopyFrom(Id),
                    FirstName = FirstName,
                    LastName = LastName,
                    Email = Email,
                    Number = Number,
                    RoleId = RoleId,
                    IsBlocked = IsBlocked
                })).Success;

            if (success) Services.GetService<NavigationService>()?.NavigateTo(new UserPageViewModel(Services));
        }
    }

    #region Properties

    public byte[] Id
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
        Error = Id is not { Length: UserModel.IdLength }
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
        FirstName = existing.FirstName;
        LastName = existing.LastName;
        Email = existing.Email;
        Number = existing.Number;
        RoleId = existing.RoleId;
        IsBlocked = existing.IsBlocked;
    }

    #endregion
}