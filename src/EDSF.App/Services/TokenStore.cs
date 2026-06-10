namespace EDSF.App.Services;

public class TokenStore
{
    public string? Token { get; set; }
    public string? Username { get; set; }
    public string? Role { get; set; }
    public bool IsLoggedIn => !string.IsNullOrEmpty(Token);
}
