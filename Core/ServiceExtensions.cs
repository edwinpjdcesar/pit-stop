using Core.Services;
using Core.Services.Interfaces;
using Core.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Core;

public static class ServiceExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddScoped<IMakeService, MakeService>();
        services.AddScoped<IModelService, ModelService>();
        services.AddScoped<IVehicleService, VehicleService>();
        services.AddScoped<IMaintenanceService, MaintenanceService>();
        services.AddScoped<IMaintenancePartService, MaintenancePartService>();
        services.AddScoped<IPartService, PartService>();

        services.AddValidatorsFromAssemblyContaining<VehicleRequestValidator>();

        return services;
    }
}
