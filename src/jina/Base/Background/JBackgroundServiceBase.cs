using eXtensionSharp;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Jina.Base.Background;

public abstract class JBackgroundServiceBase<TSelf, TRequest> : BackgroundService
where TSelf : class
{
    protected ILogger Logger = Log.Logger;
    
    private readonly int _parallelCount;
    private readonly int _interval;
    protected JBackgroundServiceBase(int parallelCount, int interval = 1000)
    {
        this._parallelCount = parallelCount;
        this._interval = interval;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Logger.Information("{ServiceName} Start", typeof(TSelf).Name);

        while (!stoppingToken.IsCancellationRequested)
        {
            var items = await OnProducerAsync(stoppingToken);
        
            Logger.Information("Retrieve item number: {Num}", items.xCount());

            try
            {
                if (items.xCount() <= 0)
                {
                    await Task.Delay(_interval, stoppingToken);
                    return;
                }

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
                Logger.Error(taskCanceledException, "{ServiceName} Error: {Error}", typeof(TSelf).Name,
                    taskCanceledException.Message);
            }
            catch (Exception e)
            {
                Logger.Error(e, "{ServiceName} Error: {Error}", typeof(TSelf).Name,
                    e.Message);
            }

            Logger.Information("{ServiceName} End", typeof(TSelf).Name);

            await Task.Delay(_interval, stoppingToken);
        }
    }

    protected abstract Task<IEnumerable<TRequest>> OnProducerAsync(CancellationToken stoppingToken);
    protected abstract Task OnConsumerAsync(TRequest request, CancellationToken stoppingToken);
}