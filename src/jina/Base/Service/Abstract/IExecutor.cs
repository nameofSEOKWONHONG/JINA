namespace Jina.Base.Service.Abstract;

public interface IExecutor<TRequest, out TResult>
{
    Task OnExecutedAsync(Action<TResult> onResult);
}