using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Jina.Base.Background;

public abstract class KafkaBackgroundServiceBase<TSelf> : BackgroundServiceBase<TSelf>
where TSelf : class
{
    private readonly string _subscibeTopic;
    private readonly int _interval;
    
    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="serviceScopeFactory"></param>
    /// <param name="subscribeTopic"></param>
    /// <param name="interval"></param>
    protected KafkaBackgroundServiceBase(ILogger<TSelf> logger, IServiceScopeFactory serviceScopeFactory, string subscribeTopic, int interval = 500) : base(logger, serviceScopeFactory)
    {
        _subscibeTopic = subscribeTopic;
        _interval = interval;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var scope = this.ServiceScopeFactory.CreateAsyncScope();
        
        var config = scope.ServiceProvider.GetRequiredService<ConsumerConfig>();
        using var consumer = new ConsumerBuilder<string, string>(config).Build();
        consumer.Subscribe(_subscibeTopic);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            if (Logger.IsEnabled(LogLevel.Information))
            {
                Logger.LogInformation("{name} Worker running at: {time}", typeof(TSelf).Name, DateTimeOffset.Now);
            }
            
            await Task.Run(async () =>
            {
                var consumeResult = consumer.Consume(stoppingToken);

                try
                {
                    await using var subScope = scope.ServiceProvider.CreateAsyncScope();
                    await ExecuteConsumeAsync(subScope, consumeResult, stoppingToken);
                    consumer.Commit(consumeResult);
                }
                catch (Exception e)
                {
                    Logger.LogError(e, "{name} Worker Error: {message}", typeof(TSelf).Name, e.Message);
                }
            }, stoppingToken);
            
            await Task.Delay(_interval, stoppingToken);
        }
        
        consumer.Close();
    }
    
    /// <summary>
    /// Kafka consume data를 처리한다. 반드시 재구현하여 처리합니다.
    /// </summary>
    /// <param name="scope">ioc container</param>
    /// <param name="consumeResult">kafka consumer result</param>
    /// <param name="stoppingToken">cancellation token</param>
    /// <returns></returns>
    protected abstract Task ExecuteConsumeAsync(AsyncServiceScope scope, 
        ConsumeResult<string, string> consumeResult,
        CancellationToken stoppingToken);     
}