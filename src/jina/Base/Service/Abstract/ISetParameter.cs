using System.Transactions;
using Jina.Validate;

namespace Jina.Base.Service.Abstract;

public interface ISetParameter<TRequest, TResult>
{
    IValidation<TRequest, TResult> SetValidator(Validator<TRequest> validator);

    Task OnExecutedAsync(Action<TResult> result);
}