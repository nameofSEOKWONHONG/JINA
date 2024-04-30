using eXtensionSharp;
using Jina.Database.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace Jina.Database;

public delegate IDbProviderBase DbProviderResolver(ENUM_DB_TYPE type);

public static class DatabaseExtensions
{
    /// <summary>
    /// Add Database Service, if use Multi Database
    /// </summary>
    /// <param name="services"></param>
    /// <param name="mssqlCreator"></param>
    /// <param name="mysqlCreator"></param>
    /// <param name="npgsqlCreator"></param>
    /// <typeparam name="TImpl"></typeparam>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static IServiceCollection AddJinaDatabase<TImpl>(this IServiceCollection services, Func<TImpl> mssqlCreator, Func<TImpl> mysqlCreator = null, Func<TImpl> npgsqlCreator = null)
        where TImpl : DbProviderBase, new()
    {
        if (mssqlCreator.xIsNotEmpty()) services.AddTransient<IDbProviderBase, TImpl>(sp => mssqlCreator());
        if (mysqlCreator.xIsNotEmpty()) services.AddTransient<IDbProviderBase, TImpl>(sp => mysqlCreator());
        if (npgsqlCreator.xIsNotEmpty()) services.AddTransient<IDbProviderBase, TImpl>(sp => npgsqlCreator());
        
        services.AddTransient<DbProviderResolver>(sp => implType =>
        {
            return implType switch
            {
                ENUM_DB_TYPE.Mssql => sp.GetRequiredService<MsSqlProvider>(),
                ENUM_DB_TYPE.Mysql => sp.GetRequiredService<MySqlProvider>(),
                ENUM_DB_TYPE.Npgsql => sp.GetRequiredService<NpgSqlProvider>(),
                _ => throw new NotImplementedException()
            };
        });
        return services;
    }
    
    /// <summary>
    /// if use single database
    /// </summary>
    /// <param name="services"></param>
    /// <param name="creator"></param>
    /// <typeparam name="TImpl"></typeparam>
    /// <returns></returns>
    public static IServiceCollection AddJinaDatabase<TImpl>(this IServiceCollection services, Func<TImpl> creator)
        where TImpl : DbProviderBase
    {
        services.AddSingleton<IDbProviderBase, TImpl>(sp => creator());
        return services;
    }
}

public enum ENUM_DB_TYPE
{
    Mssql,
    Mysql,
    Npgsql
}