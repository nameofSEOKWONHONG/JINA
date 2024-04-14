using Jina.Validate;

namespace Jina.Base.Service.Abstract;

public interface IUseTransaction<TRequest, TResult>
{
    IValidation<TRequest, TResult> SetValidator(Validator<TRequest> validator);

    void OnExecuted(Action<TResult> result);
}