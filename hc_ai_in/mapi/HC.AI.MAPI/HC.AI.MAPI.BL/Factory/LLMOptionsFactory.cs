using HC.AI.MAPI.Models;
using Microsoft.Extensions.Configuration;

namespace HC.AI.MAPI.BL.Factory;

public class LLMOptionsFactory : ILLMOptionsFactory
{
    private readonly IConfiguration _configuration;

    public LLMOptionsFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public LLMOptions GetLLMOptions(string persona)
    {
        var options = new LLMOptions();
        _configuration.GetSection(persona).Bind(options);
        return options;
    }
}
