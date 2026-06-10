using Microsoft.JSInterop;

namespace EDSF.App.Services;

public class AuthService
{
    private readonly IJSRuntime _js;
    private readonly TokenStore _tokenStore;
    private const string TokenKey = "auth_token";
    private const string UserKey = "auth_user";

    public string? Username => _tokenStore.Username;
    public string? Role => _tokenStore.Role;
    public bool IsLoggedIn => _tokenStore.IsLoggedIn;

    public AuthService(IJSRuntime js, TokenStore tokenStore)
    {
        _js = js;
        _tokenStore = tokenStore;
    }

    public async Task InitializeAsync()
    {
        try
        {
            var token = await _js.InvokeAsync<string?>("localStorage.getItem", TokenKey);
            if (!string.IsNullOrEmpty(token))
            {
                _tokenStore.Token = token;
                _tokenStore.Username = await _js.InvokeAsync<string?>("localStorage.getItem", UserKey);
            }
        }
        catch { }
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        _tokenStore.Username = username;
        _tokenStore.Role = "admin";
        _tokenStore.Token = "dev-bypass";

        await _js.InvokeVoidAsync("localStorage.setItem", TokenKey, _tokenStore.Token);
        await _js.InvokeVoidAsync("localStorage.setItem", UserKey, _tokenStore.Username);

        return true;
    }

    public async Task LogoutAsync()
    {
        await _js.InvokeVoidAsync("localStorage.removeItem", TokenKey);
        await _js.InvokeVoidAsync("localStorage.removeItem", UserKey);
        _tokenStore.Token = null;
        _tokenStore.Username = null;
        _tokenStore.Role = null;
    }
}
