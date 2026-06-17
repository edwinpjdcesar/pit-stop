using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(PitStopContext context)
    {
        await SeedMakesAsync(context);
        await SeedModelsAsync(context);
    }

    private static async Task SeedMakesAsync(PitStopContext context)
    {
        if (context.Makes.Any()) return;
        await ExecuteScriptAsync(context, "Data.Seeds.makes.sql");
    }

    private static async Task SeedModelsAsync(PitStopContext context)
    {
        if (context.Models.Any()) return;
        await ExecuteScriptAsync(context, "Data.Seeds.models.sql");
    }

    private static async Task ExecuteScriptAsync(PitStopContext context, string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(resourceName)!;
        using var reader = new StreamReader(stream);
        var sql = await reader.ReadToEndAsync();
        await context.Database.ExecuteSqlRawAsync(sql);
    }
}
