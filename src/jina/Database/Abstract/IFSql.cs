using System.Data;
using System.Data.Common;
using FreeSql;

namespace Jina.Database.Abstract;

public interface IFSql
{
    IFreeSql FreeSql { get; }
    ISelect<TEntity> Select<TEntity>() where TEntity : class;
    IInsert<TEntity> Insert<TEntity>() where TEntity : class;
    IUpdate<TEntity> Update<TEntity>() where TEntity : class;
    IDelete<TEntity> Delete<TEntity>() where TEntity : class;
    
    DbTransaction CurrentTransaction { get; }
    Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted, CancellationToken ct = default);
}