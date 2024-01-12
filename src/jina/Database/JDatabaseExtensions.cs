using eXtensionSharp;
using Jina.Database.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace Jina.Database;

public enum ENUM_DB_TYPE
{
    Mssql,
    Mysql,
    Npgsql
}

public delegate IDbProvider JDbProviderResolver(ENUM_DB_TYPE type);

public static class JDatabaseExtensions
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
        where TImpl : JSqlProviderBase, new()
    {
        if (mssqlCreator.xIsNotEmpty()) services.AddTransient<IDbProvider, TImpl>(sp => mssqlCreator());
        if (mysqlCreator.xIsNotEmpty()) services.AddTransient<IDbProvider, TImpl>(sp => mysqlCreator());
        if (npgsqlCreator.xIsNotEmpty()) services.AddTransient<IDbProvider, TImpl>(sp => npgsqlCreator());
        
        services.AddTransient<JDbProviderResolver>(sp => implType =>
        {
            return implType switch
            {
                ENUM_DB_TYPE.Mssql => sp.GetRequiredService<JMsSqlProviderBase>(),
                ENUM_DB_TYPE.Mysql => sp.GetRequiredService<JMySqlProviderBase>(),
                ENUM_DB_TYPE.Npgsql => sp.GetRequiredService<JNpgSqlProviderBase>(),
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
    public static IServiceCollection AddJinaMssql<TImpl>(this IServiceCollection services, Func<TImpl> creator)
        where TImpl : JSqlProviderBase
    {
        services.AddTransient<IDbProvider, TImpl>(sp => creator());
        return services;
    }
    
    /// <summary>
    /// if use single database
    /// </summary>
    /// <param name="services"></param>
    /// <param name="creator"></param>
    /// <typeparam name="TImpl"></typeparam>
    /// <returns></returns>
    public static IServiceCollection AddJinaMysql<TImpl>(this IServiceCollection services, Func<TImpl> creator)
        where TImpl : JSqlProviderBase
    {
        services.AddTransient<IDbProvider, TImpl>(sp => creator());
        return services;
    }
    
    /// <summary>
    /// if use single database
    /// </summary>
    /// <param name="services"></param>
    /// <param name="creator"></param>
    /// <typeparam name="TImpl"></typeparam>
    /// <returns></returns>
    public static IServiceCollection AddJinaNpgsql<TImpl>(this IServiceCollection services, Func<TImpl> creator)
        where TImpl : JSqlProviderBase
    {
        services.AddTransient<IDbProvider, TImpl>(sp => creator());
        return services;
    }
}