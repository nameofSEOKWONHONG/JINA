using eXtensionSharp;
using Microsoft.Extensions.Caching.Distributed;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;

namespace Jina.Utils.Redis;

public class ConcurrencyGuard
{
    private readonly IDistributedCache _distributedCache;
    private readonly RedLockFactory _redLockFactory;

    public ConcurrencyGuard(IDistributedCache distributedCache, IConnectionMultiplexer multiplexer)
    {
        _distributedCache = distributedCache;
        _redLockFactory = RedLockFactory.Create(new List<RedLockMultiplexer> { multiplexer as ConnectionMultiplexer });
    }
    
    public async Task<bool> AddEvent(string eventName, string value)
    {
        var key = GetEventName(eventName);
        var valueBytes = value.xToBytes();

        await using var redLock = await _redLockFactory.CreateLockAsync(key, TimeSpan.FromSeconds(2));
        if (redLock.IsAcquired.xIsFalse())
        {
            System.Console.WriteLine(
                $"{DateTime.UtcNow.ToString(ENUM_DATE_FORMAT.YYYY_MM_DD_HH_MM_SS_FFF)} Could not acquire lockfor {eventName}");
            return false;
        }
        System.Console.WriteLine(
            $"{DateTime.UtcNow.ToString(ENUM_DATE_FORMAT.YYYY_MM_DD_HH_MM_SS_FFF)} Acquired lock for {eventName}");
        var cached = await _distributedCache.GetAsync(key);
        if (cached.xIsEmpty())
        {
            System.Console.WriteLine(
                $"{DateTime.UtcNow.ToString(ENUM_DATE_FORMAT.YYYY_MM_DD_HH_MM_SS_FFF)} No duplicate event found. Adding Event: {eventName}");

            await _distributedCache.SetAsync(key, valueBytes);
            System.Console.WriteLine(
                $"{DateTime.UtcNow.ToString(ENUM_DATE_FORMAT.YYYY_MM_DD_HH_MM_SS_FFF)} Added Event: {eventName}");

            return true;
        }
        return false;
    }

    public async Task<bool> RemoveEvent(string eventName)
    {
        var key = GetEventName(eventName);
        var cached = await _distributedCache.GetAsync(key);
        if (cached.xIsNotEmpty())
        {
            await _distributedCache.RemoveAsync(key);
            return true;
        }
        return false;
    }

    private string GetEventName(string eventName)
    {
        return $"event:{eventName}";
    }
}