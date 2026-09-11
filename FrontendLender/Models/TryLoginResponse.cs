namespace Frontend.Models;

public enum LoginResult
{
    Success,
    Invalid,
    Unregistered
}

public class TryLoginResponse
{
    public LoginResult Result { get; set; }
    public string Token { get; set; }
}