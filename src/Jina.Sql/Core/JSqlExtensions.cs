using JSqlEngine.Core;
using JSqlEngine.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Jina.Sql.Core;

public static class JSqlExtensions
{
    public static IServiceCollection AddJinaJSql(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<JSqlReader>()
            .AddSingleton<JSqlTimer>()
            .AddScoped<JSql>();

        services.Configure<JSqlOption>(configuration.GetSection(nameof(JSqlOption)));
        
        return services;
    }

    public static IHost UseJinaJSql(this IHost host)
    {
        var engine = host.Services.GetService<JSqlReader>();
#pragma warning disable CS8602 // null 가능 참조에 대한 역참조입니다.
        engine.Initialize();
#pragma warning restore CS8602 // null 가능 참조에 대한 역참조입니다.
        
        var timer = host.Services.GetService<JSqlTimer>();
#pragma warning disable CS8602 // null 가능 참조에 대한 역참조입니다.
        timer.Initialize();
#pragma warning restore CS8602 // null 가능 참조에 대한 역참조입니다.
        return host;
    }
}