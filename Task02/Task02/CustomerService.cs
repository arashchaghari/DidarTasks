
namespace Task02;

// Domain layer
public interface ICustomerRepository
{
    Task AddAsync();
}

// Infrastructure layer
public class CustomerRepository(AppDbContext dbContext) : ICustomerRepository
{
    public async Task AddAsync()
    {
        dbContext.Add();
        await dbContext.SaveChangesAsync();
    }
}

public class CustomerService(ICustomerRepository repo, ILogger<CustomerService> logger)
{
    public bool AddCustomer()
    {
        try
        {
            repo.AddAsync();
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to add customer.");
            return false;
        }
    }
}

// Or can using CQRS pattern to add customer to the database instead of service (command)

// Changes:
// Dependency Injection
// Logging Abstraction
// Return Type
// Separation of Concerns
// Extensibility