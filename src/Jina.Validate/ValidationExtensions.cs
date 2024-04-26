using System.Reflection;
using eXtensionSharp;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;

namespace Jina.Validate;

public static class ValidationExtensions
{
    public static Dictionary<string, string> vToDictionary(this List<ValidationFailure> items)
    {
        return items.Select(m => new KeyValuePair<string, string>(key: m.PropertyName, value: m.ErrorMessage))
            .ToDictionary();
    }
    
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