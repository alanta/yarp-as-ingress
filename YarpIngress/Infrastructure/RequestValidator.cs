using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;

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
        var signature = req.Headers[_options.SignatureHeader];
        var signatureTimeStamp = req.Headers[_options.TimestampHeader];

        if (string.IsNullOrWhiteSpace(signature) || string.IsNullOrWhiteSpace(signatureTimeStamp))
        {
            _logger.LogError("Incoming request does not contain the required headers, found headers: {Headers}", string.Join(", ", req.Headers.Keys));
            return false;
        }

        return true;
    }

    public bool HasValidTimestamp(HttpRequest req, ISystemClock clock)
    {
        var signatureTimeStamp = req.Headers[_options.TimestampHeader];    
        if(!long.TryParse(signatureTimeStamp, out var signedTimestamp))
        {
            _logger.LogError("Incoming request has an invalid timestamp: {Timestamp}", signatureTimeStamp );
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

    public bool ValidateSignature(HttpRequest req, string body)
    {
        var signature = req.Headers[_options.SignatureHeader].ToString();
        var signatureTimeStamp = req.Headers[_options.TimestampHeader];

        // Handle reverse proxy headers
        var host = req.Headers.TryGetValue("X-Forwarded-Host", out var hostHeader) ? hostHeader.FirstOrDefault() : req.Host.Value;
        var scheme =  req.Headers.TryGetValue("X-Forwarded-Scheme", out var schemeHeader ) ? schemeHeader.FirstOrDefault() : req.Scheme;
        var path = $"{(string.IsNullOrEmpty(_options.BaseUri) ? "" : "/" + _options.BaseUri)}{req.Path.Value}";

        var stringToHash = $"{req.Method}{scheme}://{host}{path}{signatureTimeStamp}{body}";

        var signatureToMatch = CalculateHash(_options.ApiSecret, stringToHash);

        if (!signature.Equals(signatureToMatch, StringComparison.Ordinal))
        {
            _logger.LogError("Request contained invalid hash {ActualHash}, expected {ExpectedHash} for request {Method} {Scheme}://{Host}{Path}", signature, signatureToMatch, req.Method, scheme, host, path);
            return false;
        }

        return true;
    }

    public static string CalculateHash(string key, string input)
    {
        var encoding = new UTF8Encoding();

        var textBytes = encoding.GetBytes(input);
        var keyBytes = encoding.GetBytes(key);

        using var hash = new HMACSHA256(keyBytes);
        var hashBytes = hash.ComputeHash(textBytes);

        return Convert.ToBase64String(hashBytes);
    }
}
