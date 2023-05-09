var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplicationInsightsTelemetry();
var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.Run();
