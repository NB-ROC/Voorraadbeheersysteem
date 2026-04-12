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
        Id = Enumerable.Range(0, 7).Select(_ => (byte)Random.Shared.Next(256)).ToArray();
        this.WhenAnyValue(
            x => x.Name,
            x => x.Email,
            x => x.Number,
            x => x.Staff
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
                    Name = Name,
                    Email = Email,
                    Number = Number,
                    Staff = Staff
                })).Success;
            else
                success = (await Client.Users.CreateAsync(new UserCreateRequest
                {
                    Id = ByteString.CopyFrom(Id),
                    Name = Name,
                    Email = Email,
                    Number = Number,
                    Staff = Staff
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

    public string Name
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    public string Email
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    public uint Number
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = 0;

    public bool Staff
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = false;

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
            : string.IsNullOrWhiteSpace(Name) || Name.Length > UserModel.NameLength
                ? "Naam is verplicht en mag niet te lang zijn."
                : string.IsNullOrWhiteSpace(Email) || !IsValidEmail(Email)
                    ? "Ongeldig e-mailadres."
                    : GetNumberLength(Number) != (Staff ? UserModel.StaffNumberLength : UserModel.StudentNumberLength)
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
        Name = existing.Name;
        Email = existing.Email;
        Number = existing.Number;
        Staff = existing.Staff;
    }

    #endregion
}