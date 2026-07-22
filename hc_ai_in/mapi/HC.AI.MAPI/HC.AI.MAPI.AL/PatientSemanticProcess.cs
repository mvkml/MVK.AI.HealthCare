using System.Diagnostics;
using HC.AI.MAPI.Models;
using HC.AI.MAPI.Prompt.Patient;
using HC.AI.MAPI.SemanticProcess;
using HC.AI.MAPI.SemanticProcess.Mapping;

namespace HC.AI.MAPI.AL;

/// <summary>
/// Patient-side counterpart to <see cref="DoctorSemanticProcess"/> — same shape, swaps in
/// <see cref="IPatientPromptProvider"/> for the persona's system prompt. Executes through the
/// same generic Semantic layer (<see cref="ISemanticProcessService"/>), so no Patient-specific
/// execution logic is needed, only the persona-specific prompt assembly.
/// </summary>
public class PatientSemanticProcess : IPatientSemanticProcess
{
    private readonly ISemanticProcessService _semanticProcessService;
    private readonly IPatientPromptProvider _patientPromptProvider;

    public PatientSemanticProcess(
        ISemanticProcessService semanticProcessService,
        IPatientPromptProvider patientPromptProvider)
    {
        _semanticProcessService = semanticProcessService;
        _patientPromptProvider = patientPromptProvider;
    }

    public async Task<PromptModel> ProcessAsync(PromptModel model)
    {
        var stopwatch = Stopwatch.StartNew();
        var item = model.PromptItem;
        var promptTemplate =
            $"{_patientPromptProvider.GetSystemPrompt()}\n\n" +
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
