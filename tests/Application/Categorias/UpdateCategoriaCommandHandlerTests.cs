using FluentAssertions;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.UseCases.Categorias.Commands.CreateCategoria;
using Intap.FirstProject.Application.UseCases.Categorias.Commands.UpdateCategoria;
using Intap.FirstProject.Domain.Categorias;
using Intap.FirstProject.Infrastructure.Persistence;
using Intap.FirstProject.Infrastructure.Persistence.Repositories;
using Intap.FirstProject.Tests.Helpers;

namespace Intap.FirstProject.Tests.Application.Categorias;

public sealed class UpdateCategoriaCommandHandlerTests
{
    private static UpdateCategoriaCommandHandler CreateHandler(AppDbContext context) =>
        new(new CategoriaReadRepository(context),
            new CategoriaWriteRepository(context),
            new UnitOfWork(context));

    private static async Task<Guid> SeedCategoria(AppDbContext context, string nombre = "Original")
    {
        Categoria c = Categoria.Create(nombre, null);
        context.Categorias.Add(c);
        await context.SaveChangesAsync();
        return c.Id;
    }

    // IT-08: Actualizar categoría exitosamente
    [Fact]
    public async Task Handle_WithValidData_UpdatesCategoria()
    {
        // Arrange
        AppDbContext context = TestDbContextFactory.Create();
        Guid id = await SeedCategoria(context, "Tecnología");
        UpdateCategoriaCommandHandler handler = CreateHandler(context);

        // Act
        Result result = await handler.Handle(
            new UpdateCategoriaCommand(id, "Innovación", "Nueva descripción", true),
            CancellationToken.None);

        // Assert — IT-08: 200 + nombre actualizado en BD
        result.IsSuccess.Should().BeTrue();

        Categoria? updated = context.Categorias.FirstOrDefault(c => c.Id == id);
        updated!.Nombre.Should().Be("Innovación");
        updated.Descripcion.Should().Be("Nueva descripción");
        updated.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithNonExistingId_ReturnsNotFound()
    {
        // Arrange
        AppDbContext context = TestDbContextFactory.Create();
        UpdateCategoriaCommandHandler handler = CreateHandler(context);

        // Act
        Result result = await handler.Handle(
            new UpdateCategoriaCommand(Guid.NewGuid(), "Nombre", null, true),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorTypeResult.NotFound);
        result.Error.Code.Should().Be("Categoria.NotFound");
    }

    [Fact]
    public async Task Handle_WithDuplicateNombreFromOtherRecord_ReturnsConflict()
    {
        // Arrange
        AppDbContext context = TestDbContextFactory.Create();

        Categoria cat1 = Categoria.Create("Tecnología", null);
        Categoria cat2 = Categoria.Create("Marketing", null);
        context.Categorias.AddRange(cat1, cat2);
        await context.SaveChangesAsync();

        UpdateCategoriaCommandHandler handler = CreateHandler(context);

        // Act — intentar renombrar cat2 a "Tecnología" (ya existe en cat1)
        Result result = await handler.Handle(
            new UpdateCategoriaCommand(cat2.Id, "Tecnología", null, true),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorTypeResult.Conflict);
        result.Error.Code.Should().Be("Categoria.NombreDuplicado");
    }

    [Fact]
    public async Task Handle_WithSameNombreAsOwnRecord_Succeeds()
    {
        // Arrange — actualizar con el mismo nombre que ya tiene (debe ser permitido)
        AppDbContext context = TestDbContextFactory.Create();
        Guid id = await SeedCategoria(context, "Tecnología");
        UpdateCategoriaCommandHandler handler = CreateHandler(context);

        // Act
        Result result = await handler.Handle(
            new UpdateCategoriaCommand(id, "Tecnología", "Nueva descripción", true),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_DeactivateViaUpdate_SetsEstadoFalse()
    {
        // Arrange
        AppDbContext context = TestDbContextFactory.Create();
        Guid id = await SeedCategoria(context);
        UpdateCategoriaCommandHandler handler = CreateHandler(context);

        // Act
        Result result = await handler.Handle(
            new UpdateCategoriaCommand(id, "Original", null, false),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        Categoria? updated = context.Categorias.FirstOrDefault(c => c.Id == id);
        updated!.Estado.Should().BeFalse();
    }
}
