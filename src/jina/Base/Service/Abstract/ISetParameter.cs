using System.Transactions;
using Jina.Validate;

namespace Jina.Base.Service.Abstract;

public interface ISetParameter<TRequest, TResult>
{
    IUseTransaction<TRequest, TResult> UseTransaction(TransactionScopeOption option = TransactionScopeOption.Required,
        System.Transactions.IsolationLevel isolationLevel = System.Transactions.IsolationLevel.ReadUncommitted);

    IValidation<TRequest, TResult> SetValidator(RuleValidator<TRequest> validator);

    Task OnExecutedAsync(Action<TResult> onResult);
}