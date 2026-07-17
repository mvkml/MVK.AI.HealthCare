namespace HC.AI.MAPI.SemanticProcess;

public interface ISemanticProcessService
{
    Task<string> ExecutePromptAsync(string promptTemplate, IDictionary<string, object?> arguments);
}
