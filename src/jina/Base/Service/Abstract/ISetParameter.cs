using System.Transactions;
using Jina.Base.Validator;

namespace Jina.Base.Service.Abstract;

public interface ISetParameter<TRequest, TResult>
{
    IUseTransaction<TRequest, TResult> UseTransaction(TransactionScopeOption option = TransactionScopeOption.Required,
        System.Transactions.IsolationLevel isolationLevel = System.Transactions.IsolationLevel.ReadUncommitted);
    
    IValidation<TRequest, TResult> SetValidator(JValidatorBase<TRequest> validator);
    
    Task ExecutedAsync(Action<TResult> onResult);    
}