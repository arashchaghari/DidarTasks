using Grpc.Core;
using Subscription;

namespace PackagingService.Services;

public class PackagingServiceImpl : PackagingGrpcService.PackagingGrpcServiceBase
{
    private static readonly Dictionary<string, string> _userSubscriptions = [];

    public override Task<CheckAccessResponse> CheckAccessLimit(CheckAccessRequest request, ServerCallContext context)
    {
        _userSubscriptions.TryGetValue(request.UserId, out var level);

        var allowed = level switch
        {
            "Premium" => true,
            "Basic" => request.Feature != "PremiumFeature",
            _ => false,
        };

        return Task.FromResult(new CheckAccessResponse
        {
            Allowed = allowed,
            Message = allowed ? "Access granted" : "Access denied"
        });
    }

    public override Task<UpdateSubscriptionResponse> UpdateSubscription(UpdateSubscriptionRequest request, ServerCallContext context)
    {
        _userSubscriptions[request.UserId] = request.SubscriptionLevel;

        return Task.FromResult(new UpdateSubscriptionResponse
        {
            Success = true,
            Message = "Subscription updated"
        });
    }

    public override Task<GetSubscriptionLevelResponse> GetSubscriptionLevel(GetSubscriptionLevelRequest request, ServerCallContext context)
    {
        _userSubscriptions.TryGetValue(request.UserId, out var level);
        return Task.FromResult(new GetSubscriptionLevelResponse { SubscriptionLevel = level ?? "None" });
    }
}
