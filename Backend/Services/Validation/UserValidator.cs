using Grpc.Core;
using Shared;
using Backend.Entities;
using Google.Protobuf;

namespace Backend.Services.Validation;

/**
 * This class is for validating every type of user request object, along
 */
public class UserValidator
{
    private static int GetNumberLength(uint? num)
    {
        return num switch
        {
            null => -1,
            0 => 1,
            _ => (int)Math.Floor(Math.Log10((double) num)) + 1
        };
    }

    private static void Throw(string message) =>
        throw new RpcException(new Status(StatusCode.InvalidArgument, message));

    // ------------------------
    // GET
    // ------------------------
    public void ValidateGet(UserGetRequest request)
    {
        if (request.Page < 0)
            Throw("Invalid page");

        if (request.PageSize <= 0 || request.PageSize > 100)
            Throw("Invalid page size");
    }

    // ------------------------
    // CREATE
    // ------------------------
    public void ValidateCreate(UserCreateRequest request)
    {
        ValidateId(request.Id);
        ValidateEmail(request.Email);
        ValidateName(request.Name);
        ValidateNumber(request.Number, request.Staff);
    }

    // ------------------------
    // MODIFY
    // ------------------------
    public void ValidateModify(UserModifyRequest request)
    {
        ValidateId(request.Id);

        if (request.HasEmail)
            ValidateEmail(request.Email);

        if (request.HasName)
            ValidateName(request.Name);

        if (request.HasNumber)
        {
            // If staff is also provided, use it
            bool isStaff = request.HasStaff && request.Staff;

            ValidateNumber(request.Number, isStaff);
        }
    }

    // ------------------------
    // DELETE
    // ------------------------
    public void ValidateDelete(UserDeleteRequest request)
    {
        ValidateId(request.Id);
    }

    // ------------------------
    // FIELD VALIDATORS
    // ------------------------
    private static void ValidateId(ByteString? id)
    {
        if (id == null || id.Length != User.IdLength)
            Throw("Invalid id");
    }

    private static void ValidateEmail(string? email)
    {
        try { _ = new System.Net.Mail.MailAddress(email!); }
        catch { Throw("Invalid email"); }
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