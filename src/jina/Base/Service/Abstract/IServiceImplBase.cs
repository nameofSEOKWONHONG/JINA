using Jina.Session.Abstract;

namespace Jina.Base.Service.Abstract;

public interface IServiceImplBase
{
    ISessionContext Context { get; }
    Task<bool> OnExecutingAsync();
    Task OnExecuteAsync();
}

public interface IServiceImplBase<TRequest, TResult> : IServiceImplBase
{
    IServiceImplBase<TRequest, TResult> Self { get; }
    TRequest Request { get; set; }
    TResult Result { get; set; }
}