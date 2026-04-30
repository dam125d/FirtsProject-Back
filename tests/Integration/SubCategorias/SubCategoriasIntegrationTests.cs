using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Intap.FirstProject.Application.UseCases.SubCategorias.DTOs;
using Intap.FirstProject.Domain.Categorias;
using Intap.FirstProject.Domain.SubCategorias;
using Intap.FirstProject.Infrastructure.Persistence;
using Intap.FirstProject.Tests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Intap.FirstProject.Tests.Integration.SubCategorias;

/// <summary>
/// HTTP integration tests — IT-01 to IT-17.
/// One test per Scenario in spec-tests section 12.
/// Uses WebApplicationFactory with InMemory DB — real HTTP pipeline, real handlers.
/// </summary>
public sealed class SubCategoriasIntegrationTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly IntegrationTestWebAppFactory _factory;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public SubCategoriasIntegrationTests(IntegrationTestWebAppFactory factory)
    {
        _factory = factory;
    }

    // ── Seed helpers ─────────────────────────────────────────────────────────

    private async Task<Categoria> SeedActiveCategoriaAsync(AppDbContext db, string nombre = "Tecnología")
    {
        Categoria categoria = Categoria.Create(nombre, null);
        db.Categorias.Add(categoria);
        await db.SaveChangesAsync();
        return categoria;
    }

    private async Task<Categoria> SeedInactiveCategoriaAsync(AppDbContext db, string nombre = "Inactiva")
    {
        Categoria categoria = Categoria.Create(nombre, null);
        categoria.Update(nombre, null, false);
        db.Categorias.Add(categoria);
        await db.SaveChangesAsync();
        return categoria;
    }

    private async Task<SubCategoria> SeedActiveSubCategoriaAsync(AppDbContext db, string nombre, Guid categoriaId)
    {
        SubCategoria sub = SubCategoria.Create(nombre, null, categoriaId);
        db.SubCategorias.Add(sub);
        await db.SaveChangesAsync();
        return sub;
    }

    private async Task<SubCategoria> SeedInactiveSubCategoriaAsync(AppDbContext db, string nombre, Guid categoriaId)
    {
        SubCategoria sub = SubCategoria.Create(nombre, null, categoriaId);
        sub.Deactivate();
        db.SubCategorias.Add(sub);
        await db.SaveChangesAsync();
        return sub;
    }

    // ── IT-01: Listar activas (default soloActivas=true) ─────────────────────

    [Fact]
    public async Task IT01_GetAll_WithSoloActivasDefault_Returns200WithOnlyActiveOrderedAsc()
    {
        // Arrange
        AppDbContext db       = _factory.CreateDbContext();
        HttpClient   client   = _factory.CreateAuthenticatedClient(["subcategorias:read"]);
        Categoria    categoria = await SeedActiveCategoriaAsync(db, $"Cat-IT01-{Guid.NewGuid()}");

        await SeedActiveSubCategoriaAsync(db,   "Zebra",   categoria.Id);
        await SeedActiveSubCategoriaAsync(db,   "Alpha",   categoria.Id);
        await SeedActiveSubCategoriaAsync(db,   "Mango",   categoria.Id);
        await SeedInactiveSubCategoriaAsync(db, "Inactiva", categoria.Id);

        // Act
        HttpResponseMessage response = await client.GetAsync("/api/subcategorias?soloActivas=true");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        List<SubCategoriaDto>? list = await response.Content.ReadFromJsonAsync<List<SubCategoriaDto>>(JsonOptions);
        list.Should().NotBeNull();

        // Only active ones from this category
        List<SubCategoriaDto> mine = list!.Where(s => s.CategoriaId == categoria.Id).ToList();
        mine.Should().HaveCount(3);
        mine.Select(s => s.Nombre).Should().BeInAscendingOrder();
        mine.Should().NotContain(s => s.Nombre == "Inactiva");
    }

    // ── IT-02: Listar todas incluyendo inactivas ──────────────────────────────

    [Fact]
    public async Task IT02_GetAll_WithSoloActivasFalse_Returns200WithAllSubCategorias()
    {
        // Arrange
        AppDbContext db       = _factory.CreateDbContext();
        HttpClient   client   = _factory.CreateAuthenticatedClient(["subcategorias:read"]);
        Categoria    categoria = await SeedActiveCategoriaAsync(db, $"Cat-IT02-{Guid.NewGuid()}");

        await SeedActiveSubCategoriaAsync(db,   "Active1",   categoria.Id);
        await SeedActiveSubCategoriaAsync(db,   "Active2",   categoria.Id);
        await SeedActiveSubCategoriaAsync(db,   "Active3",   categoria.Id);
        await SeedInactiveSubCategoriaAsync(db, "Inactive1", categoria.Id);

        // Act
        HttpResponseMessage response = await client.GetAsync($"/api/subcategorias?soloActivas=false&categoriaId={categoria.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        List<SubCategoriaDto>? list = await response.Content.ReadFromJsonAsync<List<SubCategoriaDto>>(JsonOptions);
        list.Should().NotBeNull();
        list!.Should().HaveCount(4);
    }

    // ── IT-03: Listar filtradas por categoriaId ────────────────────────────────

    [Fact]
    public async Task IT03_GetAll_WithCategoriaIdFilter_Returns200OnlyMatchingCategory()
    {
        // Arrange
        AppDbContext db   = _factory.CreateDbContext();
        HttpClient   client = _factory.CreateAuthenticatedClient(["subcategorias:read"]);

        Categoria cat1 = await SeedActiveCategoriaAsync(db, $"Cat1-IT03-{Guid.NewGuid()}");
        Categoria cat2 = await SeedActiveCategoriaAsync(db, $"Cat2-IT03-{Guid.NewGuid()}");

        await SeedActiveSubCategoriaAsync(db, "SubCat1A", cat1.Id);
        await SeedActiveSubCategoriaAsync(db, "SubCat1B", cat1.Id);
        await SeedActiveSubCategoriaAsync(db, "SubCat2A", cat2.Id);

        // Act
        HttpResponseMessage response = await client.GetAsync($"/api/subcategorias?categoriaId={cat1.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        List<SubCategoriaDto>? list = await response.Content.ReadFromJsonAsync<List<SubCategoriaDto>>(JsonOptions);
        list.Should().NotBeNull();

        // All returned entries must belong to cat1
        list!.Should().AllSatisfy(s => s.CategoriaId.Should().Be(cat1.Id));
        list.Should().Contain(s => s.Nombre == "SubCat1A");
        list.Should().Contain(s => s.Nombre == "SubCat1B");
        list.Should().NotContain(s => s.Nombre == "SubCat2A");
    }

    // ── IT-04: Obtener por ID existente ────────────────────────────────────────

    [Fact]
    public async Task IT04_GetById_WithExistingId_Returns200WithFullDto()
    {
        // Arrange
        AppDbContext db       = _factory.CreateDbContext();
        HttpClient   client   = _factory.CreateAuthenticatedClient(["subcategorias:read"]);
        Categoria    categoria = await SeedActiveCategoriaAsync(db, $"Cat-IT04-{Guid.NewGuid()}");
        SubCategoria sub      = await SeedActiveSubCategoriaAsync(db, "Laptops", categoria.Id);

        // Act
        HttpResponseMessage response = await client.GetAsync($"/api/subcategorias/{sub.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        SubCategoriaDto? dto = await response.Content.ReadFromJsonAsync<SubCategoriaDto>(JsonOptions);
        dto.Should().NotBeNull();
        dto!.Id.Should().Be(sub.Id);
        dto.Nombre.Should().Be("Laptops");
        dto.CategoriaId.Should().Be(categoria.Id);
        dto.Estado.Should().BeTrue();
    }

    // ── IT-05: Obtener por ID inexistente ──────────────────────────────────────

    [Fact]
    public async Task IT05_GetById_WithNonExistingId_Returns404WithErrorCode()
    {
        // Arrange
        HttpClient client = _factory.CreateAuthenticatedClient(["subcategorias:read"]);

        // Act
        HttpResponseMessage response = await client.GetAsync($"/api/subcategorias/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        string body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("SUBCATEGORIA_NOT_FOUND");
    }

    // ── IT-06: Crear sub-categoría válida ─────────────────────────────────────

    [Fact]
    public async Task IT06_Create_WithValidData_Returns201AndStateTrueInDb()
    {
        // Arrange
        AppDbContext db       = _factory.CreateDbContext();
        HttpClient   client   = _factory.CreateAuthenticatedClient(["subcategorias:create"]);
        Categoria    categoria = await SeedActiveCategoriaAsync(db, $"Cat-IT06-{Guid.NewGuid()}");

        object payload = new { Nombre = "Laptops", Descripcion = "Equipos portátiles", CategoriaId = categoria.Id };

        // Act
        HttpResponseMessage response = await client.PostAsJsonAsync("/api/subcategorias", payload);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        // Verify in DB
        Guid createdId = await response.Content.ReadFromJsonAsync<Guid>(JsonOptions);
        SubCategoria? inDb = await db.SubCategorias.FirstOrDefaultAsync(s => s.Id == createdId);
        inDb.Should().NotBeNull();
        inDb!.Nombre.Should().Be("Laptops");
        inDb.Estado.Should().BeTrue();
        inDb.CategoriaId.Should().Be(categoria.Id);
    }

    // ── IT-07: Crear duplicado case-insensitive misma categoría ───────────────

    [Fact]
    public async Task IT07_Create_WithDuplicateNombreSameCategoria_Returns409WithErrorCode()
    {
        // Arrange
        AppDbContext db       = _factory.CreateDbContext();
        HttpClient   client   = _factory.CreateAuthenticatedClient(["subcategorias:create"]);
        Categoria    categoria = await SeedActiveCategoriaAsync(db, $"Cat-IT07-{Guid.NewGuid()}");
        await SeedActiveSubCategoriaAsync(db, "Laptops", categoria.Id);

        object payload = new { Nombre = "laptops", Descripcion = (string?)null, CategoriaId = categoria.Id };

        // Act
        HttpResponseMessage response = await client.PostAsJsonAsync("/api/subcategorias", payload);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        string body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("SUBCATEGORIA_NOMBRE_DUPLICADO");
    }

    // ── IT-08: Crear mismo nombre diferente categoría (CRITICAL) ─────────────

    [Fact]
    public async Task IT08_Create_WithSameNombreDifferentCategoria_Returns201()
    {
        // Arrange — uniqueness is per category, so same name in another category is ALLOWED
        AppDbContext db   = _factory.CreateDbContext();
        HttpClient   client = _factory.CreateAuthenticatedClient(["subcategorias:create"]);

        Categoria cat1 = await SeedActiveCategoriaAsync(db, $"Cat1-IT08-{Guid.NewGuid()}");
        Categoria cat2 = await SeedActiveCategoriaAsync(db, $"Cat2-IT08-{Guid.NewGuid()}");
        await SeedActiveSubCategoriaAsync(db, "Laptops", cat1.Id);

        object payload = new { Nombre = "Laptops", Descripcion = (string?)null, CategoriaId = cat2.Id };

        // Act
        HttpResponseMessage response = await client.PostAsJsonAsync("/api/subcategorias", payload);

        // Assert — MUST be 201 because different category
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    // ── IT-09: Crear sin nombre ────────────────────────────────────────────────

    [Fact]
    public async Task IT09_Create_WithEmptyNombre_Returns400WithErrorCode()
    {
        // Arrange
        AppDbContext db       = _factory.CreateDbContext();
        HttpClient   client   = _factory.CreateAuthenticatedClient(["subcategorias:create"]);
        Categoria    categoria = await SeedActiveCategoriaAsync(db, $"Cat-IT09-{Guid.NewGuid()}");

        object payload = new { Nombre = "", Descripcion = (string?)null, CategoriaId = categoria.Id };

        // Act
        HttpResponseMessage response = await client.PostAsJsonAsync("/api/subcategorias", payload);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        string body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("SUBCATEGORIA_NOMBRE_REQUERIDO");
    }

    // ── IT-10: Crear con categoría inexistente ────────────────────────────────

    [Fact]
    public async Task IT10_Create_WithNonExistentCategoriaId_Returns404WithErrorCode()
    {
        // Arrange
        HttpClient client = _factory.CreateAuthenticatedClient(["subcategorias:create"]);

        object payload = new { Nombre = "Test", Descripcion = (string?)null, CategoriaId = Guid.NewGuid() };

        // Act
        HttpResponseMessage response = await client.PostAsJsonAsync("/api/subcategorias", payload);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        string body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("SUBCATEGORIA_CATEGORIA_NOT_FOUND");
    }

    // ── IT-11: Crear con categoría inactiva ───────────────────────────────────

    [Fact]
    public async Task IT11_Create_WithInactiveCategoria_Returns422WithErrorCode()
    {
        // Arrange
        AppDbContext db       = _factory.CreateDbContext();
        HttpClient   client   = _factory.CreateAuthenticatedClient(["subcategorias:create"]);
        Categoria    categoria = await SeedInactiveCategoriaAsync(db, $"CatInactiva-IT11-{Guid.NewGuid()}");

        object payload = new { Nombre = "Test", Descripcion = (string?)null, CategoriaId = categoria.Id };

        // Act
        HttpResponseMessage response = await client.PostAsJsonAsync("/api/subcategorias", payload);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        string body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("SUBCATEGORIA_CATEGORIA_INACTIVA");
    }

    // ── IT-12: Actualizar sub-categoría exitosamente ──────────────────────────

    [Fact]
    public async Task IT12_Update_WithValidData_Returns200AndUpdatedAtChangedInDb()
    {
        // Arrange
        AppDbContext db       = _factory.CreateDbContext();
        HttpClient   client   = _factory.CreateAuthenticatedClient(["subcategorias:update"]);
        Categoria    categoria = await SeedActiveCategoriaAsync(db, $"Cat-IT12-{Guid.NewGuid()}");
        SubCategoria sub      = await SeedActiveSubCategoriaAsync(db, "Laptops", categoria.Id);

        DateTime? originalUpdatedAt = sub.UpdatedAt;

        object payload = new
        {
            Nombre      = "Notebooks",
            Descripcion = "Portátiles de alta gama",
            CategoriaId = categoria.Id,
            Estado      = true,
        };

        // Act
        HttpResponseMessage response = await client.PutAsJsonAsync($"/api/subcategorias/{sub.Id}", payload);

        // Assert HTTP
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert DB — UpdatedAt must have been set
        AppDbContext freshDb = _factory.CreateDbContext();
        SubCategoria? inDb   = await freshDb.SubCategorias.FirstOrDefaultAsync(s => s.Id == sub.Id);
        inDb.Should().NotBeNull();
        inDb!.Nombre.Should().Be("Notebooks");
        inDb.UpdatedAt.Should().NotBe(originalUpdatedAt);
        inDb.UpdatedAt.Should().NotBeNull();
    }

    // ── IT-13: Actualizar sub-categoría inexistente ───────────────────────────

    [Fact]
    public async Task IT13_Update_WithNonExistingId_Returns404WithErrorCode()
    {
        // Arrange
        AppDbContext db       = _factory.CreateDbContext();
        HttpClient   client   = _factory.CreateAuthenticatedClient(["subcategorias:update"]);
        Categoria    categoria = await SeedActiveCategoriaAsync(db, $"Cat-IT13-{Guid.NewGuid()}");

        object payload = new
        {
            Nombre      = "X",
            Descripcion = (string?)null,
            CategoriaId = categoria.Id,
            Estado      = true,
        };

        // Act
        HttpResponseMessage response = await client.PutAsJsonAsync($"/api/subcategorias/{Guid.NewGuid()}", payload);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        string body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("SUBCATEGORIA_NOT_FOUND");
    }

    // ── IT-14: Desactivar sub-categoría activa ────────────────────────────────

    [Fact]
    public async Task IT14_Deactivate_WithActiveSubCategoria_Returns204AndStateFalseInDb()
    {
        // Arrange
        AppDbContext db       = _factory.CreateDbContext();
        HttpClient   client   = _factory.CreateAuthenticatedClient(["subcategorias:delete"]);
        Categoria    categoria = await SeedActiveCategoriaAsync(db, $"Cat-IT14-{Guid.NewGuid()}");
        SubCategoria sub      = await SeedActiveSubCategoriaAsync(db, "Laptops", categoria.Id);

        // Act
        HttpResponseMessage response = await client.DeleteAsync($"/api/subcategorias/{sub.Id}");

        // Assert HTTP
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Assert DB — Estado must be false
        AppDbContext freshDb = _factory.CreateDbContext();
        SubCategoria? inDb   = await freshDb.SubCategorias.FirstOrDefaultAsync(s => s.Id == sub.Id);
        inDb.Should().NotBeNull();
        inDb!.Estado.Should().BeFalse();
    }

    // ── IT-15: Desactivar ya inactiva (idempotente, CRITICAL) ─────────────────

    [Fact]
    public async Task IT15_Deactivate_AlreadyInactive_Returns204WithoutModifyingDb()
    {
        // Arrange
        AppDbContext db       = _factory.CreateDbContext();
        HttpClient   client   = _factory.CreateAuthenticatedClient(["subcategorias:delete"]);
        Categoria    categoria = await SeedActiveCategoriaAsync(db, $"Cat-IT15-{Guid.NewGuid()}");
        SubCategoria sub      = await SeedInactiveSubCategoriaAsync(db, "AlreadyInactive", categoria.Id);

        // Capture UpdatedAt before second DELETE
        DateTime? updatedAtBeforeSecondDelete = sub.UpdatedAt;

        // Act — DELETE on already-inactive entity
        HttpResponseMessage response = await client.DeleteAsync($"/api/subcategorias/{sub.Id}");

        // Assert HTTP — must be 204 (idempotent)
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Assert DB — UpdatedAt must NOT have changed (no second write)
        AppDbContext freshDb = _factory.CreateDbContext();
        SubCategoria? inDb   = await freshDb.SubCategorias.FirstOrDefaultAsync(s => s.Id == sub.Id);
        inDb.Should().NotBeNull();
        inDb!.Estado.Should().BeFalse();
        inDb.UpdatedAt.Should().Be(updatedAtBeforeSecondDelete);
    }

    // ── IT-16: Request sin token JWT ──────────────────────────────────────────

    [Fact]
    public async Task IT16_AnyEndpoint_WithoutToken_Returns401()
    {
        // Arrange — unauthenticated client (no Authorization header)
        HttpClient client = _factory.CreateClient();

        // Act
        HttpResponseMessage getAll  = await client.GetAsync("/api/subcategorias");
        HttpResponseMessage getById = await client.GetAsync($"/api/subcategorias/{Guid.NewGuid()}");
        HttpResponseMessage create  = await client.PostAsJsonAsync("/api/subcategorias",
            new { Nombre = "X", Descripcion = (string?)null, CategoriaId = Guid.NewGuid() });

        // Assert all → 401
        getAll.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        getById.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        create.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // ── IT-17: Request sin permiso ────────────────────────────────────────────

    [Fact]
    public async Task IT17_AnyEndpoint_WithoutRequiredPermission_Returns403()
    {
        // Arrange — valid JWT but WITHOUT subcategorias permissions
        HttpClient client = _factory.CreateAuthenticatedClient(["view_projects"]);

        AppDbContext db       = _factory.CreateDbContext();
        Categoria    categoria = await SeedActiveCategoriaAsync(db, $"Cat-IT17-{Guid.NewGuid()}");

        // Act — try to CREATE without subcategorias:create
        HttpResponseMessage response = await client.PostAsJsonAsync("/api/subcategorias",
            new { Nombre = "X", Descripcion = (string?)null, CategoriaId = categoria.Id });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
