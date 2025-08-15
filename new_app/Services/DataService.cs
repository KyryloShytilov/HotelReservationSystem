using System.Threading.Tasks;
using new_app.Data;

namespace new_app.Services;

public interface IDataService
{
    Task RefreshDataAsync();
}

public class DataService : IDataService
{
    private readonly ApplicationDbContext _context;

    public DataService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task RefreshDataAsync()
    {
        // Implement any data refreshing logic here
        await Task.CompletedTask;
    }
}