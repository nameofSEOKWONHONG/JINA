using System.Transactions;
using FluentValidation;
using Jina.Validate;
using Microsoft.Extensions.Caching.Distributed;

namespace Jina.Base.Service.Abstract;

public interface ISetParameter<TRequest, TResult>
{
    ISetParameter<TRequest, TResult> WithCache(string cacheKey = null, DistributedCacheEntryOptions cacheEntryOptions = null);
    
    IValidation<TRequest, TResult> WithValidator(Func<AbstractValidator<TRequest>> validate);

    void Then(Action<TResult> then);
}