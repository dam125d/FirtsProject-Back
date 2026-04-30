using FluentAssertions;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.UseCases.TiposTareas.Commands.CreateTipoTarea;
using Intap.FirstProject.Domain.TiposTareas;
using Intap.FirstProject.Infrastructure.Persistence;
using Intap.FirstProject.Infrastructure.Persistence.Repositories;
using Intap.FirstProject.Tests.Helpers;

namespace Intap.FirstProject.Tests.Application.TiposTareas;

public sealed class CreateTipoTareaCommandHandlerTests
{
    private static CreateTipoTareaCommandHandler CreateHandler(AppDbContext context) =>
        new(new TipoTareaReadRepository(context),
            new TipoTareaWriteRepository(context),
            new UnitOfWork(context));

    // Happy path: create with valid data
    [Fact]
    public async Task Handle_WithValidData_CreatesTipoTarea()
    {
        AppDbContext context = TestDbContextFactory.Create();
        CreateTipoTareaCommandHandler handler = CreateHandler(context);
        CreateTipoTareaCommand command = new("Desarrollo", "Tareas de desarrollo");

        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Guid.Empty);

        TipoTarea? created = context.TiposTarea.FirstOrDefault(t => t.Id == result.Value);
        created.Should().NotBeNull();
        created!.Nombre.Should().Be("Desarrollo");
        created.Descripcion.Should().Be("Tareas de desarrollo");
        created.Estado.Should().BeTrue();
    }

    // Edge case: duplicate active name (case-insensitive)
    [Fact]
    public async Task Handle_WithDuplicateActiveName_ReturnsConflict()
    {
        AppDbContext context = TestDbContextFactory.Create();
        CreateTipoTareaCommandHandler handler = CreateHandler(context);

        await handler.Handle(new CreateTipoTareaCommand("Análisis", null), CancellationToken.None);

        Result<Guid> result = await handler.Handle(
            new CreateTipoTareaCommand("análisis", null),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorTypeResult.Conflict);
        result.Error.Code.Should().Be("TIPOS_TAREAS_ALREADY_EXISTS");
    }

    // Name with leading/trailing whitespace is trimmed
    [Fact]
    public async Task Handle_WithLeadingAndTrailingSpaces_TrimsNombre()
    {
        AppDbContext context = TestDbContextFactory.Create();
        CreateTipoTareaCommandHandler handler = CreateHandler(context);

        Result<Guid> result = await handler.Handle(
            new CreateTipoTareaCommand("  Testing  ", null),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        TipoTarea? created = context.TiposTarea.FirstOrDefault(t => t.Id == result.Value);
        created!.Nombre.Should().Be("Testing");
    }

    // A deactivated record with the same name does NOT block creation
    [Fact]
    public async Task Handle_WithInactiveRecordSameName_CreatesSuccessfully()
    {
        AppDbContext context = TestDbContextFactory.Create();

        TipoTarea inactive = TipoTarea.Create("Diseño", null);
        inactive.Deactivate();
        context.TiposTarea.Add(inactive);
        await context.SaveChangesAsync();

        CreateTipoTareaCommandHandler handler = CreateHandler(context);
        Result<Guid> result = await handler.Handle(
            new CreateTipoTareaCommand("Diseño", null),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    // Happy path: create without description
    [Fact]
    public async Task Handle_WithNullDescripcion_CreatesSuccessfully()
    {
        AppDbContext context = TestDbContextFactory.Create();
        CreateTipoTareaCommandHandler handler = CreateHandler(context);

        Result<Guid> result = await handler.Handle(
            new CreateTipoTareaCommand("QA", null),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        TipoTarea? created = context.TiposTarea.FirstOrDefault(t => t.Id == result.Value);
        created!.Descripcion.Should().BeNull();
    }
}
