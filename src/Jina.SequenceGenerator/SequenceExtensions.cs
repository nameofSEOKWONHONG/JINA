using Jina.SequenceGenerator.Services;
using Jina.SequenceGenerator.Services.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace Jina.SequenceGenerator;

public enum ENUM_SEQ_GENERATOR_TYPE
{
    Normal,
    Year,
}

public delegate ISequenceGenerator SequenceResolver(ENUM_SEQ_GENERATOR_TYPE type);

public static class SequenceExtensions
{
    public static IServiceCollection AddJinaSequence(this IServiceCollection services)
    {
        services.AddTransient<ISequenceGenerator, NormalSequenceGenerator>();
        services.AddTransient<ISequenceGenerator, YearSequenceGenerator>();
        
        services.AddTransient<SequenceResolver>(sp => implType =>
        {
            return implType switch
            {
                ENUM_SEQ_GENERATOR_TYPE.Normal => sp.GetRequiredService<NormalSequenceGenerator>(),
                ENUM_SEQ_GENERATOR_TYPE.Year => sp.GetRequiredService<YearSequenceGenerator>(),
                _ => throw new NotImplementedException()
            };
        });
        return services;
    }    
}
