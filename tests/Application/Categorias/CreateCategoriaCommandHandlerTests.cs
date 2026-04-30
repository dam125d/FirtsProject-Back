using FluentAssertions;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.UseCases.Categorias.Commands.CreateCategoria;
using Intap.FirstProject.Domain.Categorias;
using Intap.FirstProject.Infrastructure.Persistence;
using Intap.FirstProject.Infrastructure.Persistence.Repositories;
using Intap.FirstProject.Tests.Helpers;

namespace Intap.FirstProject.Tests.Application.Categorias;

public sealed class CreateCategoriaCommandHandlerTests
{
    private static CreateCategoriaCommandHandler CreateHandler(AppDbContext context) =>
        new(new CategoriaReadRepository(context),
            new CategoriaWriteRepository(context),
            new UnitOfWork(context));

    // IT-05: Crear categoría válida → 201 + verificar en BD
    [Fact]
    public async Task Handle_WithValidData_CreatesCategoria()
    {
        // Arrange
        AppDbContext context = TestDbContextFactory.Create();
        CreateCategoriaCommandHandler handler = CreateHandler(context);
        CreateCategoriaCommand command = new("Tecnología", "Proyectos de TI");

        // Act
        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        // Assert — IT-05: 201 + categoría en BD con Estado=true
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Guid.Empty);

        Categoria? created = context.Categorias.FirstOrDefault(c => c.Id == result.Value);
        created.Should().NotBeNull();
        created!.Nombre.Should().Be("Tecnología");
        created.Descripcion.Should().Be("Proyectos de TI");
        created.Estado.Should().BeTrue();
    }

    // IT-06: Crear con nombre duplicado case-insensitive → 409
    [Fact]
    public async Task Handle_WithDuplicateNombreCaseInsensitive_ReturnsConflict()
    {
        // Arrange
        AppDbContext context = TestDbContextFactory.Create();
        CreateCategoriaCommandHandler handler = CreateHandler(context);

        await handler.Handle(new CreateCategoriaCommand("Tecnología", null), CancellationToken.None);

        // Act — intento con case diferente (IT-06)
        Result<Guid> result = await handler.Handle(new CreateCategoriaCommand("tecnología", null), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorTypeResult.Conflict);
        result.Error.Code.Should().Be("Categoria.NombreDuplicado");
    }

    [Fact]
    public async Task Handle_WithLeadingAndTrailingSpaces_TrimsNombreBeforePersisting()
    {
        // Arrange
        AppDbContext context = TestDbContextFactory.Create();
        CreateCategoriaCommandHandler handler = CreateHandler(context);
        CreateCategoriaCommand command = new("  Marketing  ", null);

        // Act
        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        Categoria? created = context.Categorias.FirstOrDefault(c => c.Id == result.Value);
        created!.Nombre.Should().Be("Marketing");
    }

    [Fact]
    public async Task Handle_WithTrimmedDuplicate_ReturnsConflict()
    {
        // Arrange — "  Tecnología  " should conflict with "Tecnología"
        AppDbContext context = TestDbContextFactory.Create();
        CreateCategoriaCommandHandler handler = CreateHandler(context);

        await handler.Handle(new CreateCategoriaCommand("Tecnología", null), CancellationToken.None);

        // Act
        Result<Guid> result = await handler.Handle(new CreateCategoriaCommand("  Tecnología  ", null), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorTypeResult.Conflict);
    }
}
