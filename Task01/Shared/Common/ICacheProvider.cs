namespace PackagingService.Core.Domain.Contracts;

public interface ICacheProvider
{
    public Task<bool> Set(string key, string value, DateTime expirationDate);
    public Task<bool> Set(string key, string value);
    Task<bool> Remove(string key);
    Task<string?> Get(string key);
}