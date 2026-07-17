using System.Net.Http.Json;
using Microsoft.Extensions.Options;

namespace HC.AI.MAPI.Llm;

public class OllamaClient : IOllamaClient
{
    private readonly HttpClient _httpClient;
    private readonly OllamaOptions _options;

    public OllamaClient(HttpClient httpClient, IOptions<OllamaOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<string> ChatAsync(string systemPrompt, string userPrompt, bool jsonResponse = false)
    {
        var request = new OllamaChatRequest
        {
            Model = _options.Model,
            Messages = new List<OllamaMessage>
            {
                new() { Role = "system", Content = systemPrompt },
                new() { Role = "user", Content = userPrompt }
            },
            // "json" makes Ollama constrain its own output to valid JSON — used for the
            // query-building call (AgentV0.BuildQueryAsync) so the response can be
            // deserialized straight into QueryRequest without a separate parsing/repair step.
            // Left null for the answer-formatting call, which returns free-text prose.
            Format = jsonResponse ? "json" : null
        };

        // Non-streaming call: Stream defaults to false on OllamaChatRequest, so this always
        // waits for the full response body rather than reading a token stream.
        var response = await _httpClient.PostAsJsonAsync("/api/chat", request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<OllamaChatResponse>();
        return result?.Message.Content ?? string.Empty;
    }
}
