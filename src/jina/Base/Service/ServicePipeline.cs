using System.Diagnostics;
using eXtensionSharp;
using Jina.Base.Attributes;
using Jina.Base.Service.Abstract;
using Jina.Session.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Serilog;
using IsolationLevel = System.Transactions.IsolationLevel;

namespace Jina.Base.Service;

public class ServicePipeline : DisposeBase
{
    private readonly Queue<ServiceLoaderBase> _services = new();
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
        if (_services.All(m => m.Self != service))
            _services.Enqueue(svc);
        
        return svc;
    }
    
    public async Task ExecuteAsync()
    {
        try
        {
            while (_services.Count > 0)
            {
                var service = _services.Dequeue();
                var sw = Stopwatch.StartNew();

                #region [don't use this code]

                var type = service.Self.GetType();
                
                var attribute =
                    (TransactionOptionsAttribute)Attribute.GetCustomAttribute(type,
                        typeof(TransactionOptionsAttribute));

                if (attribute.xIsNotEmpty())
                {
                    // if (attribute.TransactionDbType == ENUM_TRANSACTION_DB_TYPE.EF)
                    {
                        _ = _sessionContext.DbContext.Database.CurrentTransaction.xIsEmpty() 
                            ? await _sessionContext.DbContext.Database.BeginTransactionAsync(attribute!.IsolationLevel
                                , _sessionContext.CancellationToken) 
                            : await _sessionContext.DbContext.Database.UseTransactionAsync(_sessionContext.DbContext.Database.CurrentTransaction?.GetDbTransaction());                        
                    }
                    // else
                    // {
                    //     await _sessionContext.DbProvider.CreateAsync();
                    //     await _sessionContext.DbProvider.BeginTransactionAsync(attribute.IsolationLevel,
                    //         _sessionContext.CancellationToken);
                    // }
                }
                
                #endregion

                try
                {
                    await service.ExecuteCore();
                    _sessionContext.DbContext.xAs<DbContext>().ChangeTracker.Clear();
                }
                catch (Exception e)
                {
                    Log.Logger.Error(e, "ServicePipeline Error : {Message}", e.Message);
                    throw;
                }

                sw.Stop();
                Log.Logger.Information("execute service time : {time}", sw.Elapsed.TotalSeconds);
            }
        }
        catch (Exception e)
        {
            Log.Logger.Error(e, "ServiceLoader OnExecuted Error : {Error}", e.Message);
            throw;
        }
    }
    
    private System.Transactions.IsolationLevel Convert(System.Data.IsolationLevel level)
    {
        return level switch
        {
            System.Data.IsolationLevel.ReadUncommitted => System.Transactions.IsolationLevel.ReadUncommitted,
            System.Data.IsolationLevel.ReadCommitted => System.Transactions.IsolationLevel.ReadCommitted,
            System.Data.IsolationLevel.Serializable => System.Transactions.IsolationLevel.Serializable,
            System.Data.IsolationLevel.Chaos => System.Transactions.IsolationLevel.Chaos,
            System.Data.IsolationLevel.Unspecified => System.Transactions.IsolationLevel.Unspecified,
            System.Data.IsolationLevel.Snapshot => System.Transactions.IsolationLevel.Snapshot,
            System.Data.IsolationLevel.RepeatableRead => IsolationLevel.RepeatableRead,
            _ => throw new NotImplementedException()
        };
    }    
}