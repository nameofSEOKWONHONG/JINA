using System.Data;
using Jina.Database.Abstract;
using Npgsql;

namespace Jina.Database;

public class JNpgSqlProviderBase : JSqlProviderBase
{
    private readonly NpgsqlDataSourceBuilder _dataSourceBuilder;
    private readonly NpgsqlDataSource _dataSource;
    private readonly string _connectionString;
    
    protected JNpgSqlProviderBase(NpgsqlDataSourceBuilder dataSourceBuilder)
    {
        // var connStringBuilder = new NpgsqlConnectionStringBuilder
        // {
        //     CommandTimeout = 30, // 명령 실행 제한 시간 (초)
        //     Timeout = 15, // 연결 제한 시간 (초)
        //     Pooling = true, // 연결 풀링 사용
        //     MinPoolSize = 1, // 최소 연결 풀 크기
        //     MaxPoolSize = 20 // 최대 연결 풀 크기        
        // };
        _dataSourceBuilder = dataSourceBuilder;
        _dataSourceBuilder.ConnectionStringBuilder.Pooling = true;
        _dataSource = _dataSourceBuilder.Build();
        _connectionString = _dataSource.ConnectionString;
    }

    
    protected JNpgSqlProviderBase(string connectionString)
    {
        _dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        _dataSource = _dataSourceBuilder.Build();
        _connectionString = connectionString;
    }
    
    public override async Task<IDbConnection> GetDbConnectionAsync()
    {
        var conn = await _dataSource.OpenConnectionAsync();
        if (conn.State != ConnectionState.Open)
        {
            await conn.OpenAsync();
        }

        return conn;
    }
}