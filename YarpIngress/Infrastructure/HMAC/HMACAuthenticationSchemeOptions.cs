using Microsoft.AspNetCore.Authentication;

namespace YarpIngress.Infrastructure.HMAC;

public class HMACAuthenticationSchemeOptions : AuthenticationSchemeOptions
{
    public const string ConfigSectionName = "HMAC";
    public const string AuthenticationScheme = "HMAC-SHA256";
    const long DefaultMaxAgeInMilliseconds = 60000; // 1 minute
    public const string DefaultTimestampHeader = "X-Request-Timestamp";

    /// <summary>
    /// The maximum age of the request. Default is 1 minute.
    /// </summary>
    public long MaxAgeInMilliseconds { get; set; } = DefaultMaxAgeInMilliseconds;
       
    /// <summary>
    /// The name of the request header that contains the timestamp (unix)
    /// </summary>
    public string TimestampHeader { get; set; } = DefaultTimestampHeader;
    
    /// <summary>
    /// The path segment before the API uri. This is optional and used with reverse proxies that rewrite the URL (like AKS ingress). Please exclude leading or trailing slashes.
    /// </summary>
    public string BaseUri { get; set; } = "";
    
    public long MaxBytes {get; set; } = 32*1024; // 32Kb

    public Func<string,(string secret, string[] roles)>? ApiKeyResolver {get; set;}
}