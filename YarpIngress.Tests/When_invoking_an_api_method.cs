using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Yarp.ReverseProxy.Forwarder;
using YarpIngress.Infrastructure.HMAC;
using YarpIngress.Tests.Tools;

namespace YarpIngress.Tests;

public class When_invoking_an_api_method
{
    [Fact]
    public async Task It_should_not_allow_requests_without_a_signature()
    {
        // Arrange
        var server = new WebApplicationFactory<Program>();
        var client = server.CreateClient();

        // Act
        var result = await client.GetAsync("/secure-api/test");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // It should not 

    [Fact]
    public async Task It_should_not_allow_requests_with_an_invalid_signature()
    {
        // Arrange
        var server = new WebApplicationFactory<Program>();
        var client = server.CreateClient();

        var request = new HttpRequestMessage(HttpMethod.Post, "/secure-api/test");
        request.Headers.Add(HMACAuthenticationSchemeOptions.DefaultTimestampHeader, DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("HMAC-SHA26", "id=test;signature=nope");

        // Act
        var result = await client.SendAsync(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task It_should_allow_requests_with_an_valid_signature()
    {
        // Arrange
        var server = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseSetting("HMAC:BaseUri", "");
            builder.ConfigureTestServices(services =>
                services.AddSingleton<IForwarderHttpClientFactory>(new TestForwarderHttpClientFactory()));
        });
        var client = server.CreateClient();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/test");
        var timestamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
        request.Headers.Add(HMACAuthenticationSchemeOptions.DefaultTimestampHeader, DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("HMAC-SHA256", "id=test,signature=\"" + RequestValidator.CalculateHash("SECRET", $"POSThttp://localhost/api/test{timestamp}")+"\"");

        // Act
        var result = await client.SendAsync(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory,
     InlineData("HMAC-SHA256 id=\"test\",signature=\"abcdfg=\"", "Quotes are required by RFC"),
     InlineData("HMAC-SHA256 id='test',signature='abcdfg='", "Single quotes are allowed"),
     InlineData("HMAC-SHA256 id=test,signature=abcdfg=", "We can handle not having quotes"),
     InlineData("HMAC-SHA256 signature=abcdfg=,id=test", "Order of the properties is not strict")]
    public void It_should_parse_correct_headers(string header, string because)
    {
        // Arrange
        var headerValues = new StringValues(header);

        // Act
        var valid = HMACAuhtorizationHeader.TryParse(headerValues, out var result);

        // Assert
        valid.Should().BeTrue();
        result.KeyId.Should().Be("test", because);
        result.Signature.Should().Be("abcdfg=", because);
    }
}