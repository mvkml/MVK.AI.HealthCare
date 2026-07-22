using HC.AI.MAPI.BL.Factory;
using HC.AI.MAPI.Common;
using HC.AI.MAPI.Models;

namespace HC.AI.MAPI.BL.LLMModel;

/// <summary>
/// Business logic for LLM/model selection. Resolves the appsettings.json section named by
/// <see cref="PromptItem.ModelKey"/> (set by <c>DoctorPromptMapper</c> from
/// <see cref="PromptItem.Persona"/> — e.g. "HCDocExecutor" for Doctor, "HCPatientExecutor" for
/// Patient) via <see cref="ILLMOptionsFactory"/>.
///
/// Still config-based, not database-backed (see backlog PB019/PB032/EPIC001 for the DB-driven
/// version) — this only fixes persona branching at the appsettings level. A persona whose section
/// doesn't exist in appsettings.json falls back to <see cref="LLMOptions"/>'s own defaults
/// (`IConfiguration.Bind` on a missing section is a no-op), not an error.
///
/// Per ADR001 (Context Object pattern), takes and returns the full <see cref="PromptModel"/>.
/// This is the ONLY class that decides the LLM provider/model — sets it on both
/// <c>model.LLMOptions</c> (envelope-level) and <c>model.PromptItem.LLMOptions</c>/
/// <c>LLMProvider</c> (the authoritative copy — the item is what a parallel/fan-out worker would
/// actually use), from the same value, so they can never drift apart.
/// </summary>
public class LLMModelBL : ILLMModelBL
{
    private readonly ILLMOptionsFactory _llmOptionsFactory;

    public LLMModelBL(ILLMOptionsFactory llmOptionsFactory)
    {
        _llmOptionsFactory = llmOptionsFactory;
    }

    public PromptModel GetModelDetails(PromptModel model)
    {
        var llmOptions = _llmOptionsFactory.GetLLMOptions(model.PromptItem.ModelKey);

        model.LLMOptions = llmOptions;
        model.PromptItem.LLMOptions = llmOptions;
        model.PromptItem.LLMProvider = llmOptions.Provider;

        return model;
    }
}
