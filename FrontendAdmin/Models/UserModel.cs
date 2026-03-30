using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FrontendAdmin.Models;

public class UserModel
{
    public const int IdLength = 7;
    public const int StaffNumberLength = 6;
    public const int StudentNumberLength = 7;
    public const int EmailLength = 255;
    public const int NameLength = 32;


    public byte[] Id { get; set; } = new byte[IdLength];

    [MaxLength(EmailLength)] public string Email { get; set; } = null!;

    [MaxLength(NameLength)] public string Name { get; set; } = null!;

    public uint Number { get; set; }

    public bool Staff { get; set; }
}