using FluentValidation.Results;

namespace Jina.Base.Service.Abstract;

public interface IValidation<TRequest, TResult>
{
    IExecutor<TRequest, TResult> OnError(Action<ValidationResult> validator);
}