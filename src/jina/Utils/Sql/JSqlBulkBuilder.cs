using System.Collections.Concurrent;
using System.Data;
using System.Reflection;
using Dapper;
using eXtensionSharp;
using Jina.Base.Data;
using Jina.Utils.Pooling;

namespace Jina.Utils.Sql;

public class JSqlBulkBuilder<T>
    where T : class
{
    private readonly IDbConnection _connection;
    
    private const int BATCH_SIZE = 500;

    internal JSqlBulkBuilder(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task BulkInsertAsync(string schema, string tableName, T[] items)
    {
        if (items.xIsEmpty()) return;
        
        for (var i = 0; i < items.Length; i++)
        {
            var item = items[i];
        }
        
        var batchItems = items.xBatch(BATCH_SIZE);
        foreach (T[] batchItem in batchItems)
        {
            var sql = CreateSql(schema, tableName, batchItem);
            await _connection.ExecuteAsync(sql);
        }
    }

    public Task BulkInsertAsync<TEntity>(T[] items)
    {
        var tableinfo = AssignSchemaAndTable();
        return this.BulkInsertAsync(tableinfo.schemaName, tableinfo.tableName, items);
    }
    
    private string CreateSql(string schema, string tableName, T[] items)
    {
        var columns = AssignColumn(items.First());
        var datum = AssignDatum(columns, items);

        var sql = " SET NOCOUNT ON; " + Environment.NewLine +
                  $" INSERT INTO [{schema}].[{tableName}] ({columns.xJoin()}) " + Environment.NewLine +
                  $" VALUES {datum.xJoin(Environment.NewLine)} " + Environment.NewLine +
                  " SET NOCOUNT OFF; ";

        return sql;
    }

    #region [function]

    private static ConcurrentDictionary<Type, string[]> _assignColumnStates = new();

    private string[] AssignColumn(T item)
    {
        Type itemType = typeof(T);
        if (_assignColumnStates.TryGetValue(itemType, out string[] cachedColumns))
        {
            return cachedColumns;
        }
        var columns = new List<string>();
        var props = item.xGetProperties();
        props.xForEach(prop =>
        {
            columns.Add(prop.Name);
            return true;
        });
        _assignColumnStates.TryAdd(itemType, columns.ToArray());
        return columns.ToArray();
    }
    
    private static readonly ConcurrentDictionary<Type, (string schemaName, string tableName)> _assignTableStates = new();

    private (string schemaName, string tableName) AssignSchemaAndTable()
    {
        Type itemType = typeof(T);
        if (_assignTableStates.TryGetValue(itemType, out (string schemaName, string tableName) cachedResult))
        {
            return cachedResult;
        }
        var result = XReflectionExtentions.xGetSchemaAndTableName<T>();
        _assignTableStates.TryAdd(itemType, result);
        return result;
    }    
    
    private static readonly Dictionary<string, Func<PropertyInfo, object, string>> _datumStates = new()
    {
        {
            //s += $"DECLARE @{i}{j} NVARCHAR(MAX) = '{item2.GetValue(item)}'" + Environment.NewLine;    
            DataTypeName.String, (prop, item) => $"N'{prop.GetValue(item).xValue<string>()}',"
        },
        {
            DataTypeName.DateTime, (prop, item) =>
            {
                var v = prop.GetValue(item).xValue<DateTime>();
                if (v.xIsEmpty()) return $"NULL, ";
                return $"N'{v.ToString(ENUM_DATE_FORMAT.YYYY_MM_DD_HH_MM_SS_FFF)}', ";
            }
        },
        {
            DataTypeName.NullableDateTime, (prop, item) =>
            {
                var v = prop.GetValue(item).xValue<DateTime?>();
                if (v.xIsEmpty()) return $"NULL, ";
                return $"N'{v!.Value}', ";
            }
        },
        {
            DataTypeName.Boolean, (prop, item) => $"{prop.GetValue(item).xValue<int>(0)}, "
        },
        {
            "Default", (prop, item) => $"{prop.GetValue(item).xValue<string>("0")}, "
        }
    };    
    
    private IEnumerable<string> AssignDatum(string[] columns, T[] items)
    {
        var sbPool = JStringBuilderPool.Create(items.Length);
        
        var result = new List<string>();
        items.xForEach((item, i) =>
        {
            var props = item.xGetProperties();
            var statement = sbPool.Rent();
            statement.Append("(");            
            props.xForEach((prop, j) =>
            {
                var exist = columns.xFirst(m => m == prop.Name);
                if (exist.xIsEmpty()) return true;

                if(_datumStates.TryGetValue(prop.PropertyType.ToString(), out Func<PropertyInfo, object, string> x))
                {
                    // ReSharper disable once AccessToModifiedClosure
                    statement.Append(x(prop, item));
                }
                else
                {
                    statement.Append(_datumStates["Default"](prop, item));
                }
                return true;
            });
            var valueSql = statement.ToString();
            valueSql = valueSql.Substring(0, valueSql.LastIndexOf(','));
            if (i < items.Count() - 1)
            {
                valueSql += " ), " + Environment.NewLine;
            }
            else
            {
                valueSql += " ); " + Environment.NewLine;
            }
            result.Add(valueSql);
        });
        return result.ToArray();
    }         

    #endregion

}

public static class SqlBulkBuilderExtensions
{
    public static JSqlBulkBuilder<T> CreateSqlBulkBuilder<T>(this IDbConnection db)
        where T : BulkBase
    {
        return new JSqlBulkBuilder<T>(db);
    }
}