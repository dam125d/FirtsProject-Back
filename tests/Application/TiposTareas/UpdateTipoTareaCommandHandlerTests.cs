using FluentAssertions;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.UseCases.TiposTareas.Commands.UpdateTipoTarea;
using Intap.FirstProject.Domain.TiposTareas;
using Intap.FirstProject.Infrastructure.Persistence;
using Intap.FirstProject.Infrastructure.Persistence.Repositories;
using Intap.FirstProject.Tests.Helpers;

namespace Intap.FirstProject.Tests.Application.TiposTareas;

public sealed class UpdateTipoTareaCommandHandlerTests
{
    private static UpdateTipoTareaCommandHandler CreateHandler(AppDbContext context) =>
        new(new TipoTareaReadRepository(context),
            new TipoTareaWriteRepository(context),
            new UnitOfWork(context));

    // Happy path: update with a new unique name
    [Fact]
    public async Task Handle_WithValidData_UpdatesTipoTarea()
    {
        AppDbContext context = TestDbContextFactory.Create();
        TipoTarea existing = TipoTarea.Create("Soporte", "Soporte técnico");
        context.TiposTarea.Add(existing);
        await context.SaveChangesAsync();

        UpdateTipoTareaCommandHandler handler = CreateHandler(context);
        Result<Guid> result = await handler.Handle(
            new UpdateTipoTareaCommand(existing.Id, "Soporte Técnico", "Actualizado"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        TipoTarea? updated = context.TiposTarea.FirstOrDefault(t => t.Id == existing.Id);
        updated!.Nombre.Should().Be("Soporte Técnico");
        updated.Descripcion.Should().Be("Actualizado");
        updated.UpdatedAt.Should().NotBeNull();
    }

    // Edge case: ID not found
    [Fact]
    public async Task Handle_WithNonExistingId_ReturnsNotFound()
    {
        AppDbContext context = TestDbContextFactory.Create();
        UpdateTipoTareaCommandHandler handler = CreateHandler(context);

        Result<Guid> result = await handler.Handle(
            new UpdateTipoTareaCommand(Guid.NewGuid(), "Nombre", null),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorTypeResult.NotFound);
        result.Error.Code.Should().Be("TIPOS_TAREAS_NOT_FOUND");
    }

    // Edge case: name already used by another active record
    [Fact]
    public async Task Handle_WithNameUsedByAnotherActive_ReturnsConflict()
    {
        AppDbContext context = TestDbContextFactory.Create();
        TipoTarea t1 = TipoTarea.Create("Diseño", null);
        TipoTarea t2 = TipoTarea.Create("Desarrollo", null);
        context.TiposTarea.AddRange(t1, t2);
        await context.SaveChangesAsync();

        UpdateTipoTareaCommandHandler handler = CreateHandler(context);
        Result<Guid> result = await handler.Handle(
            new UpdateTipoTareaCommand(t2.Id, "diseño", null),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorTypeResult.Conflict);
        result.Error.Code.Should().Be("TIPOS_TAREAS_ALREADY_EXISTS");
    }

    // Updating with its own same name should succeed (exclude itself from uniqueness check)
    [Fact]
    public async Task Handle_UpdateWithSameName_Succeeds()
    {
        AppDbContext context = TestDbContextFactory.Create();
        TipoTarea existing = TipoTarea.Create("Análisis", "Desc");
        context.TiposTarea.Add(existing);
        await context.SaveChangesAsync();

        UpdateTipoTareaCommandHandler handler = CreateHandler(context);
        Result<Guid> result = await handler.Handle(
            new UpdateTipoTareaCommand(existing.Id, "Análisis", "Desc actualizada"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }
}
