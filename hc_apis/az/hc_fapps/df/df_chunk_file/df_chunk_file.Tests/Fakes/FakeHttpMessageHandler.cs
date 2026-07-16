namespace df_chunk_file.Tests.Fakes;

/// <summary>Hand-rolled HttpMessageHandler stub so tests don't need a mocking library or a real
/// HTTP server — the SUT's HttpClient is constructed directly over this handler.</summary>
public class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, HttpResponseMessage> _respond;

    public int CallCount { get; private set; }
    public HttpRequestMessage? LastRequest { get; private set; }

    public FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> respond)
    {
        _respond = respond;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        CallCount++;
        LastRequest = request;
        return Task.FromResult(_respond(request));
    }
}
