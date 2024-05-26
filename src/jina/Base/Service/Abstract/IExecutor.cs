namespace Jina.Base.Service.Abstract;

public interface IExecutor<TRequest, out TResult>
{
    void Then(Action<TResult> then);
}