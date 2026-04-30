using FluentAssertions;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.UseCases.TiposProyectos.Commands.UpdateTipoProyecto;
using Intap.FirstProject.Domain.TiposProyectos;
using Intap.FirstProject.Infrastructure.Persistence;
using Intap.FirstProject.Infrastructure.Persistence.Repositories;
using Intap.FirstProject.Tests.Helpers;

namespace Intap.FirstProject.Tests.Application.TiposProyectos;

public sealed class UpdateTipoProyectoCommandHandlerTests
{
    private static UpdateTipoProyectoCommandHandler CreateHandler(AppDbContext context) =>
        new(new TipoProyectoReadRepository(context),
            new TipoProyectoWriteRepository(context),
            new UnitOfWork(context));

    private static async Task<Guid> SeedTipoProyecto(AppDbContext context, string nombre = "Original")
    {
        TipoProyecto t = TipoProyecto.Create(nombre, null);
        context.TiposProyecto.Add(t);
        await context.SaveChangesAsync();
        return t.Id;
    }

    // Happy path: update with valid data
    [Fact]
    public async Task Handle_WithValidData_UpdatesTipoProyecto()
    {
        AppDbContext context = TestDbContextFactory.Create();
        Guid id = await SeedTipoProyecto(context, "Tecnología");
        UpdateTipoProyectoCommandHandler handler = CreateHandler(context);

        Result<Guid> result = await handler.Handle(
            new UpdateTipoProyectoCommand(id, "Innovación", "Nueva descripción"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        TipoProyecto? updated = context.TiposProyecto.FirstOrDefault(t => t.Id == id);
        updated!.Nombre.Should().Be("Innovación");
        updated.Descripcion.Should().Be("Nueva descripción");
        updated.UpdatedAt.Should().NotBeNull();
    }

    // Edge case: ID not found
    [Fact]
    public async Task Handle_WithNonExistingId_ReturnsNotFound()
    {
        AppDbContext context = TestDbContextFactory.Create();
        UpdateTipoProyectoCommandHandler handler = CreateHandler(context);

        Result<Guid> result = await handler.Handle(
            new UpdateTipoProyectoCommand(Guid.NewGuid(), "Nombre", null),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorTypeResult.NotFound);
        result.Error.Code.Should().Be("TIPOS_PROYECTOS_NOT_FOUND");
    }

    // Edge case: duplicate name from another active record
    [Fact]
    public async Task Handle_WithDuplicateNameFromOtherRecord_ReturnsConflict()
    {
        AppDbContext context = TestDbContextFactory.Create();

        TipoProyecto t1 = TipoProyecto.Create("Tecnología", null);
        TipoProyecto t2 = TipoProyecto.Create("Marketing", null);
        context.TiposProyecto.AddRange(t1, t2);
        await context.SaveChangesAsync();

        UpdateTipoProyectoCommandHandler handler = CreateHandler(context);

        Result<Guid> result = await handler.Handle(
            new UpdateTipoProyectoCommand(t2.Id, "Tecnología", null),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorTypeResult.Conflict);
        result.Error.Code.Should().Be("TIPOS_PROYECTOS_ALREADY_EXISTS");
    }

    // Keeping the same name on own record is allowed
    [Fact]
    public async Task Handle_WithSameNameAsOwnRecord_Succeeds()
    {
        AppDbContext context = TestDbContextFactory.Create();
        Guid id = await SeedTipoProyecto(context, "Tecnología");
        UpdateTipoProyectoCommandHandler handler = CreateHandler(context);

        Result<Guid> result = await handler.Handle(
            new UpdateTipoProyectoCommand(id, "Tecnología", "Actualizada"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    // Descripcion can be set to null (cleared)
    [Fact]
    public async Task Handle_WithNullDescripcion_ClearsDescripcion()
    {
        AppDbContext context = TestDbContextFactory.Create();
        TipoProyecto t = TipoProyecto.Create("Consultoría", "Desc original");
        context.TiposProyecto.Add(t);
        await context.SaveChangesAsync();

        UpdateTipoProyectoCommandHandler handler = CreateHandler(context);

        Result<Guid> result = await handler.Handle(
            new UpdateTipoProyectoCommand(t.Id, "Consultoría", null),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        TipoProyecto? updated = context.TiposProyecto.FirstOrDefault(x => x.Id == t.Id);
        updated!.Descripcion.Should().BeNull();
    }
}
