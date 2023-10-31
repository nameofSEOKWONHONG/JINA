using Jina.Base.Validator;

namespace Jina.Base.Service.Abstract;

public interface IUseTransaction<TRequest, TResult>
{
    IValidation<TRequest, TResult> SetValidator(JValidatorBase<TRequest> validator);
    
    Task ExecutedAsync(Action<TResult> onResult); 
}