using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;

namespace YarpIngress.Infrastructure.HMAC;

public class RequestValidator
{
    private readonly HMACAuthenticationSchemeOptions _options;
    private readonly ILogger _logger;

    public RequestValidator(HMACAuthenticationSchemeOptions options, ILogger logger)
    {
        _options = options;
        _logger = logger;
    }

    public bool HasValidHeaders(HttpRequest req)
    {
        var authValid = HMACAuhtorizationHeader.TryParse(req.Headers.Authorization, out var authHeaderValue);
        
        if ( !authValid )
        {
            _logger.LogError("Incoming request to {scheme}://{host}{path} does not contain a valid Authorization header", req.Scheme, req.Host, req.Path );
            _logger.LogDebug("Request headers: {Headers}", string.Join(", ", req.Headers.Keys));
            return false;
        }

        var signatureTimeStamp = req.Headers[_options.TimestampHeader];

        if (string.IsNullOrWhiteSpace(signatureTimeStamp))
        {
            _logger.LogError("Incoming request does not have the {timestampHeader} header", _options.TimestampHeader);
            return false;
        }

        return true;
    }

    public bool HasValidTimestamp(HttpRequest req, ISystemClock clock)
    {
        var signatureTimeStamp = req.Headers[_options.TimestampHeader].FirstOrDefault();
        if(!long.TryParse(signatureTimeStamp, out var signedTimestamp))
        {
            _logger.LogError("Incoming request has an invalid timestamp: {Timestamp}", signatureTimeStamp ?? "<empty>" );
            return false;
        }

        var currentTimeStamp = clock.UtcNow.ToUnixTimeSeconds();

        if (_options.MaxAgeInMilliseconds > 0 && currentTimeStamp - signedTimestamp > _options.MaxAgeInMilliseconds)
        {
            _logger.LogError("Timestamp is out of range, should not be older than {MaxAge}: {Timestamp}", TimeSpan.FromMilliseconds(_options.MaxAgeInMilliseconds), signatureTimeStamp);
            return false;
        }

        return true;
    }

    /*public bool ValidateSignature(HttpRequest req, string body, Func<string, string> getApiSecret )
{
    // https://doc.traefik.io/traefik-enterprise/middlewares/hmac/
    // Authorization: Hmac keyId="secret-id-1",algorithm="hmac-sha256",headers="(request-target) (created) (expires) host x-example",signature="c29tZXNpZ25hdHVyZQ==",created="1584453022",expires="1584453032"
    
    // https://learn.microsoft.com/en-us/azure/communication-services/tutorials/hmac-header-tutorial?pivots=programming-language-csharp
    // Authorization: "HMAC-SHA256 SignedHeaders=x-ms-date;host;x-ms-content-sha256&Signature=<hmac-sha256-signature>"

    // Azue AppConfig
    // https://learn.microsoft.com/en-us/azure/azure-app-configuration/rest-api-authentication-hmac
    // Date: Fri, 11 May 2018 18:48:36 GMT
    // x-ms-content-sha256: {SHA256 hash of the request body}
    // Authorization: HMAC-SHA256 Credential={Access Key ID}&SignedHeaders=x-ms-date;host;x-ms-content-sha256&Signature={Signature}
    
    var authHeader = req.Headers.Authorization;
}*/

    public (bool success, string id, string[] roles) ValidateSignature(HttpRequest req, string body )
    {
        if( !HMACAuhtorizationHeader.TryParse(req.Headers.Authorization, out var header ) )
        {
            return (false, "", Array.Empty<string>()); // TODO : move to proper response type
        }

        var signatureTimeStamp = req.Headers[_options.TimestampHeader];

        // Handle reverse proxy headers
        var host = req.Headers.TryGetValue("X-Forwarded-Host", out var hostHeader) ? hostHeader.FirstOrDefault() : req.Host.Value;
        var scheme = req.Headers.TryGetValue("X-Forwarded-Proto", out var schemeHeader ) ? schemeHeader.FirstOrDefault() : req.Scheme; // Envoy uses X-Forwarded-Proto
        var path = $"{(string.IsNullOrEmpty(_options.BaseUri) ? "" : "/" + _options.BaseUri)}{req.Path.Value}";

        var stringToHash = $"{req.Method}{scheme}://{host}{path}{signatureTimeStamp}{body}";

        if( _options.ApiKeyResolver == null ){
            throw new InvalidOperationException($"No HMAC key resolver configured. Please set the options.{nameof(HMACAuthenticationSchemeOptions.ApiKeyResolver)} property.");
        }

        var (secret, roles) = _options.ApiKeyResolver(header.KeyId);

        if (string.IsNullOrWhiteSpace(secret))
        {
            _logger.LogWarning("No API key found with id {keyId}", header.KeyId);
            return (false, "", Array.Empty<string>());
        }

        var signatureToMatch = CalculateHash(secret, stringToHash);

        if (!header.Signature.Equals(signatureToMatch, StringComparison.Ordinal))
        {
            _logger.LogError("Request contained invalid hash {ActualHash}, expected {ExpectedHash} for request {Method} {Scheme}://{Host}{Path}", header.Signature, signatureToMatch, req.Method, scheme, host, path);
            return (false, "", Array.Empty<string>());
        }

        return (true, header.KeyId, roles);
    }

    public static string CalculateHash(string key, string input)
    {
        var textBytes = Encoding.UTF8.GetBytes(input);
        var keyBytes = Encoding.UTF8.GetBytes(key);

        using var hash = new HMACSHA256(keyBytes);
        var hashBytes = hash.ComputeHash(textBytes);

        return Convert.ToBase64String(hashBytes);
    }
}