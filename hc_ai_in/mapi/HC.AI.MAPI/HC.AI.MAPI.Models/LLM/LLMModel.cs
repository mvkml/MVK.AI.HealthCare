
namespace HC.AI.MAPI.Models;

public class LLMModel : BaseModel
{
    public LLMOptions LLMOptions { get; set; } = new LLMOptions();
}

public class LLMOptions
{

    public const string SectionName = "Ollama";
    public string Provider { get; set; } = "Ollama";
    public string BaseUrl { get; set; } = "http://localhost:11434";
    public string Model { get; set; } = "qwen2.5:7b";
}


public class OllamaOptions : LLMOptions
{

}

public class OpenAIOptoins : LLMOptions
{

}
