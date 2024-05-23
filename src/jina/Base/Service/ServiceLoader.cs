using eXtensionSharp;
using FluentValidation;
using FluentValidation.Results;
using Jina.Base.Service.Abstract;
using Microsoft.Extensions.Caching.Distributed;
using Serilog;

namespace Jina.Base.Service;

/// <summary>
/// ServiceLoader class definition with generic types TRequest and TResult. It implements interfaces for adding filters, setting parameters, validation, and execution.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResult"></typeparam>
public class ServiceLoader<TRequest, TResult> : IServiceLoaderBase
    , IAddFilter<TRequest, TResult>
    , ISetParameter<TRequest, TResult>
    , IValidation<TRequest, TResult>
    , IExecutor<TRequest, TResult>
{
    public IServiceImplBase Self { get; private set; }
    private readonly IServiceImplBase<TRequest, TResult> _service;

    #region [action behavior's]

    private readonly List<Func<bool>> _filters = new();
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

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="service"></param>
    /// <param name="sessionContext"></param>
    internal ServiceLoader(IServiceImplBase<TRequest, TResult> service)
    {
        _service = service;
        this.Self = _service;
    }

    #region [impl chain methods]

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
        this._cacheEntryOptions = cacheEntryOptions;
        
        if (_cacheEntryOptions.xIsEmpty())
        {
            //기본 설정이 없을 경우 1분 만료
            _cacheEntryOptions = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(60));
        }
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

    #endregion

    #region [execute core]

    public async Task ExecuteCore()
    {
        try
        {
            #region [execute filter, parameter, get cache, validator]

            if (InvokedFilter().xIsFalse()) return;

            InvokedParameter();
        
            #region [use cache]

            if (_useCache)
            {
                if (typeof(TResult).IsInterface)
                {
                    throw new Exception("The cache does not allow interface types");
                }

                var hasCache = await TryGetCacheAsync();
                if (hasCache)
                {
                    _onResult(_service.Result);
                    return;
                }
            }

            #endregion        

            var valid = await InvokedValidatingAsync(_service.Request);
            if (valid.xIsFalse()) return;        

            #endregion

            await ExecuteAsync(_onResult);

            #region [execute set cache]

            if (_useCache)
            {
                await SetCacheAsync();
            }        

            #endregion
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, "Error occurred during execution");
        }
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

    private async Task ExecuteAsync(Action<TResult> resultBehavior)
    {
        var valid = await _service.OnExecutingAsync();
        if(valid.xIsTrue())
        {
			await _service.OnExecuteAsync();
		}
        if (resultBehavior.xIsNotEmpty())
        {
            resultBehavior(_service.Result);
        }
    }
    
    /// <summary>
    /// {0}:TenantId,
    /// {1}:UserId,
    /// {2}:Key
    /// </summary>
    private const string KEY_FORMAT = "{0},{1},{2}";
    private async Task<bool> TryGetCacheAsync()
    {
        if (_service.Context.DistributedCache.xIsEmpty()) return false;
        
        var cacheKey = GetCacheKey();
        var cachedData = await _service.Context.DistributedCache.GetAsync(cacheKey);
        
        if (cachedData != null)
        {
            _service.Result = cachedData.xToString().xToEntity<TResult>();
            _onResult?.Invoke(_service.Result);
            return true;
        }
        return false;
    }

    private async Task SetCacheAsync()
    {
        
        if(_service.Context.DistributedCache.xIsEmpty()) return;

        var cacheKey = GetCacheKey();
        if (cacheKey.xIsEmpty()) throw new Exception("cache key is empty");
        var cachedData = _service.Result.xToBytes();
        if (cachedData.xIsEmpty()) throw new Exception("result is empty");
        
        await _service.Context.DistributedCache.SetAsync(cacheKey, cachedData, _cacheEntryOptions);
    }
    
    private string GetCacheKey()
    {
        if(_cacheKey.xIsEmpty()) throw new Exception("cache key is empty");
        return string.Format(KEY_FORMAT, _service.Context.TenantId, _service.Context.CurrentUser.UserId, _cacheKey ?? _service.Request.xToJson().xGetHashCode());
    }

    #endregion [execute core]
}