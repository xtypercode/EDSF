using System.Net.Http.Headers;

namespace EDSF.App.Services;

public class AuthHeaderHandler : DelegatingHandler
{
    private readonly TokenStore _tokenStore;

    public AuthHeaderHandler(TokenStore tokenStore)
    {
        _tokenStore = tokenStore;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = _tokenStore.Token;
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        return base.SendAsync(request, cancellationToken);
    }
}
