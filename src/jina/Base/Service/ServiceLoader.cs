using eXtensionSharp;
using FluentValidation;
using FluentValidation.Results;
using Jina.Base.Service.Abstract;
using Microsoft.Extensions.Caching.Distributed;

namespace Jina.Base.Service;


/// <summary>
/// ServiceLoader class definition with generic types TRequest and TResult. It implements interfaces for adding filters, setting parameters, validation, and execution.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResult"></typeparam>
public class ServiceLoader<TRequest, TResult> : ServiceLoaderBase
    , IAddFilter<TRequest, TResult>
    , ISetParameter<TRequest, TResult>
    , IValidation<TRequest, TResult>
    , IExecutor<TRequest, TResult>
{
    private readonly IServiceImplBase<TRequest, TResult> _service;

    #region [action behavior's]

    private List<Func<bool>> _filters = new();
    private Func<TRequest> _parameter;
    private Func<AbstractValidator<TRequest>> _onValidator;
    private Action<ValidationResult> _validateBehavior;
    private Action<TResult> _onResult;

    #endregion [action behavior's]

    #region [cache]

    private IDistributedCache _cache;
    private string _cacheKey;
    private DistributedCacheEntryOptions _cacheEntryOptions;
    private bool _useCache;
    #endregion [cache]

    internal ServiceLoader(IServiceImplBase<TRequest, TResult> service)
    {
        _service = service;
        this.Self = _service;
    }

    public IAddFilter<TRequest, TResult> AddFilter(Func<bool> onFilter)
    {
        this._filters.Add(onFilter);
        return this;
    }

    public ISetParameter<TRequest, TResult> SetParameter(Func<TRequest> onParameter)
    {
        this._parameter = onParameter;
        return this;
    }


    public ISetParameter<TRequest, TResult> UseCache(string cacheKey = null, DistributedCacheEntryOptions cacheEntryOptions = null)
    {
        this._cacheKey = cacheKey;
        this._useCache = true;
        return this;
    }

    public IValidation<TRequest, TResult> SetValidator(Func<AbstractValidator<TRequest>> onValidator)
    {
        _onValidator = onValidator;
        return this;
    }

    public IExecutor<TRequest, TResult> OnValidated(Action<ValidationResult> validateBehavior)
    {
        _validateBehavior = validateBehavior;
        return this;
    }

    public void OnExecuted(Action<TResult> onResult)
    {
        _onResult = onResult;
    }

    #region [execute core]

    public override async Task ExecuteCore()
    {
        #region [use cache]

        if (_useCache)
        {
            //캐시에 대한 키 설정이 없을 경우 Request를 기본으로 한다.
            if (_cacheKey.xIsEmpty())
            {
                _service.Result = await GetCacheAsync(_service.Request.xToJson().xGetHashCode());
            }
            else
            {
                _service.Result = await GetCacheAsync(_cacheKey);
            }
        }

        if (_service.Result.xIsNotEmpty())
        {
            if (_onResult.xIsNotEmpty())
            {
                _onResult(_service.Result);
                return;
            }
        }        

        #endregion

        #region [use validate]

        if (InvokedFilter().xIsFalse()) return;

        InvokedParameter();

        var valid = await InvokedValidatingAsync(_service.Request);
        if (valid.xIsFalse()) return;        

        #endregion

        await ExecuteAsync(_onResult);

        #region [use cache]

        if (_useCache)
        {
            await SetCacheAsync(_cacheKey, _service.Result.xToBytes(), _cacheEntryOptions);
        }        

        #endregion
    }

    private bool InvokedFilter()
    {
        var filterValid = true;
        _filters.xForEachSpan(filter =>
        {
            filterValid = filter.Invoke();
            if (filterValid.xIsFalse()) return;
        });

        return filterValid;
    }

    private void InvokedParameter()
    {
        TRequest parameter = default(TRequest);

        if (_parameter.xIsNotEmpty())
        {
            parameter = _parameter.Invoke();
            _service.Request = parameter;
        }
    }

    private async Task<bool> InvokedValidatingAsync(TRequest parameter)
    {
        if (_onValidator.xIsNotEmpty())
        {
            var rs = await _onValidator().ValidateAsync(parameter);
            if (rs.IsValid.xIsFalse())
            {
                _validateBehavior(rs);
            }

            return rs.IsValid;
        }

        return true;
    }

    private async Task<TResult> GetCacheAsync(string key)
    {
        if (key.xIsEmpty()) throw new Exception("cache key is empty");
        
        if (_cache.xIsNotEmpty())
        {
            var bytes = await _cache.GetAsync(key);
            if (bytes.xIsNotEmpty())
            {
                return bytes.xToString().xToEntity<TResult>();
            }
        }

        return default;
    }

    private async Task ExecuteAsync(Action<TResult> resultBehavior)
    {
        await _service.OnExecutingAsync();
        if(_service.Result.xIsEmpty())
        {
			await _service.OnExecuteAsync();
		}
        if (resultBehavior.xIsNotEmpty())
        {
            resultBehavior(_service.Result);
        }
    }

    private async Task SetCacheAsync(string key, byte[] bytes,  DistributedCacheEntryOptions cacheEntryOptions)
    {
        if (key.xIsEmpty()) throw new Exception("cache key is empty");
        if (bytes.xIsEmpty()) throw new Exception("result is empty");

        if (cacheEntryOptions.xIsEmpty())
        {
            //기본 설정이 없을 경우 1분 만료
            cacheEntryOptions = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(60));
        }
        
        if (_cache.xIsNotEmpty())
        {
            await _cache.SetAsync(key.xGetHashCode(), bytes, cacheEntryOptions);
        }
    }

    #endregion [execute core]
}