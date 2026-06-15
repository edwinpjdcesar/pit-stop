using Data;
using Domain.Entities;

namespace Core.IntegrationTests.Helpers;

public static class DbSeeder
{
    public static async Task<(Make make, Model model)> SeedMakeAndModelAsync(PitStopContext context)
    {
        var make = new Make { MakeId = 1, Code = "HONDA", Name = "Honda" };
        var model = new Model { ModelId = 1, MakeId = 1, Code = "CIVIC", Name = "Civic" };

        context.Makes.Add(make);
        context.Models.Add(model);
        await context.SaveChangesAsync();

        return (make, model);
    }

    public static async Task<Vehicle> SeedVehicleAsync(PitStopContext context, int makeId = 1, int modelId = 1, int year = 2020)
    {
        var vehicle = new Vehicle
        {
            VehicleId = Guid.NewGuid(),
            MakeId = makeId,
            ModelId = modelId,
            Year = year
        };

        context.Vehicles.Add(vehicle);
        await context.SaveChangesAsync();

        return vehicle;
    }

    public static async Task<Part> SeedPartAsync(PitStopContext context, string name = "Oil Filter", string description = "Standard oil filter")
    {
        var part = new Part
        {
            PartId = Guid.NewGuid(),
            Name = name,
            Description = description
        };

        context.Parts.Add(part);
        await context.SaveChangesAsync();

        return part;
    }

    public static async Task<Maintenance> SeedMaintenanceAsync(PitStopContext context, Guid vehicleId)
    {
        var maintenance = new Maintenance
        {
            MaintenanceId = Guid.NewGuid(),
            VehicleId = vehicleId,
            Description = "Oil change",
            Mileage = 24000,
            ServiceDate = new DateTime(2026, 5, 1, 10, 30, 0, DateTimeKind.Utc)
        };

        context.MaintenanceRecords.Add(maintenance);
        await context.SaveChangesAsync();

        return maintenance;
    }
}
