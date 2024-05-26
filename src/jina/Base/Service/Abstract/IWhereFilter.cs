using Microsoft.Extensions.Caching.Distributed;

namespace Jina.Base.Service.Abstract;

public interface IWhereFilter<TRequest, TResult>
{
    IWhereFilter<TRequest, TResult> Where(Func<bool> condition);

    ISetParameter<TRequest, TResult> WithParameter(Func<TRequest> parameter);
}