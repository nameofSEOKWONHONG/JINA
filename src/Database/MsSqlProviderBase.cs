using System.Data;
using eXtensionSharp;
using Jina.Database.Abstract;
using Microsoft.Data.SqlClient;

namespace Jina.Database;

public abstract class MsSqlProviderBase : IDbProvider
{
    private SqlConnection _sqlConnection;
    private readonly string _connectionString;
    protected MsSqlProviderBase(string connectionString)
    {
        _sqlConnection = new SqlConnection(connectionString);
        _connectionString = connectionString;
    }

    protected MsSqlProviderBase(SqlConnection connection)
    {
        _sqlConnection = connection;
        _connectionString = _sqlConnection.ConnectionString;
    }
    
    public async Task<IDbConnection> GetDbConnectionAsync()
    {
        //TODO : 만약 Repository 패턴으로 작성한다면 아래와 같이 처리되어야 한다.
        if (_sqlConnection.State == ConnectionState.Closed &&
            _sqlConnection.ConnectionString.xIsEmpty())
        {
            _sqlConnection = new SqlConnection(_connectionString);
        }
        
        //TODO : 만약 Inserter, Updater, Deleter등으로 구분한다면 아래와 같이 처리 되어야 한다.
        if (_sqlConnection.State != ConnectionState.Open)
        {
            await _sqlConnection.OpenAsync();
        }

        return _sqlConnection;
    }
}