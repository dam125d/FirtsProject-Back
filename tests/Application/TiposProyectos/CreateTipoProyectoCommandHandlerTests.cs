using FluentAssertions;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.UseCases.TiposProyectos.Commands.CreateTipoProyecto;
using Intap.FirstProject.Domain.TiposProyectos;
using Intap.FirstProject.Infrastructure.Persistence;
using Intap.FirstProject.Infrastructure.Persistence.Repositories;
using Intap.FirstProject.Tests.Helpers;

namespace Intap.FirstProject.Tests.Application.TiposProyectos;

public sealed class CreateTipoProyectoCommandHandlerTests
{
    private static CreateTipoProyectoCommandHandler CreateHandler(AppDbContext context) =>
        new(new TipoProyectoReadRepository(context),
            new TipoProyectoWriteRepository(context),
            new UnitOfWork(context));

    // Happy path: create with valid data
    [Fact]
    public async Task Handle_WithValidData_CreatesTipoProyecto()
    {
        AppDbContext context = TestDbContextFactory.Create();
        CreateTipoProyectoCommandHandler handler = CreateHandler(context);
        CreateTipoProyectoCommand command = new("Desarrollo", "Proyectos de desarrollo");

        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Guid.Empty);

        TipoProyecto? created = context.TiposProyecto.FirstOrDefault(t => t.Id == result.Value);
        created.Should().NotBeNull();
        created!.Nombre.Should().Be("Desarrollo");
        created.Descripcion.Should().Be("Proyectos de desarrollo");
        created.Estado.Should().BeTrue();
    }

    // Edge case: duplicate name among actives (case-insensitive)
    [Fact]
    public async Task Handle_WithDuplicateActiveName_ReturnsConflict()
    {
        AppDbContext context = TestDbContextFactory.Create();
        CreateTipoProyectoCommandHandler handler = CreateHandler(context);

        await handler.Handle(new CreateTipoProyectoCommand("Infraestructura", null), CancellationToken.None);

        Result<Guid> result = await handler.Handle(
            new CreateTipoProyectoCommand("infraestructura", null),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorTypeResult.Conflict);
        result.Error.Code.Should().Be("TIPOS_PROYECTOS_ALREADY_EXISTS");
    }

    // Name with whitespace is trimmed
    [Fact]
    public async Task Handle_WithLeadingAndTrailingSpaces_TrimsNombre()
    {
        AppDbContext context = TestDbContextFactory.Create();
        CreateTipoProyectoCommandHandler handler = CreateHandler(context);

        Result<Guid> result = await handler.Handle(
            new CreateTipoProyectoCommand("  Marketing  ", null),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        TipoProyecto? created = context.TiposProyecto.FirstOrDefault(t => t.Id == result.Value);
        created!.Nombre.Should().Be("Marketing");
    }

    // A deactivated record with the same name does NOT block creation
    [Fact]
    public async Task Handle_WithInactiveRecordSameName_CreatesSuccessfully()
    {
        AppDbContext context = TestDbContextFactory.Create();

        TipoProyecto inactive = TipoProyecto.Create("Consultoría", null);
        inactive.Deactivate();
        context.TiposProyecto.Add(inactive);
        await context.SaveChangesAsync();

        CreateTipoProyectoCommandHandler handler = CreateHandler(context);
        Result<Guid> result = await handler.Handle(
            new CreateTipoProyectoCommand("Consultoría", null),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }
}
