namespace PackagingService.Core.Domain.Contracts;

public record GetSubscriptionLevelResponseDto(string SubscriptionLevel);
public record CheckAccessResponseDto(bool Allowed, string Message);
public record UpdateSubscriptionDto(string UserId, string SubscriptionLevel);
public record UpdateSubscriptionResponseDto(bool Success, string Message);