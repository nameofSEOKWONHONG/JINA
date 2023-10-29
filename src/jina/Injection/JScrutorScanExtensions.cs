using System.Reflection;
using eXtensionSharp;
using Jina.Base.Service.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace Jina.Injection;

public static class JScrutorScanExtensions
{
    public static IServiceCollection AddJinaScan(this IServiceCollection services, Type[] types)
    {
        var assemblies = new List<Assembly>();
        types.xForEach(item =>
        {
            assemblies.Add(item.Assembly);
        });
        
        services.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(cls => cls.AssignableTo<IScopeService>())
            .AsImplementedInterfaces()
            .WithScopedLifetime()           
            .AddClasses(cls => cls.AssignableTo<ITransientService>())
            .AsImplementedInterfaces()
            .WithTransientLifetime()         
            .AddClasses(cls => cls.AssignableTo<ISingletonService>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime()
        );

        return services;
    } 
}