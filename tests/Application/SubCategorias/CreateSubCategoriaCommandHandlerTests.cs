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

public sealed class CreateSubCategoriaCommandHandlerTests
{
    private static CreateSubCategoriaCommandHandler CreateHandler(AppDbContext context) =>
        new(new SubCategoriaReadRepository(context),
            new SubCategoriaWriteRepository(context),
            new CategoriaReadRepository(context),
            new UnitOfWork(context),
            NullLogger<CreateSubCategoriaCommandHandler>.Instance);

    private static async Task<Categoria> SeedActiveCategoriaAsync(AppDbContext context, string nombre = "Tecnología")
    {
        Categoria categoria = Categoria.Create(nombre, null);
        context.Categorias.Add(categoria);
        await context.SaveChangesAsync();
        return categoria;
    }

    // Happy path: valid data → SubCategoria created with Estado=true
    [Fact]
    public async Task Handle_WithValidData_CreatesSubCategoria()
    {
        AppDbContext context = TestDbContextFactory.Create();
        Categoria categoria = await SeedActiveCategoriaAsync(context);
        CreateSubCategoriaCommandHandler handler = CreateHandler(context);
        CreateSubCategoriaCommand command = new("Laptops", "Equipos portátiles", categoria.Id);

        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Guid.Empty);

        SubCategoria? created = context.SubCategorias.FirstOrDefault(s => s.Id == result.Value);
        created.Should().NotBeNull();
        created!.Nombre.Should().Be("Laptops");
        created.CategoriaId.Should().Be(categoria.Id);
        created.Estado.Should().BeTrue();
    }

    // Trim: leading/trailing spaces are stripped before persisting
    [Fact]
    public async Task Handle_WithSpacesInNombre_TrimsBeforePersisting()
    {
        AppDbContext context = TestDbContextFactory.Create();
        Categoria categoria = await SeedActiveCategoriaAsync(context);
        CreateSubCategoriaCommandHandler handler = CreateHandler(context);

        Result<Guid> result = await handler.Handle(new CreateSubCategoriaCommand("  Laptops  ", null, categoria.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        SubCategoria? created = context.SubCategorias.FirstOrDefault(s => s.Id == result.Value);
        created!.Nombre.Should().Be("Laptops");
    }

    // Edge case: same Nombre in DIFFERENT CategoriaId → allowed
    [Fact]
    public async Task Handle_WithSameNombreDifferentCategoria_Succeeds()
    {
        AppDbContext context = TestDbContextFactory.Create();
        Categoria cat1 = await SeedActiveCategoriaAsync(context, "Tecnología");
        Categoria cat2 = await SeedActiveCategoriaAsync(context, "Electrónica");
        CreateSubCategoriaCommandHandler handler = CreateHandler(context);

        await handler.Handle(new CreateSubCategoriaCommand("Laptops", null, cat1.Id), CancellationToken.None);
        Result<Guid> result = await handler.Handle(new CreateSubCategoriaCommand("Laptops", null, cat2.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    // Edge case: duplicate Nombre (case-insensitive) in SAME CategoriaId → 409
    [Fact]
    public async Task Handle_WithDuplicateNombreSameCategoria_ReturnsConflict()
    {
        AppDbContext context = TestDbContextFactory.Create();
        Categoria categoria = await SeedActiveCategoriaAsync(context);
        CreateSubCategoriaCommandHandler handler = CreateHandler(context);

        await handler.Handle(new CreateSubCategoriaCommand("Laptops", null, categoria.Id), CancellationToken.None);
        Result<Guid> result = await handler.Handle(new CreateSubCategoriaCommand("laptops", null, categoria.Id), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorTypeResult.Conflict);
        result.Error.Code.Should().Be("SUBCATEGORIA_NOMBRE_DUPLICADO");
    }

    // Edge case: CategoriaId not found → 404
    [Fact]
    public async Task Handle_WithNonExistentCategoria_ReturnsNotFound()
    {
        AppDbContext context = TestDbContextFactory.Create();
        CreateSubCategoriaCommandHandler handler = CreateHandler(context);

        Result<Guid> result = await handler.Handle(
            new CreateSubCategoriaCommand("Laptops", null, Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorTypeResult.NotFound);
        result.Error.Code.Should().Be("SUBCATEGORIA_CATEGORIA_NOT_FOUND");
    }

    // Edge case: Categoria exists but Estado=false → 422
    [Fact]
    public async Task Handle_WithInactiveCategoria_ReturnsUnprocessable()
    {
        AppDbContext context = TestDbContextFactory.Create();
        Categoria categoria = Categoria.Create("Inactiva", null);
        categoria.Update(categoria.Nombre, null, false);
        context.Categorias.Add(categoria);
        await context.SaveChangesAsync();

        CreateSubCategoriaCommandHandler handler = CreateHandler(context);

        Result<Guid> result = await handler.Handle(
            new CreateSubCategoriaCommand("Laptops", null, categoria.Id), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorTypeResult.Unprocessable);
        result.Error.Code.Should().Be("SUBCATEGORIA_CATEGORIA_INACTIVA");
    }

    // Empty Nombre → validation error
    [Fact]
    public async Task Handle_WithEmptyNombre_ReturnsValidationError()
    {
        AppDbContext context = TestDbContextFactory.Create();
        Categoria categoria = await SeedActiveCategoriaAsync(context);
        CreateSubCategoriaCommandHandler handler = CreateHandler(context);

        Result<Guid> result = await handler.Handle(
            new CreateSubCategoriaCommand("   ", null, categoria.Id), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorTypeResult.Validation);
        result.Error.Code.Should().Be("SUBCATEGORIA_NOMBRE_REQUERIDO");
    }

    // Empty CategoriaId → validation error
    [Fact]
    public async Task Handle_WithEmptyCategoriaId_ReturnsValidationError()
    {
        AppDbContext context = TestDbContextFactory.Create();
        CreateSubCategoriaCommandHandler handler = CreateHandler(context);

        Result<Guid> result = await handler.Handle(
            new CreateSubCategoriaCommand("Laptops", null, Guid.Empty), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorTypeResult.Validation);
        result.Error.Code.Should().Be("SUBCATEGORIA_CATEGORIA_REQUERIDA");
    }
}
