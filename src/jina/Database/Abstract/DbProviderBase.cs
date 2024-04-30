using System.Data;

namespace Jina.Database.Abstract;

public abstract class DbProviderBase : IDbProviderBase
{
    public abstract Task<IDbConnection> CreateAsync();
}