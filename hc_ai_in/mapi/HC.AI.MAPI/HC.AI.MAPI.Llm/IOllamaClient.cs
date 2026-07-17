namespace HC.AI.MAPI.Llm;

public interface IOllamaClient
{
    Task<string> ChatAsync(string systemPrompt, string userPrompt, bool jsonResponse = false);
}
