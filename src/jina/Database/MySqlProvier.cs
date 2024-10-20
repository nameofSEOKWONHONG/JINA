// using System.Data;
// using System.Data.Common;
// using eXtensionSharp;
// using Jina.Database.Abstract;
// using MySql.Data.MySqlClient;
//
// namespace Jina.Database;
//
// public class MySqlProvider : DbProviderBase
// {   
//     public MySqlProvider(string connectionString)
//     {
//         this.ConnectionString = connectionString;
//         this.DbConnection = new MySqlConnection(this.ConnectionString);
//     }
//     
//     public override async Task CreateAsync()
//     {
//         //TODO : 만약 Inserter, Updater, Deleter등으로 구분한다면 아래와 같이 처리 되어야 한다.
//         if (this.DbConnection.State != ConnectionState.Open)
//         {
//             await this.DbConnection.OpenAsync();
//         }
//     }
//
//     public override async Task BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken ct = default)
//     {
//         if (this.DbConnection.xIsEmpty()) throw new Exception("Connection is not init");
//         if (this.DbTransaction.xIsEmpty())
//         {
//             this.DbTransaction = await this.DbConnection.BeginTransactionAsync(isolationLevel, ct);
//         }
//     }
//
//     public override DbConnection Connection()
//     {
//         return this.DbConnection;
//     }
//
//     public override DbTransaction Transaction()
//     {
//         return this.DbTransaction;
//     }
// }