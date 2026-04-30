using FluentAssertions;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.UseCases.Categorias.Commands.DeactivateCategoria;
using Intap.FirstProject.Domain.Categorias;
using Intap.FirstProject.Domain.Projects;
using Intap.FirstProject.Infrastructure.Persistence;
using Intap.FirstProject.Infrastructure.Persistence.Repositories;
using Intap.FirstProject.Tests.Helpers;

namespace Intap.FirstProject.Tests.Application.Categorias;

public sealed class DeactivateCategoriaCommandHandlerTests
{
    private static DeactivateCategoriaCommandHandler CreateHandler(AppDbContext context) =>
        new(new CategoriaReadRepository(context),
            new CategoriaWriteRepository(context),
            new UnitOfWork(context));

    // IT-09: Desactivar categoría sin proyectos asociados → 204 + Estado=false en BD
    [Fact]
    public async Task Handle_WithNoprojects_DeactivatesCategoria()
    {
        // Arrange
        AppDbContext context = TestDbContextFactory.Create();

        Categoria categoria = Categoria.Create("Tecnología", null);
        context.Categorias.Add(categoria);
        await context.SaveChangesAsync();

        DeactivateCategoriaCommandHandler handler = CreateHandler(context);

        // Act
        Result result = await handler.Handle(new DeactivateCategoriaCommand(categoria.Id), CancellationToken.None);

        // Assert — IT-09: 204 + Estado=false en BD
        result.IsSuccess.Should().BeTrue();

        Categoria? updated = context.Categorias.FirstOrDefault(c => c.Id == categoria.Id);
        updated!.Estado.Should().BeFalse();
        updated.UpdatedAt.Should().NotBeNull();
    }

    // IT-10: Desactivar categoría en uso por proyectos activos → 409
    [Fact]
    public async Task Handle_WithActiveProjects_ReturnsConflict()
    {
        // Arrange
        AppDbContext context = TestDbContextFactory.Create();

        Categoria categoria = Categoria.Create("Tecnología", null);
        context.Categorias.Add(categoria);

        Project project = Project.Create(
            "Proyecto A", "Cliente A", null, [],
            DateOnly.FromDateTime(DateTime.Today), null);
        project.AssignCategoria(categoria.Id);
        context.Projects.Add(project);

        await context.SaveChangesAsync();

        DeactivateCategoriaCommandHandler handler = CreateHandler(context);

        // Act
        Result result = await handler.Handle(new DeactivateCategoriaCommand(categoria.Id), CancellationToken.None);

        // Assert — IT-10: 409 con error_code CATEGORIA_EN_USO
        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorTypeResult.Conflict);
        result.Error.Code.Should().Be("Categoria.EnUso");
    }

    [Fact]
    public async Task Handle_WithNonExistingId_ReturnsNotFound()
    {
        // Arrange
        AppDbContext context = TestDbContextFactory.Create();
        DeactivateCategoriaCommandHandler handler = CreateHandler(context);

        // Act
        Result result = await handler.Handle(new DeactivateCategoriaCommand(Guid.NewGuid()), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorTypeResult.NotFound);
        result.Error.Code.Should().Be("Categoria.NotFound");
    }

    // IT-10 variant: proyecto eliminado (IsDeleted=true) no bloquea la desactivación
    [Fact]
    public async Task Handle_WithDeletedProjectOnly_Deactivates()
    {
        // Arrange
        AppDbContext context = TestDbContextFactory.Create();

        Categoria categoria = Categoria.Create("Archivada", null);
        context.Categorias.Add(categoria);

        Project project = Project.Create(
            "Proyecto Eliminado", "Cliente", null, [],
            DateOnly.FromDateTime(DateTime.Today), null);
        project.AssignCategoria(categoria.Id);
        project.Delete();
        context.Projects.Add(project);

        await context.SaveChangesAsync();

        DeactivateCategoriaCommandHandler handler = CreateHandler(context);

        // Act
        Result result = await handler.Handle(new DeactivateCategoriaCommand(categoria.Id), CancellationToken.None);

        // Assert — proyecto eliminado no cuenta como "en uso"
        result.IsSuccess.Should().BeTrue();
    }
}
