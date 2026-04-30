using Intap.FirstProject.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Intap.FirstProject.Tests.Helpers;

/// <summary>
/// Creates isolated AppDbContext instances backed by EF Core InMemory provider.
/// Each call produces a unique database so tests do not share state.
/// </summary>
public static class TestDbContextFactory
{
    public static AppDbContext Create()
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}
