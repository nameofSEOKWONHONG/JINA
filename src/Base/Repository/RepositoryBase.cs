using Jina.Database.Abstract;
using Serilog;

namespace Jina.Base.Repository;

public abstract class RepositoryBase
{
    protected readonly ILogger Logger = Log.Logger;
    protected readonly IDbProvider DbProvider;
    protected RepositoryBase(IDbProvider db)
    {
        DbProvider = db;
    }
}