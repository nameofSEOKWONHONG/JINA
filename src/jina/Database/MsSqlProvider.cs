using System.Data;
using eXtensionSharp;
using Jina.Database.Abstract;
using Microsoft.Data.SqlClient;

namespace Jina.Database;

public class MsSqlProvider : DbProviderBase
{
    private readonly string _connectionString;

    public MsSqlProvider()
    {
        
    }
    
    public MsSqlProvider(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public override async Task<IDbConnection> CreateAsync()
    {
        var connection = new SqlConnection(_connectionString);
        //TODO : 만약 Repository 패턴으로 작성한다면 아래와 같이 처리되어야 한다.
        if (connection.State == ConnectionState.Closed &&
            connection.ConnectionString.xIsEmpty())
        {
            connection = new SqlConnection(_connectionString);
        }
        
        //TODO : 만약 Inserter, Updater, Deleter등으로 구분한다면 아래와 같이 처리 되어야 한다.
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        return connection;
    }
}