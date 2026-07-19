using System.Diagnostics;
using HC.AI.MAPI.Models;
using HC.AI.MAPI.Prompt.Doctor;
using HC.AI.MAPI.SemanticProcess;
using HC.AI.MAPI.SemanticProcess.Mapping;

namespace HC.AI.MAPI.AL;

/// <summary>
/// Orchestrates the Doctor persona's chat flow (US007/TASK008): gets the persona's system
/// prompt from <see cref="IDoctorPromptProvider"/>, tags it with the request's
/// <see cref="PromptItem.Persona"/>, and executes it through the generic Semantic layer
/// (<see cref="ISemanticProcessService"/>). Lives in the Agent Layer alongside
/// <c>DoctorAgent</c> per TASK008's resolved question #1 — persona-specific orchestration
/// belongs here, not inside the generic Semantic projects.
///
/// A single <see cref="PromptModel"/> carries Request + Item + LLMOptions in, and comes back
/// out with Response populated — every layer from here down passes this one model rather than
/// separate loose parameters (see ADR001). This class in particular only ever reads
/// <c>model.PromptItem</c> — never <c>model.PromptRequest</c>, which is intake-only and stops
/// at the Service layer. The item is the self-sufficient execution unit.
/// </summary>
public class DoctorSemanticProcess : IDoctorSemanticProcess
{
    private readonly ISemanticProcessService _semanticProcessService;
    private readonly IDoctorPromptProvider _doctorPromptProvider;

    public DoctorSemanticProcess(
        ISemanticProcessService semanticProcessService,
        IDoctorPromptProvider doctorPromptProvider)
    {
        _semanticProcessService = semanticProcessService;
        _doctorPromptProvider = doctorPromptProvider;
    }

    public async Task<PromptModel> ProcessAsync(PromptModel model)
    {
        var stopwatch = Stopwatch.StartNew();
        var item = model.PromptItem;
        var promptTemplate =
            $"{_doctorPromptProvider.GetSystemPrompt()}\n\n" +
            $"[Persona: {item.Persona}]\n" +
            $"User: {item.Message}\nAssistant:";

        try
        {
            var content = await _semanticProcessService.ExecutePromptAsync(item.LLMOptions, promptTemplate, item);
            stopwatch.Stop();
            model.PromptResponse = PromptItemMapper.ToPromptResponse(content, item.LLMOptions.Model, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            model.PromptResponse = PromptItemMapper.ToErrorPromptResponse(ex.Message, stopwatch.ElapsedMilliseconds);
        }

        return model;
    }
}
