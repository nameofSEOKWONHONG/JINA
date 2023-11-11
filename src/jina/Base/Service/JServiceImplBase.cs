using Jina.Base.Service.Abstract;
using Jina.Database.Abstract;

namespace Jina.Base.Service;

public abstract class JServiceImplBase<TSelf>
{
    protected IDbProvider Db;
    protected IHttpClientFactory HttpClientFactory;
    
    protected Serilog.ILogger Logger => Serilog.Log.Logger;
    
    protected JServiceImplBase()
    {
        this.Logger.Debug("Create Instance : {Instance}", typeof(TSelf).Name);
    }

    protected JServiceImplBase(IDbProvider db)
    {
        this.Db = db;
        this.Logger.Debug("Create Instance With IDbProvider : {Instance}", typeof(TSelf).Name);
    }

    protected JServiceImplBase(IHttpClientFactory httpClientFactory)
    {
        this.HttpClientFactory = httpClientFactory;
        this.Logger.Debug("Create Instance With IDbProvider : {Instance}", typeof(TSelf).Name);
    }
    
    protected JServiceImplBase(IDbProvider db, IHttpClientFactory httpClientFactory)
    {
        this.Db = db;
        this.HttpClientFactory = httpClientFactory;
        this.Logger.Debug("Create Instance With IDbProvider And IPartnerAuthenticationHandler : {Instance}", typeof(TSelf).Name);
    }
}

public abstract class JServiceImplBase<TSelf, TRequest, TResult> 
    : JServiceImplBase<TSelf>
        , IServiceImplBase<TRequest, TResult>
{   
    protected JServiceImplBase() : base()
    {
        
    }

    protected JServiceImplBase(IDbProvider db) : base(db)
    {
        
    }
    

    protected JServiceImplBase(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {
        
    }
    
    protected JServiceImplBase(IDbProvider db, IHttpClientFactory httpClientFactory) 
        : base(db, httpClientFactory)
    {
        
    }
    
    public abstract Task<bool> OnExecutingAsync();

    public abstract Task OnExecuteAsync();

    public TRequest Request { get; set; }
    public TResult Result { get; set; }
}