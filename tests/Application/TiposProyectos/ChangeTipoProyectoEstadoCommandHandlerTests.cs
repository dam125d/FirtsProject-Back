using FluentAssertions;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.UseCases.TiposProyectos.Commands.ChangeTipoProyectoEstado;
using Intap.FirstProject.Application.UseCases.TiposProyectos.DTOs;
using Intap.FirstProject.Domain.TiposProyectos;
using Intap.FirstProject.Infrastructure.Persistence;
using Intap.FirstProject.Infrastructure.Persistence.Repositories;
using Intap.FirstProject.Tests.Helpers;

namespace Intap.FirstProject.Tests.Application.TiposProyectos;

public sealed class ChangeTipoProyectoEstadoCommandHandlerTests
{
    private static ChangeTipoProyectoEstadoCommandHandler CreateHandler(AppDbContext context) =>
        new(new TipoProyectoReadRepository(context),
            new TipoProyectoWriteRepository(context),
            new UnitOfWork(context));

    // Happy path: deactivate an active record
    [Fact]
    public async Task Handle_DeactivateActiveRecord_SetsEstadoFalse()
    {
        AppDbContext context = TestDbContextFactory.Create();
        TipoProyecto t = TipoProyecto.Create("Desarrollo", null);
        context.TiposProyecto.Add(t);
        await context.SaveChangesAsync();

        ChangeTipoProyectoEstadoCommandHandler handler = CreateHandler(context);
        Result<TipoProyectoEstadoDto> result = await handler.Handle(
            new ChangeTipoProyectoEstadoCommand(t.Id, false),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Estado.Should().BeFalse();

        TipoProyecto? updated = context.TiposProyecto.FirstOrDefault(x => x.Id == t.Id);
        updated!.Estado.Should().BeFalse();
    }

    // Happy path: reactivate an inactive record (no name conflict)
    [Fact]
    public async Task Handle_ReactivateInactiveRecord_SetsEstadoTrue()
    {
        AppDbContext context = TestDbContextFactory.Create();
        TipoProyecto t = TipoProyecto.Create("Marketing", null);
        t.Deactivate();
        context.TiposProyecto.Add(t);
        await context.SaveChangesAsync();

        ChangeTipoProyectoEstadoCommandHandler handler = CreateHandler(context);
        Result<TipoProyectoEstadoDto> result = await handler.Handle(
            new ChangeTipoProyectoEstadoCommand(t.Id, true),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Estado.Should().BeTrue();
    }

    // Edge case: ID not found
    [Fact]
    public async Task Handle_WithNonExistingId_ReturnsNotFound()
    {
        AppDbContext context = TestDbContextFactory.Create();
        ChangeTipoProyectoEstadoCommandHandler handler = CreateHandler(context);

        Result<TipoProyectoEstadoDto> result = await handler.Handle(
            new ChangeTipoProyectoEstadoCommand(Guid.NewGuid(), false),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorTypeResult.NotFound);
        result.Error.Code.Should().Be("TIPOS_PROYECTOS_NOT_FOUND");
    }

    // Edge case: deactivate already inactive (idempotent)
    [Fact]
    public async Task Handle_DeactivateAlreadyInactive_ReturnsSuccessIdempotent()
    {
        AppDbContext context = TestDbContextFactory.Create();
        TipoProyecto t = TipoProyecto.Create("Consultoría", null);
        t.Deactivate();
        context.TiposProyecto.Add(t);
        await context.SaveChangesAsync();

        ChangeTipoProyectoEstadoCommandHandler handler = CreateHandler(context);
        Result<TipoProyectoEstadoDto> result = await handler.Handle(
            new ChangeTipoProyectoEstadoCommand(t.Id, false),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Estado.Should().BeFalse();
    }

    // Edge case: activate already active (idempotent)
    [Fact]
    public async Task Handle_ActivateAlreadyActive_ReturnsSuccessIdempotent()
    {
        AppDbContext context = TestDbContextFactory.Create();
        TipoProyecto t = TipoProyecto.Create("Infraestructura", null);
        context.TiposProyecto.Add(t);
        await context.SaveChangesAsync();

        ChangeTipoProyectoEstadoCommandHandler handler = CreateHandler(context);
        Result<TipoProyectoEstadoDto> result = await handler.Handle(
            new ChangeTipoProyectoEstadoCommand(t.Id, true),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Estado.Should().BeTrue();
    }

    // Edge case: reactivate but another active record has the same name
    [Fact]
    public async Task Handle_ReactivateWithDuplicateActiveName_ReturnsConflict()
    {
        AppDbContext context = TestDbContextFactory.Create();

        // t1 is active with name "Tecnología"
        TipoProyecto t1 = TipoProyecto.Create("Tecnología", null);
        // t2 is inactive with same name
        TipoProyecto t2 = TipoProyecto.Create("Tecnología_temp", null);
        t2.Update("Tecnología", null);
        t2.Deactivate();

        context.TiposProyecto.AddRange(t1, t2);
        await context.SaveChangesAsync();

        ChangeTipoProyectoEstadoCommandHandler handler = CreateHandler(context);
        Result<TipoProyectoEstadoDto> result = await handler.Handle(
            new ChangeTipoProyectoEstadoCommand(t2.Id, true),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorTypeResult.Conflict);
        result.Error.Code.Should().Be("TIPOS_PROYECTOS_ALREADY_EXISTS");
    }
}
