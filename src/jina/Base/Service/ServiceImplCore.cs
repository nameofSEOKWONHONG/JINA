using Jina.Base.Service.Abstract;
using Jina.Database.Abstract;

namespace Jina.Base.Service;

public abstract class ServiceImplCore<TSelf>
{
    protected IDbProviderBase DbProviderBase;
    protected IHttpClientFactory HttpClientFactory;

    protected Serilog.ILogger Logger => Serilog.Log.Logger;

    protected ServiceImplCore()
    {
        this.Logger.Debug("Create Instance : {Instance}", typeof(TSelf).Name);
    }

    protected ServiceImplCore(IDbProviderBase db)
    {
        this.DbProviderBase = db;
        this.Logger.Debug("Create Instance With IDbProvider : {Instance}", typeof(TSelf).Name);
    }

    protected ServiceImplCore(IHttpClientFactory httpClientFactory)
    {
        this.HttpClientFactory = httpClientFactory;
        this.Logger.Debug("Create Instance With IDbProvider : {Instance}", typeof(TSelf).Name);
    }

    protected ServiceImplCore(IDbProviderBase db, IHttpClientFactory httpClientFactory)
    {
        this.DbProviderBase = db;
        this.HttpClientFactory = httpClientFactory;
        this.Logger.Debug("Create Instance With IDbProvider And IPartnerAuthenticationHandler : {Instance}", typeof(TSelf).Name);
    }
}

public abstract class ServiceImplCore<TSelf, TRequest, TResult>
    : ServiceImplCore<TSelf>
        , IServiceImplBase<TRequest, TResult>
{
    protected ServiceImplCore() : base()
    {
    }

    protected ServiceImplCore(IDbProviderBase db) : base(db)
    {
    }

    protected ServiceImplCore(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {
    }

    protected ServiceImplCore(IDbProviderBase db, IHttpClientFactory httpClientFactory)
        : base(db, httpClientFactory)
    {
    }

    public abstract Task<bool> OnExecutingAsync();

    public abstract Task OnExecuteAsync();

    public TRequest Request { get; set; }
    public TResult Result { get; set; }
}