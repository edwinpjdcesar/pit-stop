using Core.Services;
using Core.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Core;

public static class ServiceExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddScoped<IVehicleService, VehicleService>();
        services.AddScoped<IMaintenanceService, MaintenanceService>();
        services.AddScoped<IMaintenancePartService, MaintenancePartService>();
        services.AddScoped<IPartService, PartService>();

        return services;
    }
}
