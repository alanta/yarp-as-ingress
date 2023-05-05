using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace YarpIngress.Infrastructure;

internal class HMACAuthenticationHandler : AuthenticationHandler<HMACAuthenticationSchemeOptions>
{
    private readonly RequestValidator _validator;

    public HMACAuthenticationHandler(IOptionsMonitor<HMACAuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
        _validator = new RequestValidator(options.CurrentValue, logger.CreateLogger<RequestValidator>());
    }

    protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if( !_validator.HasValidHeaders(Request) ){
            return AuthenticateResult.NoResult();
        }

        if (Request.ContentLength != null && Request.ContentLength.Value < Options.MaxBytes)
        {
            return AuthenticateResult.Fail("Request too long");
        }

        if (!_validator.HasValidTimestamp(Request, Clock))
        {
            return AuthenticateResult.Fail("Timestamp invalid");
        }

        Request.EnableBuffering();
        var bodyAsText = await new StreamReader(Request.Body).ReadToEndAsync();
        Request.Body.Position = 0;

        var (success, user, roles) = _validator.ValidateSignature(Request, bodyAsText);
        if (!success)
        {
            return AuthenticateResult.Fail("Authentication required");
        }

        var claims = new List<Claim>{
            new Claim(ClaimTypes.Name, user)
        };

        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var ticket = new AuthenticationTicket(
            new System.Security.Claims.ClaimsPrincipal( new[]{ new ClaimsIdentity(claims, authenticationType: Scheme.Name) }),
            Scheme.Name
        );

        return AuthenticateResult.Success(ticket);
    }
}