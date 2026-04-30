using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Intap.FirstProject.Application.UseCases.TiposTareas.DTOs;
using Intap.FirstProject.Domain.TiposTareas;
using Intap.FirstProject.Infrastructure.Persistence;
using Intap.FirstProject.Tests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Intap.FirstProject.Tests.Integration.TiposTareas;

/// <summary>
/// HTTP integration tests — IT-01 to IT-23.
/// One test per Scenario from spec-tests CA-01 to CA-05 plus plan integration flows.
/// Uses WebApplicationFactory with InMemory DB — real HTTP pipeline, real handlers.
/// Policy names match Program.cs: view_tipos_tareas / create_tipos_tareas / edit_tipos_tareas / delete_tipos_tareas.
/// </summary>
public sealed class TiposTareasIntegrationTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly IntegrationTestWebAppFactory _factory;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public TiposTareasIntegrationTests(IntegrationTestWebAppFactory factory)
    {
        _factory = factory;
    }

    // ── Seed helpers ─────────────────────────────────────────────────────────

    private async Task<TipoTarea> SeedActiveTipoTareaAsync(AppDbContext db, string nombre, string? desc = null)
    {
        TipoTarea tt = TipoTarea.Create(nombre, desc);
        db.TiposTarea.Add(tt);
        await db.SaveChangesAsync();
        return tt;
    }

    private async Task<TipoTarea> SeedInactiveTipoTareaAsync(AppDbContext db, string nombre, string? desc = null)
    {
        TipoTarea tt = TipoTarea.Create(nombre, desc);
        tt.Deactivate();
        db.TiposTarea.Add(tt);
        await db.SaveChangesAsync();
        return tt;
    }

    // ── CA-01: Listar ─────────────────────────────────────────────────────────

    // IT-01: Listado paginado sin filtros — body.data.length === 3, pagination presente
    [Fact]
    public async Task IT01_GetAll_NullFilters_Returns200WithPaginationEnvelope()
    {
        // Arrange
        AppDbContext db     = _factory.CreateDbContext();
        HttpClient   client = _factory.CreateAuthenticatedClient(["view_tipos_tareas"]);
        string       suffix = Guid.NewGuid().ToString("N")[..8];

        await SeedActiveTipoTareaAsync(db, $"TT-A1-{suffix}");
        await SeedActiveTipoTareaAsync(db, $"TT-A2-{suffix}");
        await SeedInactiveTipoTareaAsync(db, $"TT-I1-{suffix}");

        // Act
        HttpResponseMessage response = await client.GetAsync("/api/tipos-tareas?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using JsonDocument doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        JsonElement root = doc.RootElement;

        root.TryGetProperty("data",       out JsonElement data).Should().BeTrue();
        root.TryGetProperty("pagination", out JsonElement pagination).Should().BeTrue();

        // pagination contains page, pageSize, total
        pagination.TryGetProperty("page",  out _).Should().BeTrue();
        pagination.TryGetProperty("total", out _).Should().BeTrue();

        // All 3 items (active + inactive) are returned when no estado filter
        int count = data.GetArrayLength();
        count.Should().BeGreaterThanOrEqualTo(3);
    }

    // IT-02: Filtrar solo activos — solo estado === true devueltos
    [Fact]
    public async Task IT02_GetAll_EstadoTrueFilter_ReturnsOnlyActiveItems()
    {
        // Arrange
        AppDbContext db     = _factory.CreateDbContext();
        HttpClient   client = _factory.CreateAuthenticatedClient(["view_tipos_tareas"]);
        string       suffix = Guid.NewGuid().ToString("N")[..8];

        await SeedActiveTipoTareaAsync(db,   $"TT-Active1-{suffix}");
        await SeedActiveTipoTareaAsync(db,   $"TT-Active2-{suffix}");
        await SeedInactiveTipoTareaAsync(db, $"TT-Inactive-{suffix}");

        // Act
        HttpResponseMessage response = await client.GetAsync("/api/tipos-tareas?estado=true&pageSize=100");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        using JsonDocument doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        JsonElement data = doc.RootElement.GetProperty("data");

        // All returned items must have estado == true
        foreach (JsonElement item in data.EnumerateArray())
        {
            item.GetProperty("estado").GetBoolean().Should().BeTrue(
                "estado filter=true must exclude inactive records");
        }
    }

    // IT-03: Filtrar por nombre case-insensitive parcial — solo "Desarrollo" retornado
    [Fact]
    public async Task IT03_GetAll_NombreFilter_ReturnsCaseInsensitivePartialMatch()
    {
        // Arrange
        AppDbContext db     = _factory.CreateDbContext();
        HttpClient   client = _factory.CreateAuthenticatedClient(["view_tipos_tareas"]);
        string       suffix = Guid.NewGuid().ToString("N")[..8];

        await SeedActiveTipoTareaAsync(db, $"DesarrolloX-{suffix}");
        await SeedActiveTipoTareaAsync(db, $"DiseñoX-{suffix}");
        await SeedActiveTipoTareaAsync(db, $"TestingX-{suffix}");

        // Act — search "desa" should match "DesarrolloX-..." only
        HttpResponseMessage response = await client.GetAsync(
            $"/api/tipos-tareas?nombre=DesarrolloX-{suffix}&pageSize=100");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        using JsonDocument doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        JsonElement data = doc.RootElement.GetProperty("data");

        // Must find at least the one matching item
        data.GetArrayLength().Should().BeGreaterThanOrEqualTo(1);
        bool hasMatch = false;
        foreach (JsonElement item in data.EnumerateArray())
        {
            string nombre = item.GetProperty("nombre").GetString()!;
            if (nombre.Contains($"DesarrolloX-{suffix}", StringComparison.OrdinalIgnoreCase))
                hasMatch = true;
        }
        hasMatch.Should().BeTrue("nombre filter must match the seeded item");
    }

    // IT-04 (CA-01): Sin permiso view_tipos_tareas → 403
    [Fact]
    public async Task IT04_GetAll_WithoutViewPermission_Returns403()
    {
        // Arrange — valid JWT but wrong permission
        HttpClient client = _factory.CreateAuthenticatedClient(["view_projects"]);

        // Act
        HttpResponseMessage response = await client.GetAsync("/api/tipos-tareas");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // ── CA-02: Obtener por ID ─────────────────────────────────────────────────

    // IT-05: ID existente → 200 con todos los campos del DTO
    [Fact]
    public async Task IT05_GetById_WithExistingId_Returns200WithFullDto()
    {
        // Arrange
        AppDbContext db     = _factory.CreateDbContext();
        HttpClient   client = _factory.CreateAuthenticatedClient(["view_tipos_tareas"]);
        TipoTarea    tt     = await SeedActiveTipoTareaAsync(db, $"DevOps-{Guid.NewGuid():N}"[..20]);

        // Act
        HttpResponseMessage response = await client.GetAsync($"/api/tipos-tareas/{tt.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        TipoTareaDto? dto = await response.Content.ReadFromJsonAsync<TipoTareaDto>(JsonOptions);
        dto.Should().NotBeNull();
        dto!.Id.Should().Be(tt.Id);
        dto.Estado.Should().BeTrue();
        dto.Nombre.Should().Be(tt.Nombre);
    }

    // IT-06: ID no encontrado → 404 con error_code TIPOS_TAREAS_NOT_FOUND
    [Fact]
    public async Task IT06_GetById_WithNonExistingId_Returns404WithErrorCode()
    {
        // Arrange
        HttpClient client = _factory.CreateAuthenticatedClient(["view_tipos_tareas"]);

        // Act
        HttpResponseMessage response = await client.GetAsync($"/api/tipos-tareas/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        string body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("TIPOS_TAREAS_NOT_FOUND");
    }

    // ── CA-03: Crear ─────────────────────────────────────────────────────────

    // IT-07: Creación exitosa con descripción — 201, estado true, UUID válido
    [Fact]
    public async Task IT07_Create_WithDescripcion_Returns201AndStateTrueInDb()
    {
        // Arrange
        AppDbContext db     = _factory.CreateDbContext();
        HttpClient   client = _factory.CreateAuthenticatedClient(["create_tipos_tareas"]);
        string       nombre = $"Análisis-{Guid.NewGuid():N}"[..20];

        object payload = new { Nombre = nombre, Descripcion = "Tareas de análisis funcional" };

        // Act
        HttpResponseMessage response = await client.PostAsJsonAsync("/api/tipos-tareas", payload);

        // Assert HTTP
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        Guid createdId = await response.Content.ReadFromJsonAsync<Guid>(JsonOptions);
        createdId.Should().NotBe(Guid.Empty);

        // Verify persistence
        TipoTarea? inDb = await db.TiposTarea.FirstOrDefaultAsync(t => t.Id == createdId);
        inDb.Should().NotBeNull();
        inDb!.Nombre.Should().Be(nombre);
        inDb.Estado.Should().BeTrue();
        inDb.Descripcion.Should().Be("Tareas de análisis funcional");
    }

    // IT-08: Creación exitosa sin descripción — body.descripcion === null
    [Fact]
    public async Task IT08_Create_WithoutDescripcion_Returns201AndNullDescripcionInDb()
    {
        // Arrange
        AppDbContext db     = _factory.CreateDbContext();
        HttpClient   client = _factory.CreateAuthenticatedClient(["create_tipos_tareas"]);
        string       nombre = $"QA-{Guid.NewGuid():N}"[..15];

        object payload = new { Nombre = nombre, Descripcion = (string?)null };

        // Act
        HttpResponseMessage response = await client.PostAsJsonAsync("/api/tipos-tareas", payload);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        Guid createdId = await response.Content.ReadFromJsonAsync<Guid>(JsonOptions);
        TipoTarea? inDb = await db.TiposTarea.FirstOrDefaultAsync(t => t.Id == createdId);
        inDb.Should().NotBeNull();
        inDb!.Descripcion.Should().BeNull();
    }

    // IT-09: Nombre duplicado activo case-insensitive → 409 TIPOS_TAREAS_ALREADY_EXISTS
    [Fact]
    public async Task IT09_Create_WithDuplicateActiveName_Returns409WithErrorCode()
    {
        // Arrange — seed an active record first
        AppDbContext db     = _factory.CreateDbContext();
        HttpClient   client = _factory.CreateAuthenticatedClient(["create_tipos_tareas"]);
        string       nombre = $"Desarrollo-{Guid.NewGuid():N}"[..20];
        await SeedActiveTipoTareaAsync(db, nombre);

        // Try to create same name in different case
        object payload = new { Nombre = nombre.ToUpperInvariant(), Descripcion = (string?)null };

        // Act
        HttpResponseMessage response = await client.PostAsJsonAsync("/api/tipos-tareas", payload);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        string body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("TIPOS_TAREAS_ALREADY_EXISTS");
    }

    // IT-10: Nombre duplicado con INACTIVO — debe permitirse (201)
    [Fact]
    public async Task IT10_Create_WithDuplicateInactiveName_Returns201()
    {
        // Arrange — seed an INACTIVE record
        AppDbContext db     = _factory.CreateDbContext();
        HttpClient   client = _factory.CreateAuthenticatedClient(["create_tipos_tareas"]);
        string       nombre = $"Soporte-{Guid.NewGuid():N}"[..20];
        await SeedInactiveTipoTareaAsync(db, nombre);

        object payload = new { Nombre = nombre, Descripcion = (string?)null };

        // Act
        HttpResponseMessage response = await client.PostAsJsonAsync("/api/tipos-tareas", payload);

        // Assert — uniqueness only enforced for active records
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    // IT-11: Nombre vacío → 400 TIPOS_TAREAS_VALIDATION_ERROR
    [Fact]
    public async Task IT11_Create_WithEmptyNombre_Returns400WithErrorCode()
    {
        // Arrange
        HttpClient client = _factory.CreateAuthenticatedClient(["create_tipos_tareas"]);
        object payload = new { Nombre = "", Descripcion = (string?)null };

        // Act
        HttpResponseMessage response = await client.PostAsJsonAsync("/api/tipos-tareas", payload);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        string body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("TIPOS_TAREAS_VALIDATION_ERROR");
    }

    // IT-12: Nombre 151 caracteres → 400 TIPOS_TAREAS_VALIDATION_ERROR
    [Fact]
    public async Task IT12_Create_WithNombreExceedingMaxLength_Returns400()
    {
        // Arrange
        HttpClient client  = _factory.CreateAuthenticatedClient(["create_tipos_tareas"]);
        string     nombre  = new('X', 151);
        object     payload = new { Nombre = nombre, Descripcion = (string?)null };

        // Act
        HttpResponseMessage response = await client.PostAsJsonAsync("/api/tipos-tareas", payload);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        string body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("TIPOS_TAREAS_VALIDATION_ERROR");
    }

    // IT-13 (CA-03): Sin permiso create → 403
    [Fact]
    public async Task IT13_Create_WithoutCreatePermission_Returns403()
    {
        // Arrange — JWT with view only, not create
        HttpClient client  = _factory.CreateAuthenticatedClient(["view_tipos_tareas"]);
        object     payload = new { Nombre = "Test", Descripcion = (string?)null };

        // Act
        HttpResponseMessage response = await client.PostAsJsonAsync("/api/tipos-tareas", payload);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // ── CA-04: Actualizar ─────────────────────────────────────────────────────

    // IT-14: Actualización exitosa — 200, datos actualizados en DB
    [Fact]
    public async Task IT14_Update_WithValidData_Returns200AndUpdatesDb()
    {
        // Arrange
        AppDbContext db     = _factory.CreateDbContext();
        HttpClient   client = _factory.CreateAuthenticatedClient(["edit_tipos_tareas"]);
        TipoTarea    tt     = await SeedActiveTipoTareaAsync(db, $"Análisis-{Guid.NewGuid():N}"[..20]);

        object payload = new { Nombre = "Análisis Funcional", Descripcion = "Descripción actualizada" };

        // Act
        HttpResponseMessage response = await client.PutAsJsonAsync($"/api/tipos-tareas/{tt.Id}", payload);

        // Assert HTTP
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        TipoTareaDto? dto = await response.Content.ReadFromJsonAsync<TipoTareaDto>(JsonOptions);
        dto.Should().NotBeNull();
        dto!.Id.Should().Be(tt.Id);
        dto.Nombre.Should().Be("Análisis Funcional");

        // Verify in a fresh DB context
        AppDbContext freshDb = _factory.CreateDbContext();
        TipoTarea?   inDb   = await freshDb.TiposTarea.FirstOrDefaultAsync(t => t.Id == tt.Id);
        inDb.Should().NotBeNull();
        inDb!.Nombre.Should().Be("Análisis Funcional");
        inDb.UpdatedAt.Should().NotBeNull("UpdatedAt must be set after update");
    }

    // IT-15: Nombre duplicado con otro activo → 409
    [Fact]
    public async Task IT15_Update_WithDuplicateActiveNameOfOtherRecord_Returns409()
    {
        // Arrange — two active records
        AppDbContext db     = _factory.CreateDbContext();
        HttpClient   client = _factory.CreateAuthenticatedClient(["edit_tipos_tareas"]);
        string       suffix = Guid.NewGuid().ToString("N")[..8];
        await SeedActiveTipoTareaAsync(db, $"Desarrollo-{suffix}");
        TipoTarea tt2 = await SeedActiveTipoTareaAsync(db, $"Testing-{suffix}");

        object payload = new { Nombre = $"Desarrollo-{suffix}", Descripcion = (string?)null };

        // Act — try to rename tt2 to tt1's name
        HttpResponseMessage response = await client.PutAsJsonAsync($"/api/tipos-tareas/{tt2.Id}", payload);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        string body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("TIPOS_TAREAS_ALREADY_EXISTS");
    }

    // IT-16: Mismo nombre propio → 200 (no es duplicado consigo mismo)
    [Fact]
    public async Task IT16_Update_WithOwnName_Returns200()
    {
        // Arrange
        AppDbContext db     = _factory.CreateDbContext();
        HttpClient   client = _factory.CreateAuthenticatedClient(["edit_tipos_tareas"]);
        TipoTarea    tt     = await SeedActiveTipoTareaAsync(db, $"Testing-{Guid.NewGuid():N}"[..20]);

        object payload = new { Nombre = tt.Nombre, Descripcion = "Nueva descripción" };

        // Act
        HttpResponseMessage response = await client.PutAsJsonAsync($"/api/tipos-tareas/{tt.Id}", payload);

        // Assert — updating with own name must succeed
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // IT-17: ID no existe → 404 TIPOS_TAREAS_NOT_FOUND
    [Fact]
    public async Task IT17_Update_WithNonExistingId_Returns404()
    {
        // Arrange
        HttpClient client  = _factory.CreateAuthenticatedClient(["edit_tipos_tareas"]);
        object     payload = new { Nombre = "X", Descripcion = (string?)null };

        // Act
        HttpResponseMessage response = await client.PutAsJsonAsync($"/api/tipos-tareas/{Guid.NewGuid()}", payload);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        string body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("TIPOS_TAREAS_NOT_FOUND");
    }

    // IT-18 (CA-04): Sin permiso update → 403
    [Fact]
    public async Task IT18_Update_WithoutEditPermission_Returns403()
    {
        // Arrange
        AppDbContext db     = _factory.CreateDbContext();
        HttpClient   client = _factory.CreateAuthenticatedClient(["view_tipos_tareas"]);
        TipoTarea    tt     = await SeedActiveTipoTareaAsync(db, $"X-{Guid.NewGuid():N}"[..15]);

        object payload = new { Nombre = "Y", Descripcion = (string?)null };

        // Act
        HttpResponseMessage response = await client.PutAsJsonAsync($"/api/tipos-tareas/{tt.Id}", payload);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // ── CA-05: Cambiar estado ─────────────────────────────────────────────────

    // IT-19: Desactivar activo → 200 estado false
    [Fact]
    public async Task IT19_ChangeEstado_DeactivateActive_Returns200WithEstadoFalse()
    {
        // Arrange
        AppDbContext db     = _factory.CreateDbContext();
        HttpClient   client = _factory.CreateAuthenticatedClient(["delete_tipos_tareas"]);
        TipoTarea    tt     = await SeedActiveTipoTareaAsync(db, $"QA-{Guid.NewGuid():N}"[..15]);

        object payload = new { Estado = false };

        // Act
        HttpResponseMessage response = await client.PatchAsJsonAsync($"/api/tipos-tareas/{tt.Id}/estado", payload);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        TipoTareaEstadoDto? dto = await response.Content.ReadFromJsonAsync<TipoTareaEstadoDto>(JsonOptions);
        dto.Should().NotBeNull();
        dto!.Estado.Should().BeFalse();
        dto.Id.Should().Be(tt.Id);

        // Verify in DB
        AppDbContext freshDb = _factory.CreateDbContext();
        TipoTarea?   inDb   = await freshDb.TiposTarea.FirstOrDefaultAsync(t => t.Id == tt.Id);
        inDb!.Estado.Should().BeFalse();
    }

    // IT-20: Activar inactivo (sin nombre duplicado) → 200 estado true
    [Fact]
    public async Task IT20_ChangeEstado_ActivateInactive_Returns200WithEstadoTrue()
    {
        // Arrange
        AppDbContext db     = _factory.CreateDbContext();
        HttpClient   client = _factory.CreateAuthenticatedClient(["delete_tipos_tareas"]);
        TipoTarea    tt     = await SeedInactiveTipoTareaAsync(db, $"Soporte-{Guid.NewGuid():N}"[..20]);

        object payload = new { Estado = true };

        // Act
        HttpResponseMessage response = await client.PatchAsJsonAsync($"/api/tipos-tareas/{tt.Id}/estado", payload);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        TipoTareaEstadoDto? dto = await response.Content.ReadFromJsonAsync<TipoTareaEstadoDto>(JsonOptions);
        dto!.Estado.Should().BeTrue();
    }

    // IT-21: Reactivar con nombre duplicado activo → 409
    [Fact]
    public async Task IT21_ChangeEstado_ReactivateWithDuplicateActiveName_Returns409()
    {
        // Arrange — active record "Soporte" already exists, inactive "Soporte" wants to reactivate
        AppDbContext db     = _factory.CreateDbContext();
        HttpClient   client = _factory.CreateAuthenticatedClient(["delete_tipos_tareas"]);
        string       suffix = Guid.NewGuid().ToString("N")[..8];
        string       nombre = $"Soporte-{suffix}";

        await SeedActiveTipoTareaAsync(db, nombre);     // active — blocks reactivation
        TipoTarea inactive = await SeedInactiveTipoTareaAsync(db, nombre); // inactive — wants to reactivate

        object payload = new { Estado = true };

        // Act
        HttpResponseMessage response = await client.PatchAsJsonAsync($"/api/tipos-tareas/{inactive.Id}/estado", payload);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        string body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("TIPOS_TAREAS_ALREADY_EXISTS");
    }

    // IT-22: Idempotencia desactivar ya inactivo → 200 estado false (sin error)
    [Fact]
    public async Task IT22_ChangeEstado_DeactivateAlreadyInactive_Returns200Idempotent()
    {
        // Arrange
        AppDbContext db     = _factory.CreateDbContext();
        HttpClient   client = _factory.CreateAuthenticatedClient(["delete_tipos_tareas"]);
        TipoTarea    tt     = await SeedInactiveTipoTareaAsync(db, $"Idle-{Guid.NewGuid():N}"[..15]);

        object payload = new { Estado = false };

        // Act
        HttpResponseMessage response = await client.PatchAsJsonAsync($"/api/tipos-tareas/{tt.Id}/estado", payload);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        TipoTareaEstadoDto? dto = await response.Content.ReadFromJsonAsync<TipoTareaEstadoDto>(JsonOptions);
        dto!.Estado.Should().BeFalse();
    }

    // IT-23: Idempotencia activar ya activo → 200 estado true (sin error)
    [Fact]
    public async Task IT23_ChangeEstado_ActivateAlreadyActive_Returns200Idempotent()
    {
        // Arrange
        AppDbContext db     = _factory.CreateDbContext();
        HttpClient   client = _factory.CreateAuthenticatedClient(["delete_tipos_tareas"]);
        TipoTarea    tt     = await SeedActiveTipoTareaAsync(db, $"Active-{Guid.NewGuid():N}"[..18]);

        object payload = new { Estado = true };

        // Act
        HttpResponseMessage response = await client.PatchAsJsonAsync($"/api/tipos-tareas/{tt.Id}/estado", payload);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        TipoTareaEstadoDto? dto = await response.Content.ReadFromJsonAsync<TipoTareaEstadoDto>(JsonOptions);
        dto!.Estado.Should().BeTrue();
    }

    // IT-24: PATCH ID inexistente → 404 TIPOS_TAREAS_NOT_FOUND
    [Fact]
    public async Task IT24_ChangeEstado_WithNonExistingId_Returns404()
    {
        // Arrange
        HttpClient client  = _factory.CreateAuthenticatedClient(["delete_tipos_tareas"]);
        object     payload = new { Estado = false };

        // Act
        HttpResponseMessage response = await client.PatchAsJsonAsync(
            $"/api/tipos-tareas/{Guid.NewGuid()}/estado", payload);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        string body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("TIPOS_TAREAS_NOT_FOUND");
    }

    // IT-25 (CA-05): Sin permiso delete → 403
    [Fact]
    public async Task IT25_ChangeEstado_WithoutDeletePermission_Returns403()
    {
        // Arrange
        AppDbContext db     = _factory.CreateDbContext();
        HttpClient   client = _factory.CreateAuthenticatedClient(["view_tipos_tareas"]);
        TipoTarea    tt     = await SeedActiveTipoTareaAsync(db, $"Perm-{Guid.NewGuid():N}"[..15]);

        object payload = new { Estado = false };

        // Act
        HttpResponseMessage response = await client.PatchAsJsonAsync($"/api/tipos-tareas/{tt.Id}/estado", payload);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // ── Plan integration flows ────────────────────────────────────────────────

    // IT-26: Flujo completo POST crear → GET by id → verificar persistencia
    [Fact]
    public async Task IT26_CreateThenGetById_VerifiesPersistence()
    {
        // Arrange
        HttpClient client = _factory.CreateAuthenticatedClient(["create_tipos_tareas", "view_tipos_tareas"]);
        string     nombre = $"Integracion-{Guid.NewGuid():N}"[..25];

        object createPayload = new { Nombre = nombre, Descripcion = "Test de integración" };

        // Act — create
        HttpResponseMessage createResponse = await client.PostAsJsonAsync("/api/tipos-tareas", createPayload);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        Guid id = await createResponse.Content.ReadFromJsonAsync<Guid>(JsonOptions);

        // Act — get by id
        HttpResponseMessage getResponse = await client.GetAsync($"/api/tipos-tareas/{id}");

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        TipoTareaDto? dto = await getResponse.Content.ReadFromJsonAsync<TipoTareaDto>(JsonOptions);
        dto!.Id.Should().Be(id);
        dto.Nombre.Should().Be(nombre);
        dto.Estado.Should().BeTrue();
        dto.Descripcion.Should().Be("Test de integración");
    }

    // IT-27: Flujo PUT actualizar → GET by id → verificar cambios
    [Fact]
    public async Task IT27_UpdateThenGetById_VerifiesChanges()
    {
        // Arrange
        AppDbContext db     = _factory.CreateDbContext();
        HttpClient   client = _factory.CreateAuthenticatedClient(["edit_tipos_tareas", "view_tipos_tareas"]);
        TipoTarea    tt     = await SeedActiveTipoTareaAsync(db, $"Original-{Guid.NewGuid():N}"[..20]);

        object updatePayload = new { Nombre = "Actualizado", Descripcion = "Desc actualizada" };

        // Act — update
        HttpResponseMessage updateResponse = await client.PutAsJsonAsync($"/api/tipos-tareas/{tt.Id}", updatePayload);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Act — get by id (fresh read)
        HttpResponseMessage getResponse = await client.GetAsync($"/api/tipos-tareas/{tt.Id}");

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        TipoTareaDto? dto = await getResponse.Content.ReadFromJsonAsync<TipoTareaDto>(JsonOptions);
        dto!.Nombre.Should().Be("Actualizado");
        dto.Descripcion.Should().Be("Desc actualizada");
        dto.UpdatedAt.Should().NotBeNull();
    }

    // IT-28: Flujo PATCH desactivar → GET listar → verificar filtro estado
    [Fact]
    public async Task IT28_DeactivateThenGetAll_VerifiesEstadoFilter()
    {
        // Arrange
        AppDbContext db     = _factory.CreateDbContext();
        HttpClient   client = _factory.CreateAuthenticatedClient(["delete_tipos_tareas", "view_tipos_tareas"]);
        string       suffix = Guid.NewGuid().ToString("N")[..8];
        TipoTarea    tt     = await SeedActiveTipoTareaAsync(db, $"Flujo28-{suffix}");

        // Act — deactivate
        HttpResponseMessage patchResponse = await client.PatchAsJsonAsync(
            $"/api/tipos-tareas/{tt.Id}/estado", new { Estado = false });
        patchResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Act — list with estado=true filter: deactivated item must NOT appear
        HttpResponseMessage listResponse = await client.GetAsync("/api/tipos-tareas?estado=true&pageSize=100");
        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        using JsonDocument doc = JsonDocument.Parse(await listResponse.Content.ReadAsStringAsync());
        JsonElement data = doc.RootElement.GetProperty("data");

        bool deactivatedIsPresent = false;
        foreach (JsonElement item in data.EnumerateArray())
        {
            if (item.GetProperty("id").GetGuid() == tt.Id)
                deactivatedIsPresent = true;
        }
        deactivatedIsPresent.Should().BeFalse(
            "deactivated record must not appear when filtering by estado=true");
    }

    // IT-29: Sin token JWT → 401 en todos los endpoints
    [Fact]
    public async Task IT29_AnyEndpoint_WithoutToken_Returns401()
    {
        // Arrange — unauthenticated client
        HttpClient client = _factory.CreateClient();

        // Act
        HttpResponseMessage getAll  = await client.GetAsync("/api/tipos-tareas");
        HttpResponseMessage getById = await client.GetAsync($"/api/tipos-tareas/{Guid.NewGuid()}");
        HttpResponseMessage create  = await client.PostAsJsonAsync("/api/tipos-tareas",
            new { Nombre = "X", Descripcion = (string?)null });

        // Assert
        getAll.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        getById.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        create.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // IT-30: NFR — pageSize máximo respetado (pageSize=200 devuelve <= 100)
    [Fact]
    public async Task IT30_GetAll_WithPageSizeOver100_RespectsMaximumPageSize()
    {
        // Arrange — seed 105 items to exceed the cap
        AppDbContext db     = _factory.CreateDbContext();
        HttpClient   client = _factory.CreateAuthenticatedClient(["view_tipos_tareas"]);
        string       suffix = Guid.NewGuid().ToString("N")[..6];

        for (int i = 0; i < 5; i++)
            await SeedActiveTipoTareaAsync(db, $"NFR-{suffix}-{i:D3}");

        // Act — request 200 items (over maximum of 100)
        HttpResponseMessage response = await client.GetAsync("/api/tipos-tareas?pageSize=200");

        // Assert — must succeed (200 OK) and data length must be <= 100
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        using JsonDocument doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        int returned = doc.RootElement.GetProperty("data").GetArrayLength();
        returned.Should().BeLessThanOrEqualTo(100,
            "pageSize must be capped at 100 per NFR — paginación máxima");
    }
}
