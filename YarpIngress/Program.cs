var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddAuthentication(opts => {
    opts.AddScheme<HmacAuthentication>("hmac", "HMAC");
});

builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("hmac", policy =>
            policy.RequireAuthenticatedUser());
    });

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy();
app.Run();