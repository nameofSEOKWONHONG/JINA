using System.Transactions;
using FluentValidation.Results;
using Jina.Base.Validator;

namespace Jina.Base.Service.Abstract;

public interface IAddFilter<TRequest, TResult>
{
    IAddFilter<TRequest, TResult> AddFilter(Func<bool> onFilter);

    ISetParameter<TRequest, TResult> SetParameter(Func<TRequest> onParameter);
}

public interface ISetParameter<TRequest, TResult>
{
    IUseTransaction<TRequest, TResult> UseTransaction(TransactionScopeOption option = TransactionScopeOption.Required,
        System.Transactions.IsolationLevel isolationLevel = System.Transactions.IsolationLevel.ReadUncommitted);
    
    IValidation<TRequest, TResult> SetValidator(JValidatorBase<TRequest> validator);
    
    Task ExecutedAsync(Action<TResult> onResult);    
}

public interface IUseTransaction<TRequest, TResult>
{
    IValidation<TRequest, TResult> SetValidator(JValidatorBase<TRequest> validator);
    
    Task ExecutedAsync(Action<TResult> onResult); 
}

public interface IValidation<TRequest, TResult>
{
    IExecutor<TRequest, TResult> OnValidated(Action<ValidationResult> validateBehavior);
}

public interface IExecutor<TRequest, out TResult>
{
    Task ExecutedAsync(Action<TResult> onResult);
}