using Jina.Lang.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace Jina.Lang;

public static class JLocalizerExtensions
{
    public static IServiceCollection AddJinaLocalizer(this IServiceCollection services)
    {
        services.AddSingleton<ILocalizer, JLocalizer>();
        return services;
    } 
}