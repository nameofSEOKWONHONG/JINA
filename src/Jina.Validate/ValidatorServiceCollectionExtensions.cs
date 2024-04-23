using System.Reflection;
using eXtensionSharp;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Jina.Validate;

public static class ValidatorServiceCollectionExtensions
{
    public static IServiceCollection AddValidator(this IServiceCollection services, string domainName,
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
        services.AddValidatorsFromAssemblies(assemblies);
        return services;
    }
}