using System.Net;
using Yarp.ReverseProxy.Forwarder;

namespace YarpIngress.Tests.Tools;

public class TestForwarderHttpClientFactory : ForwarderHttpClientFactory
{
    protected override HttpMessageHandler WrapHandler(ForwarderHttpClientContext context, HttpMessageHandler handler)
    {
        var fake = new FakeHttpMessageHandler();
        fake.AddResponse("/test", HttpStatusCode.OK);
        fake.AddResponse("/health", HttpStatusCode.OK);
        return fake;
    }
}