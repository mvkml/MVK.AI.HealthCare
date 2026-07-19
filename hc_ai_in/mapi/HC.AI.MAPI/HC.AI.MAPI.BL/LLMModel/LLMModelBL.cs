using HC.AI.MAPI.Models;
using Microsoft.Extensions.Options;
using OllamaOptions = HC.AI.MAPI.Llm.OllamaOptions;

namespace HC.AI.MAPI.BL.LLMModel;

/// <summary>
/// Business logic for LLM/model selection. Today this always returns the single Ollama
/// configuration bound from appsettings, regardless of <see cref="PromptItem.Persona"/>.
///
/// NOT IMPLEMENTED (see backlog PB019): persona/user-type-driven model selection (Doctor vs
/// Insurance Provider vs Client) sourced from a database-backed configuration table, deciding
/// provider (Ollama, OpenAI, etc.) and model per persona. This class exists now so callers
/// already depend on "ask the BL for model details" rather than reading config directly —
/// swapping in the database-backed lookup later is a change local to this class.
///
/// Per ADR001 (Context Object pattern), takes and returns the full <see cref="PromptModel"/>.
/// This is the ONLY class that decides the LLM provider/model — sets it on both
/// <c>model.LLMOptions</c> (envelope-level) and <c>model.PromptItem.LLMOptions</c>/
/// <c>LLMProvider</c> (the authoritative copy — the item is what a parallel/fan-out worker would
/// actually use), from the same value, so they can never drift apart.
/// </summary>
public class LLMModelBL : ILLMModelBL
{
    private readonly OllamaOptions _ollamaOptions;

    public LLMModelBL(IOptions<OllamaOptions> ollamaOptions)
    {
        _ollamaOptions = ollamaOptions.Value;
    }

    public PromptModel GetModelDetails(PromptModel model)
    {
        var llmOptions = new LLMOptions
        {
            Provider = _ollamaOptions.Provider,
            BaseUrl = _ollamaOptions.BaseUrl,
            Model = _ollamaOptions.Model,
        };

        model.LLMOptions = llmOptions;
        model.PromptItem.LLMOptions = llmOptions;
        model.PromptItem.LLMProvider = llmOptions.Provider;

        return model;
    }
}
