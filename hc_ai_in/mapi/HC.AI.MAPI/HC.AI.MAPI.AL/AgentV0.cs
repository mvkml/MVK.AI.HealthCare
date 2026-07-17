using System.Text.Json;
using HC.AI.MAPI.Llm;
using HC.AI.MAPI.Models;
using HC.AI.MAPI.Tool;

namespace HC.AI.MAPI.AL;

public class AgentV0
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HealthcareQueryTool _queryTool;
    private readonly IOllamaClient _ollamaClient;

    public AgentV0(HealthcareQueryTool queryTool, IOllamaClient ollamaClient)
    {
        _queryTool = queryTool;
        _ollamaClient = ollamaClient;
    }

    public async Task<string> HandleRequestAsync(string userQuestion)
    {
        var query = await BuildQueryAsync(userQuestion);
        var result = await _queryTool.ExecuteQueryAsync(query);
        return await FormatResponseAsync(result);
    }

    private async Task<QueryRequest> BuildQueryAsync(string userQuestion)
    {
        var raw = await _ollamaClient.ChatAsync(
            HealthcareQueryPrompts.BuildQuerySystemPrompt(),
            userQuestion,
            jsonResponse: true);

        return JsonSerializer.Deserialize<QueryRequest>(raw, JsonOptions)
            ?? throw new InvalidOperationException("Ollama returned an empty query.");
    }

    private async Task<string> FormatResponseAsync(QueryResult result)
    {
        var resultJson = JsonSerializer.Serialize(result, JsonOptions);
        return await _ollamaClient.ChatAsync(
            HealthcareQueryPrompts.BuildAnswerSystemPrompt(),
            resultJson);
    }
}
