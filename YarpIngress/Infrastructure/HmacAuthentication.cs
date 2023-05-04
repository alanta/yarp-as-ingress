using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

public class HMACAuthenticationSchemeOptions : AuthenticationSchemeOptions
{
    const long DefaultMaxAgeInMilliseconds = 60000; // 1 minute
    public const string DefaultKeyHeader = "X-Request-Key";
    public const string DefaultSignatureHeader = "X-Request-Signature";
    public const string DefaultTimestampHeader = "X-Request-Timestamp";

    /// <summary>
    /// The maximum age of the request. Default is 1 minute.
    /// </summary>
    public long MaxAgeInMilliseconds { get; set; } = DefaultMaxAgeInMilliseconds;
    
    /// <summary>
    /// The name of the request header that contains the signature
    /// </summary>
    public string SignatureHeader { get; set; }= DefaultSignatureHeader;
    
    /// <summary>
    /// The name of the request header that contains the timestamp (unix)
    /// </summary>
    public string TimestampHeader { get; set; } = DefaultTimestampHeader;
    
    /// <summary>
    /// The path segment before the API uri. This is optional and used with reverse proxies (like AKS ingress). Please exclude leading or trailing slashes.
    /// </summary>
    public string BaseUri { get; set; } = "";
    
    public long MaxBytes {get; set; } = 32*1024; // 32Kb

    /// <summary>
    /// The shared secret used to calculate the message signature.
    /// </summary>
    public string ApiSecret { get; set; } = "SECRET";
}

internal class HmacAuthentication : Microsoft.AspNetCore.Authentication.AuthenticationHandler<HMACAuthenticationSchemeOptions>
{
    private readonly RequestValidator _validator;

    public HmacAuthentication(IOptionsMonitor<HMACAuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
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

        if (!_validator.ValidateSignature(Request, bodyAsText))
        {
            return AuthenticateResult.Fail("Authentication required");
        }

        var claims = new[]{
            new Claim(ClaimTypes.Name, "Test"), // TODO
            new Claim(ClaimTypes.Role, "Default") // TODO
        };

        var ticket = new AuthenticationTicket(
            new System.Security.Claims.ClaimsPrincipal( new[]{ new ClaimsIdentity(claims, authenticationType: Scheme.Name) }),
            Scheme.Name
        );

        return AuthenticateResult.Success(ticket);
    }
}