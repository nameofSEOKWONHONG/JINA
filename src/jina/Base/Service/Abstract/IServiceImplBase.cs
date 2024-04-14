namespace Jina.Base.Service.Abstract;

public interface IServiceImplBase
{
    Task OnExecutingAsync();
    Task OnExecuteAsync();
}

public interface IServiceImplBase<TRequest, TResult> : IServiceImplBase
{
    IServiceImplBase<TRequest, TResult> Self { get; }
    TRequest Request { get; set; }
    TResult Result { get; set; }
}