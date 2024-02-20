using Jina.Lang.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace Jina.Lang;

public static class LocalizeExtensions
{
    public static IServiceCollection AddLocalizer(this IServiceCollection services)
    {
        services.AddSingleton<ILocalizer, Localizer>();
        return services;
    } 
}