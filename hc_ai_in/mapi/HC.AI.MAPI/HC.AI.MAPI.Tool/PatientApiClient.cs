using System.Text;

namespace HC.AI.MAPI.Tool;

public class PatientApiClient
{
    private readonly HttpClient _httpClient;

    public PatientApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<HttpResponseMessage> PostQueryAsync(string queryJson)
    {
        var content = new StringContent(queryJson, Encoding.UTF8, "application/json");
        return _httpClient.PostAsync("/api/ai-search/execute-query", content);
    }
}
