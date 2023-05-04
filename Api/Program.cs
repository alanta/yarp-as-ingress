var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/hello/{name}", (string name) => $"Hello {name}!");

app.MapGet("/health", () => $"OK");

app.Run();
