namespace YarpIngress.Infrastructure;

public class HMACAuthenticationBuilder
{
    public HMACAuthenticationBuilder(HMACAuthenticationSchemeOptions options)
    {
        Options = options;
        _defaultResolver = key => _apiSecrets.TryGetValue(key, out var result) ? result : (secret: "", roles: Array.Empty<string>());
    }

    public HMACAuthenticationSchemeOptions Options { get; }

    /// <summary>
    /// Set a single API secret. The key id parameter on the Authorization header is passed along as the user name without further validation.
    /// </summary>
    /// <param name="secret">The secret for the authenticated user.</param>
    /// <param name="roles">The roles to assign to the authenticated user.</param>
    /// <returns></returns>
    public HMACAuthenticationBuilder WithApiSecret(string secret, params string[] roles)
    {
        Options.ApiKeyResolver = _ => (secret,roles);

        return this;
    }

    /// <summary>
    /// Configure HMAC authentication to resolve secrets and roles externally.
    /// </summary>
    /// <param name="resolveApiKey">A func that will resolve a key to a secret and 0 or more roles. It should never throw and return an empty tuple if no matching key is found.</param>
    /// <returns>The builder.</returns>
    public HMACAuthenticationBuilder WithApiSecrets(Func<string,(string secret, string[] roles)> resolveApiKey)
    {
        Options.ApiKeyResolver = resolveApiKey;

        return this;
    }

    private readonly Dictionary<string, (string secret, string[] roles)> _apiSecrets = new (StringComparer.OrdinalIgnoreCase);
    private readonly Func<string, (string secret, string[] roles)> _defaultResolver;

    /// <summary>
    /// Add an API secret.
    /// </summary>
    /// <param name="key">The key for the secret. This is passed as the id parameter in the Authorization header.</param>
    /// <param name="secret">The API secret used to sign the request.</param>
    /// <param name="roles">The roles to assign to the authenticated user.</param>
    /// <returns>The builder.</returns>
    /// <exception cref="InvalidOperationException">Thrown when another method for resolving API keys was already set.</exception>
    public HMACAuthenticationBuilder AddSecret(string key, string secret, params string[] roles)
    {
        _apiSecrets.Add(key, (secret, roles));

        if (Options.ApiKeyResolver != null && Options.ApiKeyResolver != _defaultResolver)
            throw new InvalidOperationException("Cannot add secret, a custom key resolver has been configured.");
        Options.ApiKeyResolver ??= _defaultResolver;

        return this;
    }
}