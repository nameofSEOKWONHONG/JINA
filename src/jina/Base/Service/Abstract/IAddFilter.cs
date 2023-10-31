using System.Transactions;
using FluentValidation.Results;
using Jina.Base.Validator;

namespace Jina.Base.Service.Abstract;

public interface IAddFilter<TRequest, TResult>
{
    IAddFilter<TRequest, TResult> AddFilter(Func<bool> onFilter);

    ISetParameter<TRequest, TResult> SetParameter(Func<TRequest> onParameter);
}







