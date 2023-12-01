using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace StashMaven.Tests.WebApi;

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        Claim[] claims = { new(ClaimTypes.Name, "StashMaven Test User") };
        ClaimsIdentity identity = new(claims, "StashMavenTestIdentity");
        ClaimsPrincipal principal = new(identity);
        AuthenticationTicket ticket = new(principal, "StashMavenTestScheme");

        AuthenticateResult result = AuthenticateResult.Success(ticket);

        return Task.FromResult(result);
    }
}
