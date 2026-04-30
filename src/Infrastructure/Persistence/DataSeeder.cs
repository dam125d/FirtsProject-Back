using Intap.FirstProject.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Intap.FirstProject.Infrastructure.Persistence;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        ILogger<AppDbContext> logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

        try
        {
            // Non-relational providers (e.g. InMemory in tests) do not support migrations.
            if (context.Database.IsRelational())
                await context.Database.MigrateAsync();
            else
                await context.Database.EnsureCreatedAsync();

            const string adminEmail    = "admin@intap.com";
            const string adminPassword = "Admin@1234";

            User? existing = await context.Users
                .FirstOrDefaultAsync(u => u.Email == adminEmail);

            if (existing is null)
            {
                logger.LogInformation("Seeding initial admin user...");

                string passwordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword, workFactor: 12);

                User admin = User.Create(
                    fullName: "Administrador",
                    email: adminEmail,
                    passwordHash: passwordHash,
                    roleId: new Guid("00000000-0000-0000-0000-000000000001")
                );

                context.Users.Add(admin);
                await context.SaveChangesAsync();

                logger.LogInformation("Admin user seeded: {Email}", adminEmail);
            }
            else if (!BCrypt.Net.BCrypt.Verify(adminPassword, existing.PasswordHash))
            {
                logger.LogWarning("Admin user exists but password hash is invalid — resetting...");

                existing.ResetPassword(BCrypt.Net.BCrypt.HashPassword(adminPassword, workFactor: 12));
                await context.SaveChangesAsync();

                logger.LogInformation("Admin user password hash updated: {Email}", adminEmail);
            }
            else
            {
                logger.LogInformation("Admin user already exists and is valid, skipping seed.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }
}
