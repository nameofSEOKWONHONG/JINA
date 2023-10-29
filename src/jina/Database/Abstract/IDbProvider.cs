using System.Data;

namespace Jina.Database.Abstract;

public interface IDbProvider
{
    Task<IDbConnection> GetDbConnectionAsync();
}