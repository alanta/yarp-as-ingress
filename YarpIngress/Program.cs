using Microsoft.Extensions.DependencyInjection;
using YarpIngress.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddAuthentication(opts => {
    opts.AddScheme<HMACAuthenticationHandler>("hmac", "HMAC");
});

builder.Services.Configure<HMACAuthenticationSchemeOptions>(
    builder.Configuration.GetSection(key: HMACAuthenticationSchemeOptions.ConfigSectionName));

builder.Services.PostConfigure<HMACAuthenticationSchemeOptions>(opts =>
{
    new HMACAuthenticationBuilder(opts).AddSecret("Test", "SECRET", "API");
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("hmac", policy =>
        policy.RequireAuthenticatedUser());
});

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
var app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => "OK");

app.MapReverseProxy();

app.Run();

// This is to enable tests to attacht to the program
public partial class Program { }