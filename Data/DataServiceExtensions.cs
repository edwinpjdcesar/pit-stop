using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Data;

public static class DataServiceExtensions
{
    public static IServiceCollection AddDataServices(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<PitStopContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }
}
