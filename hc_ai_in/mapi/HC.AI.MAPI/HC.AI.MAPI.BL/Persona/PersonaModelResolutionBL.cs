using HC.AI.MAPI.Models.Persona;

namespace HC.AI.MAPI.BL.Persona;

public class PersonaModelResolutionBL : IPersonaModelResolutionBL
{
    private readonly IPersonaLlmConfigProvider _configProvider;

    public PersonaModelResolutionBL(IPersonaLlmConfigProvider configProvider)
    {
        _configProvider = configProvider;
    }

    public PersonaModelResolutionResult ResolveClassification(int roleId)
    {
        var option = _configProvider.GetActiveOption(roleId, ModelRole.Classification, null);
        var prompt = _configProvider.GetActivePrompt(roleId, ModelRole.Classification, null);

        return BuildResult(option, prompt);
    }

    public PersonaModelResolutionResult ResolveExecutor(int roleId, string promptTypeCode)
    {
        var promptType = _configProvider.GetPromptTypeByCode(roleId, promptTypeCode);
        if (promptType is null)
        {
            return new PersonaModelResolutionResult { IsResolved = false };
        }

        var option = _configProvider.GetActiveOption(roleId, ModelRole.Executor, promptType.PersonaPromptTypeId);
        var prompt = _configProvider.GetActivePrompt(roleId, ModelRole.Executor, promptType.PersonaPromptTypeId);

        return BuildResult(option, prompt);
    }

    private static PersonaModelResolutionResult BuildResult(PersonaLlmOption? option, PersonaPromptRecord? prompt)
    {
        if (option is null || prompt is null)
        {
            return new PersonaModelResolutionResult { IsResolved = false };
        }

        return new PersonaModelResolutionResult
        {
            IsResolved = true,
            ModelName = option.ModelName,
            Provider = option.Provider,
            PromptText = prompt.PromptText
        };
    }
}
