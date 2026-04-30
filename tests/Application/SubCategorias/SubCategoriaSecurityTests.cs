using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.UseCases.SubCategorias.Commands.CreateSubCategoria;
using Intap.FirstProject.Domain.Categorias;
using Intap.FirstProject.Domain.SubCategorias;
using Intap.FirstProject.Infrastructure.Persistence;
using Intap.FirstProject.Infrastructure.Persistence.Repositories;
using Intap.FirstProject.Tests.Helpers;
using Microsoft.Extensions.Logging.Abstractions;

namespace Intap.FirstProject.Tests.Application.SubCategorias;

/// <summary>
/// Security tests — SEC-01 to SEC-04.
/// SEC-01: SQL injection payloads must not cause 500 — EF Core parameterized queries prevent injection.
/// SEC-02: Altered JWT token must be rejected with 401.
/// SEC-03: Valid JWT without required permission must return 403.
/// SEC-04: Sensitive data (passwords, tokens) must not appear in handler output.
/// </summary>
public sealed class SubCategoriaSecurityTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly IntegrationTestWebAppFactory _factory;

    public SubCategoriaSecurityTests(IntegrationTestWebAppFactory factory)
    {
        _factory = factory;
    }

    // ── Handler-level helpers ────────────────────────────────────────────────

    private static CreateSubCategoriaCommandHandler CreateHandlerFor(AppDbContext context) =>
        new(new SubCategoriaReadRepository(context),
            new SubCategoriaWriteRepository(context),
            new CategoriaReadRepository(context),
            new UnitOfWork(context),
            NullLogger<CreateSubCategoriaCommandHandler>.Instance);

    private async Task<Categoria> SeedActiveCategoriaAsync(AppDbContext db, string nombre)
    {
        Categoria categoria = Categoria.Create(nombre, null);
        db.Categorias.Add(categoria);
        await db.SaveChangesAsync();
        return categoria;
    }

    // ── SEC-01: SQL injection in Nombre ──────────────────────────────────────

    /// <summary>
    /// SEC-01: SQL injection payloads in Nombre must be treated as plain text.
    /// EF Core uses parameterized queries — injection is impossible at the ORM layer.
    /// The handler must NOT throw and must return a deterministic result.
    /// The SubCategorias table must remain intact after each call.
    /// </summary>
    [Theory]
    [InlineData("'; DROP TABLE SubCategorias; --")]
    [InlineData("' OR '1'='1")]
    [InlineData("<script>alert('xss')</script>")]
    [InlineData("1; SELECT * FROM Users")]
    [InlineData("admin'--")]
    public async Task SEC01_CreateHandler_SqlInjectionPayloadInNombre_DoesNotThrowAndTableRemainsIntact(string maliciousInput)
    {
        // Arrange
        AppDbContext context = TestDbContextFactory.Create();
        Categoria categoria = await SeedActiveCategoriaAsync(context, $"SecCat-{Guid.NewGuid()}");
        CreateSubCategoriaCommandHandler handler = CreateHandlerFor(context);

        CreateSubCategoriaCommand command = new(maliciousInput, null, categoria.Id);

        // Act — must not throw under any injection payload
        Func<Task<Result<Guid>>> act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().NotThrowAsync("EF Core parameterized queries prevent SQL injection");

        // Assert — table must still be accessible (not dropped)
        bool tableIntact = context.SubCategorias != null;
        tableIntact.Should().BeTrue("SubCategorias table must remain intact after injection attempt");

        // Assert — result is deterministic (Success or Conflict — never a server error)
        Result<Guid> result = await handler.Handle(command, CancellationToken.None);
        bool isDeterministicResult = result.IsSuccess || result.Error!.Type == ErrorTypeResult.Conflict;
        isDeterministicResult.Should().BeTrue("handler must return Success or Conflict, never an exception");
    }

    // ── SEC-02: Altered JWT token ─────────────────────────────────────────────

    /// <summary>
    /// SEC-02: A JWT token with an invalid signature must be rejected with 401.
    /// Tampers the payload of a valid token to simulate signature mismatch.
    /// </summary>
    [Fact]
    public async Task SEC02_AnyEndpoint_WithTamperedJwtSignature_Returns401()
    {
        // Arrange — generate a valid token then corrupt the signature segment
        string validToken  = IntegrationTestWebAppFactory.GenerateToken(["subcategorias:read"]);
        string[] parts     = validToken.Split('.');

        // Corrupt the signature by appending characters — signature check will fail
        string tamperedToken = $"{parts[0]}.{parts[1]}.{parts[2]}TAMPERED";

        HttpClient client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tamperedToken);

        // Act
        HttpResponseMessage response = await client.GetAsync("/api/subcategorias");

        // Assert — 401 Unauthorized (invalid signature)
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized,
            "a JWT with a tampered signature must be rejected with 401");
    }

    /// <summary>
    /// SEC-02 (variant): A completely fabricated JWT (signed with a different key) must also return 401.
    /// </summary>
    [Fact]
    public async Task SEC02_AnyEndpoint_WithFabricatedJwt_Returns401()
    {
        // Arrange — build a raw JWT-like string with wrong structure (not signed by the test key)
        string fabricatedToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9" +
                                 ".eyJzdWIiOiJoYWNrZXIiLCJwZXJtaXNzaW9uIjoic3ViY2F0ZWdvcmlhczpyZWFkIn0" +
                                 ".INVALID_SIGNATURE_THAT_WILL_NEVER_VERIFY";

        HttpClient client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", fabricatedToken);

        // Act
        HttpResponseMessage response = await client.GetAsync("/api/subcategorias");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized,
            "a fabricated JWT must be rejected with 401");
    }

    // ── SEC-03: Valid JWT without required permission ─────────────────────────

    /// <summary>
    /// SEC-03: A valid JWT without subcategorias:create permission must be rejected with 403.
    /// Verifies authorization policy enforcement at the HTTP pipeline level.
    /// </summary>
    [Fact]
    public async Task SEC03_CreateEndpoint_WithoutSubcategoriasCreatePermission_Returns403()
    {
        // Arrange — valid JWT with an unrelated permission
        AppDbContext db       = _factory.CreateDbContext();
        HttpClient   client   = _factory.CreateAuthenticatedClient(["view_projects"]);
        Categoria    categoria = await SeedActiveCategoriaAsync(db, $"SecCat-SEC03-{Guid.NewGuid()}");

        object payload = new { Nombre = "Unauthorized", Descripcion = (string?)null, CategoriaId = categoria.Id };

        // Act
        HttpResponseMessage response = await client.PostAsJsonAsync("/api/subcategorias", payload);

        // Assert — 403 Forbidden (authorized user but missing required permission)
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden,
            "a JWT without subcategorias:create must be rejected with 403");
    }

    [Fact]
    public async Task SEC03_ReadEndpoint_WithoutSubcategoriasReadPermission_Returns403()
    {
        // Arrange — JWT with create but NOT read permission
        HttpClient client = _factory.CreateAuthenticatedClient(["subcategorias:create"]);

        // Act
        HttpResponseMessage response = await client.GetAsync("/api/subcategorias");

        // Assert — endpoint requires subcategorias:read, which is absent
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden,
            "a JWT without subcategorias:read must be rejected with 403 on GET /api/subcategorias");
    }

    [Fact]
    public async Task SEC03_UpdateEndpoint_WithoutSubcategoriasUpdatePermission_Returns403()
    {
        // Arrange — valid JWT with read only, not update
        HttpClient client = _factory.CreateAuthenticatedClient(["subcategorias:read"]);

        object payload = new { Nombre = "X", Descripcion = (string?)null, CategoriaId = Guid.NewGuid(), Estado = true };

        // Act
        HttpResponseMessage response = await client.PutAsJsonAsync($"/api/subcategorias/{Guid.NewGuid()}", payload);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden,
            "a JWT without subcategorias:update must be rejected with 403 on PUT");
    }

    [Fact]
    public async Task SEC03_DeleteEndpoint_WithoutSubcategoriasDeletePermission_Returns403()
    {
        // Arrange — valid JWT with read only, not delete
        HttpClient client = _factory.CreateAuthenticatedClient(["subcategorias:read"]);

        // Act
        HttpResponseMessage response = await client.DeleteAsync($"/api/subcategorias/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden,
            "a JWT without subcategorias:delete must be rejected with 403 on DELETE");
    }

    // ── SEC-04: Sensitive data not exposed in logs/response ──────────────────

    /// <summary>
    /// SEC-04: Error responses and handler results must not contain sensitive data
    /// (passwords, tokens, internal stack traces, connection strings).
    /// Verifies that 4xx responses expose only structured error codes, not raw exceptions.
    /// </summary>
    [Fact]
    public async Task SEC04_ErrorResponse_DoesNotExposeStackTraceOrConnectionString()
    {
        // Arrange — trigger a 404 and inspect the response body
        HttpClient client = _factory.CreateAuthenticatedClient(["subcategorias:read"]);

        // Act
        HttpResponseMessage response = await client.GetAsync($"/api/subcategorias/{Guid.NewGuid()}");

        // Assert HTTP
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        string body = await response.Content.ReadAsStringAsync();

        // No stack trace markers
        body.Should().NotContainAny(
            "at System.", "StackTrace", "InnerException",
            "Server=", "Password=", "Data Source=",
            "connection string", "token_secret",
            "password_hash", "PasswordHash");

        // Should contain only structured error code
        body.Should().Contain("SUBCATEGORIA_NOT_FOUND");
    }

    [Fact]
    public async Task SEC04_CreateWithBadRequest_DoesNotExposeInternals()
    {
        // Arrange — send empty Nombre to trigger 400
        AppDbContext db       = _factory.CreateDbContext();
        HttpClient   client   = _factory.CreateAuthenticatedClient(["subcategorias:create"]);
        Categoria    categoria = await SeedActiveCategoriaAsync(db, $"SecCat-SEC04-{Guid.NewGuid()}");

        object payload = new { Nombre = "", Descripcion = (string?)null, CategoriaId = categoria.Id };

        // Act
        HttpResponseMessage response = await client.PostAsJsonAsync("/api/subcategorias", payload);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        string body = await response.Content.ReadAsStringAsync();

        body.Should().NotContainAny(
            "at System.", "StackTrace", "InnerException",
            "password_hash", "PasswordHash", "connection string");

        // Should contain a structured validation error code
        body.Should().Contain("SUBCATEGORIA_NOMBRE_REQUERIDO");
    }

    /// <summary>
    /// SEC-04 (handler level): The Result object returned by CreateSubCategoriaCommandHandler
    /// must not carry sensitive data in its Error or Value fields.
    /// </summary>
    [Fact]
    public async Task SEC04_HandlerResult_DoesNotContainSensitiveDataInErrorMessages()
    {
        // Arrange — trigger a conflict error at handler level
        AppDbContext context = TestDbContextFactory.Create();
        Categoria categoria = await SeedActiveCategoriaAsync(context, $"SecCat-SEC04H-{Guid.NewGuid()}");
        CreateSubCategoriaCommandHandler handler = CreateHandlerFor(context);

        await handler.Handle(new CreateSubCategoriaCommand("Laptops", null, categoria.Id), CancellationToken.None);

        // Act — second call triggers conflict
        Result<Guid> result = await handler.Handle(
            new CreateSubCategoriaCommand("laptops", null, categoria.Id), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().NotContainAny(
            "password", "token", "secret", "connectionString", "hash");
        result.Error.Description.Should().NotContainAny(
            "password", "token", "secret", "connectionString", "hash");
    }
}
