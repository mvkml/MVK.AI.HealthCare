using HC.AI.MAPI.Models;
using Microsoft.SemanticKernel;

namespace HC.AI.MAPI.Semantic.Factory;

public interface IKernalFactory
{
    Kernel CreateKernel(LLMOptions llmOptions);
}
