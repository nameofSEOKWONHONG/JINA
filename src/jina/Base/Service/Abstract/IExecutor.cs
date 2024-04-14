namespace Jina.Base.Service.Abstract;

public interface IExecutor<TRequest, out TResult>
{
    void OnExecuted(Action<TResult> result);
}