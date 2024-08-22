using Microsoft.Extensions.Caching.Distributed;

namespace Jina.Base.Service.Abstract;

public interface IWhenFilter<TRequest, TResult>
{
    IWhenFilter<TRequest, TResult> When(Func<bool> condition);

    ISetParameter<TRequest, TResult> WithParameter(Func<TRequest> parameter);
}