using FluentAssertions;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.UseCases.TiposTareas.Commands.ChangeTipoTareaEstado;
using Intap.FirstProject.Application.UseCases.TiposTareas.DTOs;
using Intap.FirstProject.Domain.TiposTareas;
using Intap.FirstProject.Infrastructure.Persistence;
using Intap.FirstProject.Infrastructure.Persistence.Repositories;
using Intap.FirstProject.Tests.Helpers;

namespace Intap.FirstProject.Tests.Application.TiposTareas;

public sealed class ChangeTipoTareaEstadoCommandHandlerTests
{
    private static ChangeTipoTareaEstadoCommandHandler CreateHandler(AppDbContext context) =>
        new(new TipoTareaReadRepository(context),
            new TipoTareaWriteRepository(context),
            new UnitOfWork(context));

    // Happy path: deactivate an active record
    [Fact]
    public async Task Handle_DeactivateActiveRecord_SetsEstadoFalse()
    {
        AppDbContext context = TestDbContextFactory.Create();
        TipoTarea t = TipoTarea.Create("Análisis", null);
        context.TiposTarea.Add(t);
        await context.SaveChangesAsync();

        ChangeTipoTareaEstadoCommandHandler handler = CreateHandler(context);
        Result<TipoTareaEstadoDto> result = await handler.Handle(
            new ChangeTipoTareaEstadoCommand(t.Id, false),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Estado.Should().BeFalse();

        TipoTarea? updated = context.TiposTarea.FirstOrDefault(x => x.Id == t.Id);
        updated!.Estado.Should().BeFalse();
    }

    // Happy path: reactivate an inactive record (no name conflict)
    [Fact]
    public async Task Handle_ReactivateInactiveRecord_SetsEstadoTrue()
    {
        AppDbContext context = TestDbContextFactory.Create();
        TipoTarea t = TipoTarea.Create("Testing", null);
        t.Deactivate();
        context.TiposTarea.Add(t);
        await context.SaveChangesAsync();

        ChangeTipoTareaEstadoCommandHandler handler = CreateHandler(context);
        Result<TipoTareaEstadoDto> result = await handler.Handle(
            new ChangeTipoTareaEstadoCommand(t.Id, true),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Estado.Should().BeTrue();
    }

    // Edge case: ID not found
    [Fact]
    public async Task Handle_WithNonExistingId_ReturnsNotFound()
    {
        AppDbContext context = TestDbContextFactory.Create();
        ChangeTipoTareaEstadoCommandHandler handler = CreateHandler(context);

        Result<TipoTareaEstadoDto> result = await handler.Handle(
            new ChangeTipoTareaEstadoCommand(Guid.NewGuid(), false),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorTypeResult.NotFound);
        result.Error.Code.Should().Be("TIPOS_TAREAS_NOT_FOUND");
    }

    // Edge case: deactivate already inactive (idempotent)
    [Fact]
    public async Task Handle_DeactivateAlreadyInactive_ReturnsSuccessIdempotent()
    {
        AppDbContext context = TestDbContextFactory.Create();
        TipoTarea t = TipoTarea.Create("Diseño", null);
        t.Deactivate();
        context.TiposTarea.Add(t);
        await context.SaveChangesAsync();

        ChangeTipoTareaEstadoCommandHandler handler = CreateHandler(context);
        Result<TipoTareaEstadoDto> result = await handler.Handle(
            new ChangeTipoTareaEstadoCommand(t.Id, false),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Estado.Should().BeFalse();
    }

    // Edge case: activate already active (idempotent)
    [Fact]
    public async Task Handle_ActivateAlreadyActive_ReturnsSuccessIdempotent()
    {
        AppDbContext context = TestDbContextFactory.Create();
        TipoTarea t = TipoTarea.Create("Desarrollo", null);
        context.TiposTarea.Add(t);
        await context.SaveChangesAsync();

        ChangeTipoTareaEstadoCommandHandler handler = CreateHandler(context);
        Result<TipoTareaEstadoDto> result = await handler.Handle(
            new ChangeTipoTareaEstadoCommand(t.Id, true),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Estado.Should().BeTrue();
    }

    // Edge case: reactivate but another active record has the same name
    [Fact]
    public async Task Handle_ReactivateWithDuplicateActiveName_ReturnsConflict()
    {
        AppDbContext context = TestDbContextFactory.Create();

        // t1 is active with name "Soporte"
        TipoTarea t1 = TipoTarea.Create("Soporte", null);
        // t2 is inactive with same name
        TipoTarea t2 = TipoTarea.Create("Soporte_temp", null);
        t2.Update("Soporte", null);
        t2.Deactivate();

        context.TiposTarea.AddRange(t1, t2);
        await context.SaveChangesAsync();

        ChangeTipoTareaEstadoCommandHandler handler = CreateHandler(context);
        Result<TipoTareaEstadoDto> result = await handler.Handle(
            new ChangeTipoTareaEstadoCommand(t2.Id, true),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorTypeResult.Conflict);
        result.Error.Code.Should().Be("TIPOS_TAREAS_ALREADY_EXISTS");
    }
}
