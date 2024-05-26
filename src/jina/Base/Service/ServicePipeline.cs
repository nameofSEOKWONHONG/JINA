using System.Diagnostics;
using eXtensionSharp;
using Jina.Base.Attributes;
using Jina.Base.Service.Abstract;
using Jina.Session.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Serilog;

namespace Jina.Base.Service;

/// <summary>
/// 서비스 파이프라인. 트랜잭션은 최초의 TransactionAttribute에 따라 동작한다.
/// TransactionScope는 사용하지 않고, ADO Transaction 및 EF 트랜잭션만 사용한다.
/// Transaction은 FreeSql을 사용할 경우 FreeSql옵션을 설정해야 한다.
/// 기본은 EF Transaction이다.
/// Transaction의 최종 결과는 TransactionMiddleware에서 Commit및 Rollback 처리한다. 
/// </summary>
public class ServicePipeline : DisposeBase
{
    private readonly Queue<IServiceLoaderBase> _services = new();
    private readonly ISessionContext _sessionContext;
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    public ServicePipeline(ISessionContext context)
    {
        _sessionContext = context;
    }

    public ServiceLoader<TRequest, TResult> Register<TRequest, TResult>(IServiceImplBase<TRequest, TResult> service)
    {
        var svc = new ServiceLoader<TRequest, TResult>(service);
        _services.Enqueue(svc);
        
        return svc;
    }
    
    public async Task ExecuteAsync()
    {
        try
        {
            await ProcessQueueAsync();
        }
        catch (Exception e)
        {
            Log.Logger.Error(e, "ServiceLoader ExecuteAsync Error : {Error}", e.Message);
            throw;
        }
    }
    
    private async Task ProcessQueueAsync()
    {
        if (_services.Count > 0)
        {
            var service = _services.Dequeue();
            
            Log.Logger.Information("{ServiceName} service start", service.GetType().Name);
            
            var sw = Stopwatch.StartNew();
            var type = service.Self.GetType();
                
            var attribute =
                (TransactionOptionsAttribute)Attribute.GetCustomAttribute(type,
                    typeof(TransactionOptionsAttribute));

            if (attribute.xIsNotEmpty())
            {
                await HandleTransactionAsync(attribute);
            }

            await service.ExecuteCore();
            _sessionContext.DbContext.xAs<DbContext>().ChangeTracker.Clear();

            sw.Stop();
            Log.Logger.Information("{ServiceName} service end", service.GetType().Name);
            Log.Logger.Information("Service execute time:{Sec}", sw.Elapsed.TotalSeconds);

            await ProcessQueueAsync();
        }
    }
    
    private async Task HandleTransactionAsync(TransactionOptionsAttribute attribute)
    {
        if (attribute.DbProviderType == ENUM_DB_PROVIDER_TYPE.EF)
        {
            if (_sessionContext.DbContext.Database.CurrentTransaction.xIsEmpty())
            {
                var cts = new CancellationTokenSource(attribute.Timeout);
                await _sessionContext.DbContext.Database.BeginTransactionAsync(attribute.IsolationLevel, cts.Token).ConfigureAwait(false);
            }
            else
            {
                await _sessionContext.DbContext.Database.UseTransactionAsync(_sessionContext.DbContext.Database.CurrentTransaction!.GetDbTransaction()).ConfigureAwait(false);
            }    
        }
        else
        {
            if (_sessionContext.FSql.CurrentTransaction.xIsEmpty())
            {
                var cts = new CancellationTokenSource(attribute.Timeout);
                await _sessionContext.FSql.BeginTransactionAsync(attribute.IsolationLevel, cts.Token);
            }
        }
    }
    
    // private System.Transactions.IsolationLevel Convert(System.Data.IsolationLevel level)
    // {
    //     return level switch
    //     {
    //         System.Data.IsolationLevel.ReadUncommitted => System.Transactions.IsolationLevel.ReadUncommitted,
    //         System.Data.IsolationLevel.ReadCommitted => System.Transactions.IsolationLevel.ReadCommitted,
    //         System.Data.IsolationLevel.Serializable => System.Transactions.IsolationLevel.Serializable,
    //         System.Data.IsolationLevel.Chaos => System.Transactions.IsolationLevel.Chaos,
    //         System.Data.IsolationLevel.Unspecified => System.Transactions.IsolationLevel.Unspecified,
    //         System.Data.IsolationLevel.Snapshot => System.Transactions.IsolationLevel.Snapshot,
    //         System.Data.IsolationLevel.RepeatableRead => System.Transactions.IsolationLevel.RepeatableRead,
    //         _ => throw new NotImplementedException()
    //     };
    // }    
}