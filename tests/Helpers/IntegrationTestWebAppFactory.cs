using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Intap.FirstProject.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Intap.FirstProject.Tests.Helpers;

/// <summary>
/// WebApplicationFactory that replaces the real DB with EF Core InMemory.
/// Each instance gets a unique DB name so parallel test classes do not share state.
/// JWT configuration is overridden to use the TestJwtSettings key.
/// </summary>
public sealed class IntegrationTestWebAppFactory : WebApplicationFactory<Program>
{
    public static class TestJwtSettings
    {
        public const string SecretKey = "IntapTestSuperSecretKeyForIntegration2026!";
        public const string Issuer    = "Intap.FirstProject.API";
        public const string Audience  = "Intap.FirstProject.Client";
    }

    private readonly string _dbName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Remove real Npgsql DbContext registration
            ServiceDescriptor? descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor is not null)
                services.Remove(descriptor);

            // Also remove the DbContext itself if registered
            ServiceDescriptor? contextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(AppDbContext));
            if (contextDescriptor is not null)
                services.Remove(contextDescriptor);

            // Register InMemory replacement
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(_dbName));
        });

        // Override JWT config so tests can sign tokens with known key
        builder.UseSetting("Jwt:SecretKey",  TestJwtSettings.SecretKey);
        builder.UseSetting("Jwt:Issuer",     TestJwtSettings.Issuer);
        builder.UseSetting("Jwt:Audience",   TestJwtSettings.Audience);
    }

    /// <summary>
    /// Resolves the scoped AppDbContext for seed/assertion within a test.
    /// Always create a new scope — never reuse the factory's root scope.
    /// </summary>
    public AppDbContext CreateDbContext()
    {
        IServiceScope scope = Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    /// <summary>
    /// Generates a signed JWT bearing the given permission claims.
    /// </summary>
    public static string GenerateToken(IEnumerable<string> permissions, string? userId = null)
    {
        SymmetricSecurityKey key         = new(Encoding.UTF8.GetBytes(TestJwtSettings.SecretKey));
        SigningCredentials   credentials = new(key, SecurityAlgorithms.HmacSha256);

        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Sub,   userId ?? Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Email, "test@intap.com"),
            new(JwtRegisteredClaimNames.Name,  "Test User"),
            new(ClaimTypes.Role, "TestRole"),
            new(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
            ..permissions.Select(p => new Claim("permission", p)),
        ];

        JwtSecurityToken token = new(
            issuer:             TestJwtSettings.Issuer,
            audience:           TestJwtSettings.Audience,
            claims:             claims,
            expires:            DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Creates an HttpClient with Authorization bearer token for the given permissions.
    /// </summary>
    public HttpClient CreateAuthenticatedClient(IEnumerable<string> permissions)
    {
        HttpClient client = CreateClient();
        string     token  = GenerateToken(permissions);
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}
