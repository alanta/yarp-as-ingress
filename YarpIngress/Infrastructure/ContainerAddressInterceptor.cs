using Flurl;
using Yarp.ReverseProxy.Configuration;

namespace YarpIngress.Infrastructure;

/// <summary>
/// Handles runtime DNS resolution for other containers in the environment.
/// </summary>
public class ContainerAddressInterceptor : IProxyConfigFilter
{
    private const string ConfigSection = "Destinations";
    private readonly IConfiguration _configuration;

    public ContainerAddressInterceptor(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public ValueTask<ClusterConfig> ConfigureClusterAsync(ClusterConfig cluster, CancellationToken cancel)
    {
        var dnsSuffix = _configuration.GetValue<string>("CONTAINER_APP_ENV_DNS_SUFFIX");
        
        var newDests = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase);

        foreach (var destination in cluster.Destinations)
        {
            var key = destination.Key;
            var originalAddress = destination.Value.Address;

            var newAddress = _configuration.GetValue<string>($"Destinations:{key}");
            
            if (string.IsNullOrWhiteSpace(newAddress))
            {
                if (!string.IsNullOrWhiteSpace(dnsSuffix))
                {
                    // Build the address using the container name and DNS suffix
                    newAddress = $"https://{destination.Value.Address}.internal.{dnsSuffix}";
                }
                else
                {
                    if (!Url.IsValid(originalAddress))
                    {
                        throw new InvalidOperationException(
                            $"Invalid target url specified for destination '{key}'. Specify a proper URL for the destination or add a setting named 'Destinations:{key}' and specify the full url.");
                    }
                    
                    newAddress = originalAddress;
                }
            }

            // TODO : log and validate new address

            var modifiedDest = destination.Value with { Address = newAddress };
            newDests.Add(destination.Key, modifiedDest);

        }

        return new ValueTask<ClusterConfig>(cluster with { Destinations = newDests });
    }

    public ValueTask<RouteConfig> ConfigureRouteAsync(RouteConfig route, ClusterConfig? cluster, CancellationToken cancel)
    {
        return ValueTask.FromResult(route);
    }
}