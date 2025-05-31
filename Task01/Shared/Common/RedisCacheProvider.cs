using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using PackagingService.Core.Domain.Contracts;
using Polly;
using Polly.CircuitBreaker;
using Polly.Registry;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Common;

public class RedisCacheService(IDistributedCache cache,
                               IOptions<RedisOption> options,
                               ResiliencePipelineProvider<string> resiliencePipeline)
    : ICacheProvider
{
    private readonly IDistributedCache _cache = cache;
    private readonly ResiliencePipelineProvider<string> _resiliencePipeline = resiliencePipeline;
    private readonly RedisOption _options = options.Value;

    public async Task<bool> Remove(string key)
    {
        var pipeline = _resiliencePipeline.GetPipeline<bool>(nameof(RedisCashServiceResiliencyKey.SetOrRemoveCircuitBreaker));

        var outcome = await pipeline.ExecuteOutcomeAsync(async (ctx, state) =>
        {
            await _cache.RemoveAsync(key);
            return Outcome.FromResult(true);

        }, ResilienceContextPool.Shared.Get(), "state");

        return outcome.Exception is not BrokenCircuitException && outcome.Result;
    }

    public async Task<bool> Set(string key, string value)
    {
        return await Set(key, value, DateTime.UtcNow.AddDays(_options.DefaultExpirationTimeInDays));
    }

    public async Task<bool> Set(string key, string value, DateTime expirationDate)
    {
        var pipeline = _resiliencePipeline.GetPipeline<bool>(
          nameof(RedisCashServiceResiliencyKey.SetOrRemoveCircuitBreaker));

        var outcome = await pipeline.ExecuteOutcomeAsync(async (ctx, state) =>
        {
            DistributedCacheEntryOptions cacheEntryOptions = new()
            {
                SlidingExpiration = TimeSpan.FromHours(_options.SlidingExpirationHour),
                AbsoluteExpiration = expirationDate,
            };
            var dataToCache = Encoding.UTF8.GetBytes(value);
            await _cache.SetAsync(key, dataToCache, cacheEntryOptions);
            return Outcome.FromResult(true);

        }, ResilienceContextPool.Shared.Get(), "state");

        return outcome.Exception is not BrokenCircuitException && outcome.Result;
    }

    public async Task<string?> Get(string key)
    {
        var pipeline = _resiliencePipeline.GetPipeline<string>(
            nameof(RedisCashServiceResiliencyKey.GetCircuitBreaker));

        var outcome = await pipeline.ExecuteOutcomeAsync(async (ctx, state) =>
        {
            return Outcome.FromResult(await _cache.GetStringAsync(key));
        }, ResilienceContextPool.Shared.Get(), "state");

        return outcome.Exception is BrokenCircuitException ? null : outcome.Result;

    }
}

public enum RedisCashServiceResiliencyKey
{
    SetOrRemoveCircuitBreaker,
    GetCircuitBreaker
}

public class RedisOption
{
    [Range(1, 60)]
    public byte DefaultExpirationTimeInDays { get; set; }
    public int SlidingExpirationHour { get; set; }

    //[Url]
    public string RedisCacheUrl { get; set; }
    public string Password { get; set; }
}