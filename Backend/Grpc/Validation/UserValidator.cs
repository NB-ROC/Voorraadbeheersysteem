using System.Net.Mail;
using Backend.Database.Managers;
using Backend.Entities;
using Google.Protobuf;
using Shared;

namespace Backend.Grpc.Validation;

/**
 * This class is for validating every type of user request object. This throws RpcException for every wrongdoing.
 */
public class UserValidator : Validator
{
    private readonly UserManager _userManager;

    public UserValidator(UserManager userManager)
    {
        _userManager = userManager;
    }

    public void ValidatePage(UserPageRequest request)
    {
        if (request.Page < 1)
            Throw("Invalid page");
        if (request.PageSize is < 1 or > 100)
            Throw("Invalid page size");
    }

    public void ValidateGet(UserGetRequest request)
    {
        ValidateId(request.Id);
    }

    public void ValidateCreate(UserCreateRequest request)
    {
        ValidateId(request.Id);
        ValidateName(request.Name);
        ValidateEmail(request.Email);
        ValidateNumber(request.Number, request.Staff);
    }

    /**
     * This is async due to the needed database logic here.
     */
    public async Task<User> ValidateModify(UserModifyRequest request)
    {
        User? user = await _userManager.Get(request.Id.ToByteArray());
        if (user == null) Throw("Invalid User");

        ValidateId(request.Id);
        if (request.HasEmail) ValidateEmail(request.Email);
        if (request.HasName) ValidateName(request.Name);
        if (request.HasNumber) ValidateNumber(request.Number, request.HasStaff ? request.Staff : user!.Staff);

        return user!;
    }

    public void ValidateDelete(UserDeleteRequest request)
    {
        ValidateId(request.Id);
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

    private static void ValidateId(ByteString? id)
    {
        if (id == null || id.Length != User.IdLength)
            Throw("Invalid id");
    }

    private static void ValidateEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email)) Throw("Invalid email");
        try
        {
            _ = new MailAddress(email!);
        }
        catch
        {
            Throw("Invalid email");
        }
    }

    private static void ValidateName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length > User.NameLength)
            Throw("Invalid name");
    }

    private static void ValidateNumber(uint? number, bool isStaff)
    {
        int length = GetNumberLength(number);

        int expectedLength = isStaff
            ? User.StaffNumberLength
            : User.StudentNumberLength;

        if (length != expectedLength)
            Throw("Invalid number");
    }
}