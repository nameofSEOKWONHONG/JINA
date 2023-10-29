using System.Data;

namespace Jina.Database.Abstract;

public abstract class JSqlProviderBase : IDbProvider
{
    public abstract Task<IDbConnection> GetDbConnectionAsync();
}