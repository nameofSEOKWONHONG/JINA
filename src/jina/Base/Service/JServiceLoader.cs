using System.Diagnostics;
using System.Transactions;
using eXtensionSharp;
using FluentValidation.Results;
using Jina.Base.Service.Abstract;
using Jina.Base.Validator;
using Microsoft.Extensions.Caching.Distributed;
using Serilog;

namespace Jina.Base.Service;

public class JServiceLoader<TService, TRequest, TResult> : JDisposeBase
    , IAddFilter<TRequest, TResult>
    , ISetParameter<TRequest, TResult>
    , IUseTransaction<TRequest, TResult>
    , IValidation<TRequest, TResult>
    , IExecutor<TRequest, TResult>
{
    private readonly IServiceImplBase<TRequest, TResult> _service;
    
    #region [action behavior's]

    private List<Func<bool>> _filters = new ();
    private Func<TRequest> _parameter;
    private JValidatorBase<TRequest> _validator;
    private Action<ValidationResult> _validateBehavior;

    #endregion
    
    #region [database transaction]
    
    private bool _useTransaction;
    private System.Transactions.IsolationLevel _isolationLevel;
    private TransactionScopeOption _transactionScopeOption;
    
    #endregion
    
    #region [cache]

    private IDistributedCache _cache;
    private string _cacheKey;
    private DistributedCacheEntryOptions _cacheEntryOptions;

    #endregion

    internal JServiceLoader(IServiceImplBase<TRequest, TResult> service)
    {
        _service = service;
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

    public IUseTransaction<TRequest, TResult> UseTransaction(TransactionScopeOption option = TransactionScopeOption.Required,
        IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted)
    {
        _useTransaction = true;
        _transactionScopeOption = option;
        _isolationLevel = isolationLevel;
        return this;
    }
    
    public IValidation<TRequest, TResult> SetValidator(JValidatorBase<TRequest> validator)
    {
        _validator = validator;
        return this;
    }

    public IExecutor<TRequest, TResult> OnValidated(Action<ValidationResult> validateBehavior)
    {
        _validateBehavior = validateBehavior;
        return this;
    }    

    public async Task ExecutedAsync(Action<TResult> onResult)
    {
        var sw = Stopwatch.StartNew();
        if (_useTransaction.xIsTrue())
        {
            using var scope = new TransactionScope(_transactionScopeOption,
                new TransactionOptions() { IsolationLevel = _isolationLevel },
                TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                await ExecuteCore(onResult);
                scope.Complete();
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, "ServiceLoader OnExecuted Error : {Error}", e.Message);
                throw;                    
            }
        }
        else
        {
            await ExecuteCore(onResult);
        }
        sw.Stop();
        #if DEBUG
        Log.Logger.Information("{ServiceName} execute time(sec):{Second}", _service.GetType().Name, sw.Elapsed.TotalSeconds);
        #endif
    }


    #region [execute core]
    private async Task ExecuteCore(Action<TResult> resultBehavior = null)
    {
        if (InvokedFilter().xIsFalse()) return;

        InvokedParameter();

        var valid = await InvokedValidatingAsync(_service.Request);
        if(valid.xIsFalse()) return;

        await GetCacheAsync();

        await ExecuteAsync(resultBehavior);

        await SetCacheAsync();
    }
    
    private bool InvokedFilter()
    {
        var filterValid = true;
        _filters.ForEach(filter =>
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
        if (_validator.xIsNotEmpty())
        {
            var rs = await _validator.ValidateAsync(parameter);
            if (rs.IsValid.xIsFalse())
            {
                _validateBehavior(rs);
            }

            return rs.IsValid;
        }

        return true;
    }

    private async Task GetCacheAsync()
    {
        if (_cache.xIsNotEmpty())
        {
            var bytes = await _cache.GetAsync(_cacheKey.xGetHashCode());
            if (bytes.xIsNotEmpty())
            {
                var exist = bytes.xToString().xToEntity<TResult>();
                if (exist.xIsNotEmpty())
                {
                    _service.Result = exist;
                }
            }    
        }
    }

    private async Task ExecuteAsync(Action<TResult> resultBehavior)
    {
        var isOk = await _service.OnExecutingAsync();
        if (isOk)
        {
            await _service.OnExecuteAsync();
        }
        if (resultBehavior.xIsNotEmpty())
        {
            resultBehavior(_service.Result);
        }
    }

    private async Task SetCacheAsync()
    {
        if (_cache.xIsNotEmpty())
        {
            if (_service.Result.xIsNotEmpty())
            {
                await _cache.SetAsync(_cacheKey.xGetHashCode(), _service.Result.xToBytes(), _cacheEntryOptions);
            }    
        }
    }    

    #endregion
}