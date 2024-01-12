using Jina.Database;
using Jina.Database.Abstract;
using Serilog;

namespace Jina.Base.Repository;

public abstract class JRepositoryBase
{
    protected readonly ILogger Logger = Log.Logger;
    protected readonly IDbProvider DbProvider;
    protected readonly JDbProviderResolver DbProviderResolver;
    
    /// <summary>
    /// 만약, 모든 Db를 사용한다면 JDbProviderResolver를 사용하고 하나의 db만 사용한다면, IDbProvider를 사용한다.
    /// </summary>
    /// <param name="db"></param>
    protected JRepositoryBase(IDbProvider db)
    {
        DbProvider = db;
    }

    /// <summary>
    /// 만약, 모든 Db를 사용한다면 JDbProviderResolver를 사용하고 하나의 db만 사용한다면, IDbProvider를 사용한다.
    /// </summary>
    /// <param name="resolver"></param>
    protected JRepositoryBase(JDbProviderResolver resolver)
    {
        this.DbProviderResolver = resolver;
    }
}