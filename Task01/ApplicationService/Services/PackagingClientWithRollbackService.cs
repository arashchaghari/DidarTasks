namespace ApplicationService.Services;

// Strong Consistency: This service ensures that if the subscription update fails, it rolls back to the previous subscription level.
public class PackagingClientWithRollbackService(PackagingClient packagingClient)
{
    public async Task<bool> ChangeSubscriptionWithRollbackAsync(string userId, string newLevel)
    {
        try
        {
            var currentLevel = await GetCurrentSubscriptionLevelAsync(userId);

            try
            {
                var updateResponse = await packagingClient.UpdateSubscriptionAsync(userId, newLevel);
                if (!updateResponse.Success)
                    throw new Exception(updateResponse.Message);

                await PerformOtherBusinessLogicAsync(userId);

                return true;
            }
            catch (Exception)
            {
                var result = await packagingClient.UpdateSubscriptionAsync(userId, currentLevel);

                // Log the result or ...

                return false;
            }
        }
        catch (Exception)
        {
            // Log the error
            return false;
        }
    }

    public async Task<string> GetCurrentSubscriptionLevelAsync(string userId)
    {
        var response = await packagingClient.GetSubscriptionAsync(userId);
        return response.SubscriptionLevel;
    }

    private static Task PerformOtherBusinessLogicAsync(string userId)
    {

        // Perform the logic based on the userId
        return Task.CompletedTask;
    }

    public async Task<bool> PerformFeatureActionAsync(string userId, string feature)
    {
        var accessResponse = await packagingClient.CheckAccessAsync(userId, feature);

        if (!accessResponse.Allowed)
        {
            throw new UnauthorizedAccessException(accessResponse.Message);
        }

        // ...

        return true;
    }
}