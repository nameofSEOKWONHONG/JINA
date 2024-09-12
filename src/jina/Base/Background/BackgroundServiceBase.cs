using DocumentFormat.OpenXml.Office2013.Drawing.TimeSlicer;
using eXtensionSharp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Jina.Base.Background;

public abstract class BackgroundServiceBase<TSelf> : BackgroundService
{
    protected ILogger<TSelf> Logger { get; set; }
    protected IServiceScopeFactory ServiceScopeFactory { get; set; }

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="serviceScopeFactory"></param>
    protected BackgroundServiceBase(ILogger<TSelf> logger, IServiceScopeFactory serviceScopeFactory)
    {
        Logger = logger;
        ServiceScopeFactory = serviceScopeFactory;
    }
}

public abstract class BackgroundServiceBase<TSelf, TRequest> : BackgroundServiceBase<TSelf>
where TSelf : class
where TRequest : class
{
    private readonly int _parallelCount;
    private readonly int _interval;

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="serviceScopeFactory"></param>
    /// <param name="parallelCount"></param>
    /// <param name="interval"></param>
    protected BackgroundServiceBase(ILogger<TSelf> logger, IServiceScopeFactory serviceScopeFactory, int parallelCount, int interval = 1000):base(logger, serviceScopeFactory)
    {
        this._parallelCount = parallelCount;
        this._interval = interval;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Logger.LogInformation("{ServiceName} Start", typeof(TSelf).Name);

        while (!stoppingToken.IsCancellationRequested)
        {
            var items = await OnProducerAsync(stoppingToken);        
            Logger.LogInformation("Retrieve item count: {Num}", items.xCount());

            if (items.xCount() <= 0)
            {
                await Task.Delay(_interval, stoppingToken);
                continue;
            }

            try
            {
                await Parallel.ForEachAsync(items, new ParallelOptions()
                {
                    // ReSharper disable once HeapView.BoxingAllocation
                    MaxDegreeOfParallelism = this._parallelCount.xValue<int>(Environment.ProcessorCount),
                    CancellationToken = stoppingToken
                }, async (request, token) =>
                {
                    token.ThrowIfCancellationRequested();
                    await OnConsumerAsync(request, token);
                    token.ThrowIfCancellationRequested();
                });
            }
            catch (TaskCanceledException taskCanceledException)
            {
                Logger.LogError(taskCanceledException, "{ServiceName} Error: {Error}", typeof(TSelf).Name,
                    taskCanceledException.Message);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "{ServiceName} Error: {Error}", typeof(TSelf).Name,
                    e.Message);
            }

            Logger.LogInformation("{ServiceName} End", typeof(TSelf).Name);

            await Task.Delay(_interval, stoppingToken);
        }
    }

    protected abstract Task<IEnumerable<TRequest>> OnProducerAsync(CancellationToken stoppingToken);
    protected abstract Task OnConsumerAsync(TRequest request, CancellationToken stoppingToken);
}