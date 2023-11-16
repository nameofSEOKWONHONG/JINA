using Jina.Validate;

namespace Jina.Base.Service.Abstract;

public interface IUseTransaction<TRequest, TResult>
{
    IValidation<TRequest, TResult> SetValidator(JValidator<TRequest> validator);

    Task ExecutedAsync(Action<TResult> onResult);
}