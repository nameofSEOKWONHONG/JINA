using System.Data;
using System.Data.Common;
using eXtensionSharp;
using Jina.Database.Abstract;
using Npgsql;

namespace Jina.Database;

public class NpgSqlProvider : DbProviderBase
{   
    // public NpgSqlProvider(NpgsqlDataSourceBuilder dataSourceBuilder)
    // {
    //     // var connStringBuilder = new NpgsqlConnectionStringBuilder
    //     // {
    //     //     CommandTimeout = 30, // 명령 실행 제한 시간 (초)
    //     //     Timeout = 15, // 연결 제한 시간 (초)
    //     //     Pooling = true, // 연결 풀링 사용
    //     //     MinPoolSize = 1, // 최소 연결 풀 크기
    //     //     MaxPoolSize = 20 // 최대 연결 풀 크기        
    //     // };
    //     _dataSourceBuilder = dataSourceBuilder;
    //     _dataSourceBuilder.ConnectionStringBuilder.Pooling = true;
    //     _dataSource = _dataSourceBuilder.Build();
    //     _connectionString = _dataSource.ConnectionString;
    // }

    
    protected NpgSqlProvider(string connectionString)
    {
        this.ConnectionString = connectionString;
        this.DbConnection = new NpgsqlConnection(connectionString);
    }
    
    public override async Task CreateAsync()
    {
        if (this.DbConnection.State != ConnectionState.Open)
        {
            await this.DbConnection.OpenAsync();
        }
    }

    public override async Task BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken ct = default)
    {
        if (this.DbTransaction.xIsEmpty())
        {
            this.DbTransaction = await this.DbConnection.BeginTransactionAsync(isolationLevel, ct);
        }
    }

    public override DbConnection Connection()
    {
        return this.DbConnection;
    }

    public override DbTransaction Transaction()
    {
        return this.DbTransaction;
    }
}