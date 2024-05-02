using System.Data;
using System.Data.Common;

namespace Jina.Database.Abstract;

public interface IDbProviderBase
{
    Task CreateAsync();
    Task BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken ct = default);
    DbConnection Connection();
    DbTransaction Transaction();
}