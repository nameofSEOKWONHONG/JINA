using System.Reflection;
using eXtensionSharp;
using Jina.Base.Service.Abstract;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace Jina.Injection;

public static class ScanExtensions
{
    public static IServiceCollection AddJinaScan(this IServiceCollection services, string domainName,
        string extensionName = ".dll")
    {
        var files = Directory.GetFiles(AppContext.BaseDirectory)
            .Where(m => Path.GetFileName(m).Contains(domainName) && Path.GetExtension(m) == extensionName)
            .ToList();
        var assemblies = new List<Assembly>();
        files.xForEach(file =>
        {
            assemblies.Add(Assembly.Load(Path.GetFileNameWithoutExtension(file)));
        });
    
        services.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(cls => cls.AssignableTo<IScopeService>())
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsImplementedInterfaces()
            .WithScopedLifetime()           
            .AddClasses(cls => cls.AssignableTo<ITransientService>())
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsImplementedInterfaces()
            .WithTransientLifetime()         
            .AddClasses(cls => cls.AssignableTo<ISingletonService>())
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsImplementedInterfaces()
            .WithSingletonLifetime()
        );
        
        return services;
    }
    
    public static IServiceCollection AddJinaScan(this IServiceCollection services, Assembly[] assemblies)
    {
        services.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(cls => cls.AssignableTo<IScopeService>())
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsImplementedInterfaces()
            .WithScopedLifetime()           
            .AddClasses(cls => cls.AssignableTo<ITransientService>())
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsImplementedInterfaces()
            .WithTransientLifetime()         
            .AddClasses(cls => cls.AssignableTo<ISingletonService>())
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsImplementedInterfaces()
            .WithSingletonLifetime()
        );

        return services;
    } 
}