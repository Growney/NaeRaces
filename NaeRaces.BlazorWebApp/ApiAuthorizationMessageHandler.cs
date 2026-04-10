using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace NaeRaces.BlazorWebApp;

class ApiAuthorizationMessageHandler : DelegatingHandler
{
    private readonly IAccessTokenProvider _tokenProvider;
    private readonly string[] _authorizedUrls;

    public ApiAuthorizationMessageHandler(IAccessTokenProvider provider, string[] authorizedUrls)
    {
        _tokenProvider = provider;
        _authorizedUrls = authorizedUrls;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.RequestUri is not null && _authorizedUrls.Any(url => request.RequestUri.AbsoluteUri.StartsWith(url, StringComparison.OrdinalIgnoreCase)))
        {
            var tokenResult = await _tokenProvider.RequestAccessToken();
            if (tokenResult.TryGetToken(out var token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
