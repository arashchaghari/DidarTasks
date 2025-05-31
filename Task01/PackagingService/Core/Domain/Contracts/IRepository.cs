namespace PackagingService.Core.Domain.Contracts;

public interface IRepository
{
    Task<GetSubscriptionLevelResponseDto> GetSubscriptionLevel(string UserId);
    Task<CheckAccessResponseDto> CheckAccessLimit(string userId);
    Task<UpdateSubscriptionResponseDto> UpdateSubscription(UpdateSubscriptionDto request);
}