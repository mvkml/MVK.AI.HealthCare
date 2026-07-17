using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HC.AI.MAPI.Models;
using Microsoft.SemanticKernel;


namespace HC.AI.MAPI.Semantic.Factory
{
    public class KernalFactory : IKernalFactory
    {

        public KernalFactory() { }

        public Kernel CreateKernel(LLMOptions llmOptions)
        {
            switch (llmOptions.Provider)
            {
                default:
                    return Kernel.CreateBuilder()
                .AddOllamaChatCompletion(modelId: llmOptions.Model, endpoint: new Uri(llmOptions.BaseUrl))
                .Build();
            }

        }

    }
}
