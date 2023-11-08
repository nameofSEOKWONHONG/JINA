using Jina.Storage.Abstract;
using Jina.Storage.Impl;
using Microsoft.Extensions.DependencyInjection;

namespace Jina.Storage;

public enum ENUM_STOREAGE_TYPE
{
    Azure = 1,
    Aws   = 2,
    Gcp   = 3
}

public delegate IStorage StorageResolver(ENUM_STOREAGE_TYPE type);

public static class StorageExtensions
{
    public static IServiceCollection AddJinaStorage(this IServiceCollection services)
    {
        services.AddTransient<IStorage, AzureStorage>();

        services.AddTransient<StorageResolver>(sp => type =>
        {
            return type switch
            {
                ENUM_STOREAGE_TYPE.Azure => sp.GetRequiredService<IStorage>(),
                _ => throw new NotImplementedException()
            };
        });

        return services;
    }    
}
