using Jina.Base.Service.Abstract;
using Jina.Database.Abstract;
using Jina.Session.Abstract;
using Microsoft.Extensions.Logging;

namespace Jina.Base.Service;

public abstract class ServiceImplCore<TSelf>
{
    protected ILogger Logger;

    protected ServiceImplCore(ILogger logger)
    {
        this.Logger = logger;
        this.Logger.LogDebug("Create Instance : {Instance}", typeof(TSelf).Name);
    }

}

public abstract class ServiceImplCore<TSelf, TRequest, TResult>
    : ServiceImplCore<TSelf>
{
    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="logger"></param>
    protected ServiceImplCore(ILogger<TSelf> logger) : base(logger)
    {
    }

    public abstract Task<bool> OnExecutingAsync();

    public abstract Task OnExecuteAsync();

    public IServiceImplBase<TRequest, TResult> Self { get; init; }
    public TRequest Request { get; set; }
    public TResult Result { get; set; }
}