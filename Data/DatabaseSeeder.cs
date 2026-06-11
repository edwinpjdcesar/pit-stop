using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Data;

public class DatabaseSeeder
{
    private readonly PitStopContext _context;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(PitStopContext context, ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await SeedMakesAndModelsAsync(cancellationToken);
    }

    private async Task SeedMakesAndModelsAsync(CancellationToken cancellationToken)
    {
        if (await _context.Makes.AnyAsync(cancellationToken))
        {
            _logger.LogInformation("Makes table already populated — skipping seed");
            return;
        }

        _logger.LogInformation("Seeding Makes and Models...");

        var sql = ReadEmbeddedSql("Data.Seeds.mock-makes-and-models.sql");
        await _context.Database.ExecuteSqlRawAsync(sql, cancellationToken);

        _logger.LogInformation("Makes and Models seeded successfully");
    }

    private static string ReadEmbeddedSql(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();

        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Embedded resource '{resourceName}' not found.");

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
