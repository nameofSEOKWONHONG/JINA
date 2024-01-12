using Jina.Base.Service.Abstract;
using Jina.Database.Abstract;

namespace Jina.Base.Service;

public abstract class ServiceImplBase<TSelf>
{
    protected IDbProviderBase DbProviderBase;
    protected IHttpClientFactory HttpClientFactory;

    protected Serilog.ILogger Logger => Serilog.Log.Logger;

    protected ServiceImplBase()
    {
        this.Logger.Debug("Create Instance : {Instance}", typeof(TSelf).Name);
    }

    protected ServiceImplBase(IDbProviderBase db)
    {
        this.DbProviderBase = db;
        this.Logger.Debug("Create Instance With IDbProvider : {Instance}", typeof(TSelf).Name);
    }

    protected ServiceImplBase(IHttpClientFactory httpClientFactory)
    {
        this.HttpClientFactory = httpClientFactory;
        this.Logger.Debug("Create Instance With IDbProvider : {Instance}", typeof(TSelf).Name);
    }

    protected ServiceImplBase(IDbProviderBase db, IHttpClientFactory httpClientFactory)
    {
        this.DbProviderBase = db;
        this.HttpClientFactory = httpClientFactory;
        this.Logger.Debug("Create Instance With IDbProvider And IPartnerAuthenticationHandler : {Instance}", typeof(TSelf).Name);
    }
}

public abstract class ServiceImplBase<TSelf, TRequest, TResult>
    : ServiceImplBase<TSelf>
        , IServiceImplBase<TRequest, TResult>
{
    protected ServiceImplBase() : base()
    {
    }

    protected ServiceImplBase(IDbProviderBase db) : base(db)
    {
    }

    protected ServiceImplBase(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {
    }

    protected ServiceImplBase(IDbProviderBase db, IHttpClientFactory httpClientFactory)
        : base(db, httpClientFactory)
    {
    }

    public abstract Task<bool> OnExecutingAsync();

    public abstract Task OnExecuteAsync();

    public TRequest Request { get; set; }
    public TResult Result { get; set; }
}