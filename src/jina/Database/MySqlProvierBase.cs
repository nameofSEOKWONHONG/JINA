using System.Data;
using eXtensionSharp;
using Jina.Database.Abstract;
using MySql.Data.MySqlClient;

namespace Jina.Database;

public class MySqlProviderBase : DbProviderBase
{
    private MySqlConnection _mySqlConnection;
    private readonly string _connectionString;
    
    protected MySqlProviderBase(string connectionString)
    {
        _mySqlConnection = new MySqlConnection(connectionString);
        _connectionString = connectionString;
    }

    protected MySqlProviderBase(MySqlConnection connection)
    {
        _mySqlConnection = connection;
        _connectionString = _mySqlConnection.ConnectionString;
    }
    
    public override async Task<IDbConnection> GetDbConnectionAsync()
    {
        //TODO : 만약 Repository 패턴으로 작성한다면 아래와 같이 처리되어야 한다.
        if (_mySqlConnection.State == ConnectionState.Closed &&
            _mySqlConnection.ConnectionString.xIsEmpty())
        {
            _mySqlConnection = new MySqlConnection(_connectionString);
        }
        
        //TODO : 만약 Inserter, Updater, Deleter등으로 구분한다면 아래와 같이 처리 되어야 한다.
        if (_mySqlConnection.State != ConnectionState.Open)
        {
            await _mySqlConnection.OpenAsync();
        }

        return _mySqlConnection;
    }
}