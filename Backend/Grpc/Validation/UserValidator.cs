using System.Net.Mail;
using Backend.Database.Managers;
using Backend.Entities;
using Protos.User;

namespace Backend.Grpc.Validation;

public class UserValidator : Validator
{
    private readonly UserManager _manager;

    public UserValidator(UserManager manager)
    {
        _manager = manager;
    }

    public void ValidatePage(UserPageRequest request)
    {
        if (request.Page < 1) Throw("Invalid page");
        if (request.PageSize is < 1 or > 100) Throw("Invalid page size");
    }

    public void ValidateCreate(UserCreateRequest request)
    {
        ValidateName(request.FirstName);
        ValidateName(request.LastName);
        ValidateEmail(request.Email);

        if (request.RoleId <= 0)
            Throw("Invalid role");
    }

    public async Task<User> ValidateModify(UserModifyRequest request)
    {
        User? user = await _manager.Get(request.Id.ToByteArray());
        if (user == null) Throw("Invalid user");

        if (request.HasFirstName) ValidateName(request.FirstName);
        if (request.HasLastName) ValidateName(request.LastName);
        if (request.HasEmail) ValidateEmail(request.Email);

        return user;
    }

    private static void ValidateEmail(string email)
    {
        try
        {
            _ = new MailAddress(email);
        }
        catch
        {
            Throw("Invalid email");
        }
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length > 50)
            Throw("Invalid name");
    }
}