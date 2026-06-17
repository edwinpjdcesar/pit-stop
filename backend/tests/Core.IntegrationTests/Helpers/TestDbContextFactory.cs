using Data;
using Microsoft.EntityFrameworkCore;

namespace Core.IntegrationTests.Helpers;

public static class TestDbContextFactory
{
    public static PitStopContext Create()
    {
        var options = new DbContextOptionsBuilder<PitStopContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new PitStopContext(options);
    }
}
