using System.Diagnostics;
using eXtensionSharp;
using Jina.Base.Service.Abstract;
using Jina.Database.Abstract;
using Jina.Session.Abstract;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Jina.Base.Service;

public class ServicePipeline : DisposeBase
{
    private readonly Queue<ServiceLoaderBase> _services = new Queue<ServiceLoaderBase>();
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

    private bool _isCommited = false;
    public async Task ExecuteAsync()
    {
        try
        {
            while (_services.Count > 0)
            {
                var service = _services.Dequeue();
                var sw = Stopwatch.StartNew();

                #region [don't use this code]

                // var type = service.Self.GetType();
                // var attribute =
                //     (TransactionOptionsAttribute)Attribute.GetCustomAttribute(type,
                //         typeof(TransactionOptionsAttribute));                

                #endregion

                await service.ExecuteCore();
                _sessionContext.DbContext.xAs<DbContext>().ChangeTracker.Clear();

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
}