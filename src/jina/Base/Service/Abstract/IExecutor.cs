namespace Jina.Base.Service.Abstract;

public interface IExecutor<TRequest, out TResult>
{
    Task ExecutedAsync(Action<TResult> onResult);
}