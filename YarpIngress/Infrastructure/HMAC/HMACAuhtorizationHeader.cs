using Microsoft.Extensions.Primitives;

namespace YarpIngress.Infrastructure.HMAC;

public record HMACAuhtorizationHeader(string KeyId, string Signature)
{
    public static HMACAuhtorizationHeader None { get; } = new("", "");

    public static bool TryParse(StringValues values, out HMACAuhtorizationHeader result)
    {
        foreach (var headerValue in values.Where(v => !string.IsNullOrWhiteSpace(v)))
        {
            if (TryParse(headerValue!, out var matchingHeader))
            {
                result = matchingHeader;
                return true;
            }
        }

        result = None;
        return false;
    }

    /// <summary>
    /// Try to parse the HMAC parameters from an Authorization header value.
    /// </summary>
    /// <param name="value">The header value.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c> if the header has the right scheme and contains the required parameters.</returns>
    public static bool TryParse(string value, out HMACAuhtorizationHeader result)
    {
        var indexOfFirstSpace = value.IndexOf(' ');
        if (indexOfFirstSpace > 0)
        {
            var scheme = value[..indexOfFirstSpace];
            if (!string.Equals(scheme, HMACAuthenticationSchemeOptions.AuthenticationScheme, StringComparison.OrdinalIgnoreCase))
            {
                // Incorrect scheme
                result = None;
                return false;
            }

            // Note: using custom parsing here because System.Net.Http.Headers.AuthenticationHeader.TryParse
            // is a little too strict and cannot parse the parameters from the string

            var indexOfId = value.IndexOf("id=", StringComparison.OrdinalIgnoreCase);
            var indexOfSignature = value.IndexOf("signature=", StringComparison.OrdinalIgnoreCase);

            if (indexOfSignature < 0 || indexOfId < 0)
            {
                // Missing parameter
                result = None;
                return false;
            }

            string id, signature;

            if (indexOfId > indexOfSignature)
            {
                id = value[(indexOfId+3)..];
                signature = value[(indexOfSignature+10)..(indexOfId-1)];
            }
            else
            {
                id = value[(indexOfId + 3)..(indexOfSignature-1)];
                signature = value[(indexOfSignature+10)..];
            }

            result = new HMACAuhtorizationHeader(id.Trim('"', '\''), signature.Trim('"', '\''));
            return true;

        }
        
        result = None;
        return false;
    }
}