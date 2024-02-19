using eXtensionSharp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Jina.Base.Background;

public abstract class BackgroundServiceBase<TSelf, TRequest> : BackgroundService
where TSelf : class
{
    protected ILogger<TSelf> Logger { get; set; }
    protected IServiceScopeFactory ServiceScopeFactory { get; set; }
    private readonly int _parallelCount;
    private readonly int _interval;

    protected BackgroundServiceBase(ILogger<TSelf> logger, IServiceScopeFactory serviceScopeFactory, int parallelCount, int interval = 1000)
    {
        Logger = logger;
        this._parallelCount = parallelCount;
        this._interval = interval;
        this.ServiceScopeFactory = serviceScopeFactory;
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