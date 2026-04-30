using FluentAssertions;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.UseCases.SubCategorias.Commands.UpdateSubCategoria;
using Intap.FirstProject.Domain.Categorias;
using Intap.FirstProject.Domain.SubCategorias;
using Intap.FirstProject.Infrastructure.Persistence;
using Intap.FirstProject.Infrastructure.Persistence.Repositories;
using Intap.FirstProject.Tests.Helpers;

namespace Intap.FirstProject.Tests.Application.SubCategorias;

public sealed class UpdateSubCategoriaCommandHandlerTests
{
    private static UpdateSubCategoriaCommandHandler CreateHandler(AppDbContext context) =>
        new(new SubCategoriaReadRepository(context),
            new SubCategoriaWriteRepository(context),
            new CategoriaReadRepository(context),
            new UnitOfWork(context));

    private static async Task<(Categoria, SubCategoria)> SeedAsync(AppDbContext context)
    {
        Categoria categoria = Categoria.Create("Tecnología", null);
        context.Categorias.Add(categoria);

        SubCategoria subCategoria = SubCategoria.Create("Laptops", null, categoria.Id);
        context.SubCategorias.Add(subCategoria);
        await context.SaveChangesAsync();
        return (categoria, subCategoria);
    }

    // Happy path: valid update
    [Fact]
    public async Task Handle_WithValidData_UpdatesSubCategoria()
    {
        AppDbContext context = TestDbContextFactory.Create();
        (Categoria categoria, SubCategoria subCategoria) = await SeedAsync(context);
        UpdateSubCategoriaCommandHandler handler = CreateHandler(context);

        Result result = await handler.Handle(
            new UpdateSubCategoriaCommand(subCategoria.Id, "Tablets", "Dispositivos táctiles", categoria.Id, true),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        SubCategoria? updated = context.SubCategorias.FirstOrDefault(s => s.Id == subCategoria.Id);
        updated!.Nombre.Should().Be("Tablets");
        updated.Descripcion.Should().Be("Dispositivos táctiles");
        updated.UpdatedAt.Should().NotBeNull();
    }

    // Edge case: non-existing Id → 404
    [Fact]
    public async Task Handle_WithNonExistingId_ReturnsNotFound()
    {
        AppDbContext context = TestDbContextFactory.Create();
        UpdateSubCategoriaCommandHandler handler = CreateHandler(context);

        Result result = await handler.Handle(
            new UpdateSubCategoriaCommand(Guid.NewGuid(), "Tablets", null, Guid.NewGuid(), true),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorTypeResult.NotFound);
        result.Error.Code.Should().Be("SUBCATEGORIA_NOT_FOUND");
    }

    // Edge case: change to inactive CategoriaId → 422
    [Fact]
    public async Task Handle_WithCategoriaIdChangedToInactive_ReturnsUnprocessable()
    {
        AppDbContext context = TestDbContextFactory.Create();
        (_, SubCategoria subCategoria) = await SeedAsync(context);

        Categoria inactiveCategoria = Categoria.Create("Inactiva", null);
        inactiveCategoria.Update(inactiveCategoria.Nombre, null, false);
        context.Categorias.Add(inactiveCategoria);
        await context.SaveChangesAsync();

        UpdateSubCategoriaCommandHandler handler = CreateHandler(context);

        Result result = await handler.Handle(
            new UpdateSubCategoriaCommand(subCategoria.Id, "Tablets", null, inactiveCategoria.Id, true),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorTypeResult.Unprocessable);
        result.Error.Code.Should().Be("SUBCATEGORIA_CATEGORIA_INACTIVA");
    }

    // Edge case: duplicate Nombre excluding self → 409
    [Fact]
    public async Task Handle_WithDuplicateNombreExcludingSelf_ReturnsConflict()
    {
        AppDbContext context = TestDbContextFactory.Create();
        (Categoria categoria, SubCategoria subCategoria) = await SeedAsync(context);

        // Seed a second SubCategoria with the name we'll try to assign
        SubCategoria other = SubCategoria.Create("Tablets", null, categoria.Id);
        context.SubCategorias.Add(other);
        await context.SaveChangesAsync();

        UpdateSubCategoriaCommandHandler handler = CreateHandler(context);

        Result result = await handler.Handle(
            new UpdateSubCategoriaCommand(subCategoria.Id, "Tablets", null, categoria.Id, true),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorTypeResult.Conflict);
        result.Error.Code.Should().Be("SUBCATEGORIA_NOMBRE_DUPLICADO");
    }

    // Null Descripcion in PUT → accepted (saved as null)
    [Fact]
    public async Task Handle_WithNullDescripcion_PersistsNull()
    {
        AppDbContext context = TestDbContextFactory.Create();
        (Categoria categoria, SubCategoria subCategoria) = await SeedAsync(context);
        UpdateSubCategoriaCommandHandler handler = CreateHandler(context);

        Result result = await handler.Handle(
            new UpdateSubCategoriaCommand(subCategoria.Id, "Laptops", null, categoria.Id, true),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        SubCategoria? updated = context.SubCategorias.FirstOrDefault(s => s.Id == subCategoria.Id);
        updated!.Descripcion.Should().BeNull();
    }
}
