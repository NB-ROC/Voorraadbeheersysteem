using System.Collections.Generic;
using Protos.Scan;

namespace FrontendAdmin.Services;

public class AuthState
{
    public int? UserId { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public List<string> Roles { get; private set; } = new();
    public bool IsLender { get; private set; }

    public bool IsAuthenticated => UserId.HasValue;

    public void SetFromScan(int userId, TryLoginResponse response)
    {
        UserId = userId;

        FullName = $"{response.FirstName} {response.LastName}";

        Roles = response.Roles.ToList();

        IsLender = response.IsLender;
    }

    public void Clear()
    {
        UserId = null;
        FullName = string.Empty;
        Roles.Clear();
        IsLender = false;
    }
}