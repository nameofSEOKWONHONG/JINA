using System.Data;
using System.Data.Common;
using eXtensionSharp;
using Jina.Database.Abstract;
using Microsoft.Data.SqlClient;

namespace Jina.Database;

public class MsSqlProvider : DbProviderBase
{
    public MsSqlProvider()
    {
        
    }
    
    public MsSqlProvider(string connectionString)
    {
        this.ConnectionString = connectionString;
        this.DbConnection = new SqlConnection(connectionString);
    }
    
    public override async Task CreateAsync()
    {
        if (DbConnection.State != ConnectionState.Open)
        {
            await DbConnection.OpenAsync();
        }
    }

    public override async Task BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken ct = default)
    {
        if (DbConnection.xIsEmpty()) throw new Exception("Connection not init");
        if (this.DbTransaction.xIsEmpty())
        {
            DbTransaction = await DbConnection.BeginTransactionAsync(isolationLevel, ct);    
        }
    }

    public override DbConnection Connection() => DbConnection;
    public override DbTransaction Transaction() => DbTransaction;
}