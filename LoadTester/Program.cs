using LoadTester;
using NBomber.CSharp;
using NBomber.Http.CSharp;

// A basic test that hits both the frontend and api container
//
// You can play with the injection rates and intervals to see 
// ACA scale the containers with traffic volume

string baseUrl = args.FirstOrDefault() ?? "http://localhost:5204/";
baseUrl = baseUrl.TrimEnd('/') + "/"; // ensure the url has a trailing slash

var scenario = Scenario.Create("http_scenario", async context =>
{
    using var httpClient = new HttpClient();
    var request =
        Http.CreateRequest("GET", baseUrl)
            .WithHeader("Accept", "text/html");

    var response = await Http.Send(httpClient, request);
    return response;
})
.WithoutWarmUp()
.WithLoadSimulations(Simulation.Inject(rate: 10, interval: TimeSpan.FromSeconds(2), during: TimeSpan.FromMinutes(30)));

var scenario2 = Scenario.Create("http_scenario2", async context =>
{
    using var httpClient = new HttpClient();
    var url = baseUrl + "api/hello/stranger";
    
    var request =
        Http.CreateRequest("GET", url )
            .WithHMACSignature("Test", "SECRET")
            .WithHeader("Accept", "text/html");

    var response = await Http.Send(httpClient, request);
    return response;
})
.WithoutWarmUp()
.WithLoadSimulations(Simulation.Inject(rate: 10, interval: TimeSpan.FromSeconds(2), during: TimeSpan.FromMinutes(30)));

NBomberRunner
    .RegisterScenarios(scenario, scenario2)
    .Run();
