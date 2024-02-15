using JSqlEngine.Core;
using JSqlEngine.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Jina.Sql.Core;

public static class JSqlExtensions
{
    public static IServiceCollection AddJSql(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<JSqlReader>()
            .AddSingleton<JSqlTimer>()
            .AddScoped<JSql>();

        services.Configure<JSqlOption>(configuration.GetSection(nameof(JSqlOption)));
        
        return services;
    }

    public static IHost UseJSql(this IHost host)
    {
        var reader = host.Services.GetService<JSqlReader>();
        reader.Initialize();
        
        var timer = host.Services.GetService<JSqlTimer>();
        timer.Initialize();
        return host;
    }
}