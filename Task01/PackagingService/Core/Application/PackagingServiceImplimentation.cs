using Grpc.Core;
using Mapster;
using PackagingService.Core.Domain.Contracts;
using Subscription;
using System.Text.Json;

namespace PackagingService.Core.Application;

public class PackagingServiceImpl(IRepository repo, ICacheProvider cache) : PackagingGrpcService.PackagingGrpcServiceBase
{
    public override async Task<CheckAccessResponse> CheckAccessLimit(CheckAccessRequest request, ServerCallContext context)
    {
        var cacheKey = $"AccessLimit_{request.UserId}";

        var cachedData = await cache.Get(cacheKey);
        if (cachedData != null)
        {
            var cachedResponse = JsonSerializer.Deserialize<CheckAccessResponse>(cachedData);
            if (cachedResponse != null)
                return cachedResponse;
        }

        var result = await repo.CheckAccessLimit(request.UserId);
        var adapted = result.Adapt<CheckAccessResponse>();
        await cache.Set(cacheKey, JsonSerializer.Serialize(adapted));

        return adapted;
    }
    public override async Task<UpdateSubscriptionResponse> UpdateSubscription(UpdateSubscriptionRequest request, ServerCallContext context)
    {
        var result = await repo.UpdateSubscription(request.Adapt<UpdateSubscriptionDto>());

        var cacheKey = $"AccessLimit_{request.UserId}";
        await cache.Remove(cacheKey);

        return result.Adapt<UpdateSubscriptionResponse>();
    }

    public override async Task<GetSubscriptionLevelResponse> GetSubscriptionLevel(GetSubscriptionLevelRequest request, ServerCallContext context)
    {
        var cacheKey = $"SubscriptionLevel_{request.UserId}";

        var cachedData = await cache.Get(cacheKey);
        if (cachedData != null)
        {
            var cachedResponse = JsonSerializer.Deserialize<GetSubscriptionLevelResponse>(cachedData);
            if (cachedResponse != null)
                return cachedResponse;
        }

        var result = await repo.GetSubscriptionLevel(request.UserId);
        var adapted = result.Adapt<GetSubscriptionLevelResponse>();

        await cache.Set(cacheKey, JsonSerializer.Serialize(adapted));

        return adapted;
    }
}
