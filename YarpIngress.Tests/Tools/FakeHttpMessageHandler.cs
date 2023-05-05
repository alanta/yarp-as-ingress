using System.Net;
using System.Text;
using System.Text.Json;

namespace YarpIngress.Tests.Tools;

/// <summary>
/// A fake message handler that can be used inject specific responses to requests.
/// </summary>
public class FakeHttpMessageHandler : DelegatingHandler
{
    readonly List<(Func<Uri, bool> predicate, HttpResponseMessage response, Action<HttpRequestMessage>? validateRequest)> _responses
        = new List<(Func<Uri, bool> predicate, HttpResponseMessage response, Action<HttpRequestMessage>? validateRequest)>();

    public void AddResponse(string uri, HttpStatusCode status, object? response = null, Action<HttpRequestMessage>? validateRequest = null)
    {
        AddResponse(request => request.PathAndQuery == uri, status, response, validateRequest);
    }

    public void AddResponse(Func<Uri, bool> predicate, HttpStatusCode status, object? response = null, Action<HttpRequestMessage>? validateRequest = null)
    {
        var httpResponse = new HttpResponseMessage(status);
        if (response != null)
        {
            httpResponse.Content = new StringContent(JsonSerializer.Serialize(response), Encoding.UTF8,
                "application/json");

        }
        _responses.Add((predicate, httpResponse, validateRequest));
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var (_, response, validateRequest) = _responses.FirstOrDefault(item => item.predicate(request.RequestUri));

        Assert.True(response != null, $"Request for {request.RequestUri.PathAndQuery} was not setup");

        validateRequest?.Invoke(request);

#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type. -- false positive because assert will throw
        return Task.FromResult(response);
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
    }
}