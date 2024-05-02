using System.Data;
using System.Data.Common;
using eXtensionSharp;
using Jina.Base;

namespace Jina.Database.Abstract;

public abstract class DbProviderBase : DisposeBase, IDbProviderBase
{
    protected string ConnectionString;
    protected DbConnection DbConnection;
    protected DbTransaction DbTransaction;
    
    public abstract Task CreateAsync();
    public abstract Task BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken ct = default);
    public abstract DbConnection Connection();
    public abstract DbTransaction Transaction();
    protected override async void Dispose(bool disposing)
    {
        if (this.DbTransaction.xIsNotEmpty())
        {
            await this.DbTransaction.DisposeAsync();    
        }

        if (this.DbConnection.xIsNotEmpty())
        {
            await this.DbConnection.CloseAsync();
            await this.DbConnection.DisposeAsync();            
        }
    }
}