using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace YarpIngress.Tests;

public class When_invoking_healthchecks
{
    [Fact]
    public async Task Health_should_return_http_ok()
    {
        // Arrange
        var server = new WebApplicationFactory<Program>();
        var client = server.CreateClient();

        // Act
        var result = await client.GetAsync("/health");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}