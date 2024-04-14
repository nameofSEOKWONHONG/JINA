using Jina.Base.Service.Abstract;
using Jina.Database.Abstract;

namespace Jina.Base.Service;

public abstract class ServiceImplCore<TSelf>
{
    protected Serilog.ILogger Logger => Serilog.Log.Logger;

    protected ServiceImplCore()
    {
        this.Logger.Debug("Create Instance : {Instance}", typeof(TSelf).Name);
    }

}

public abstract class ServiceImplCore<TSelf, TRequest, TResult>
    : ServiceImplCore<TSelf>, IServiceImplBase<TRequest, TResult>
{
    
    protected ServiceImplCore() : base()
    {
    }

    public abstract Task OnExecutingAsync();

    public abstract Task OnExecuteAsync();

    public IServiceImplBase<TRequest, TResult> Self { get; init; }
    public TRequest Request { get; set; }
    public TResult Result { get; set; }
}