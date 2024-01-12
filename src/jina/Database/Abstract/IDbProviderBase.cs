using System.Data;

namespace Jina.Database.Abstract;

public interface IDbProviderBase
{
    Task<IDbConnection> GetDbConnectionAsync();
}