using System.Transactions;
using FluentValidation;
using Jina.Validate;
using Microsoft.Extensions.Caching.Distributed;

namespace Jina.Base.Service.Abstract;

public interface ISetParameter<TRequest, TResult>
{
    ISetParameter<TRequest, TResult> UseCache(string cacheKey = null, DistributedCacheEntryOptions cacheEntryOptions = null);
    
    IValidation<TRequest, TResult> SetValidator(Func<AbstractValidator<TRequest>> onValidator);

    void OnExecuted(Action<TResult> result);
}