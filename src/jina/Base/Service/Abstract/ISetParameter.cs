using System.Transactions;
using FluentValidation;
using Jina.Validate;

namespace Jina.Base.Service.Abstract;

public interface ISetParameter<TRequest, TResult>
{
    IValidation<TRequest, TResult> SetValidator(Func<AbstractValidator<TRequest>> onValidator);

    void OnExecuted(Action<TResult> result);
}