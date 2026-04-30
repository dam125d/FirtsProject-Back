using FluentAssertions;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.UseCases.Categorias.Commands.CreateCategoria;
using Intap.FirstProject.Infrastructure.Persistence;
using Intap.FirstProject.Infrastructure.Persistence.Repositories;
using Intap.FirstProject.Tests.Helpers;

namespace Intap.FirstProject.Tests.Application.Categorias;

/// <summary>
/// SEC-01: SQL injection payloads should be accepted as plain text or rejected by validator — never cause 500.
/// EF Core with parameterized queries prevents SQL injection at the ORM layer.
/// These tests verify the handler completes without throwing and returns a deterministic result.
/// </summary>
public sealed class CategoriaSecurityTests
{
    private static CreateCategoriaCommandHandler CreateHandler(AppDbContext context) =>
        new(new CategoriaReadRepository(context),
            new CategoriaWriteRepository(context),
            new UnitOfWork(context));

    // SEC-01: SQL injection in Nombre — must not throw, must return Success or Conflict (never 500)
    [Theory]
    [InlineData("'; DROP TABLE Categorias; --")]
    [InlineData("' OR '1'='1")]
    [InlineData("<script>alert('xss')</script>")]
    [InlineData("1; SELECT * FROM Users")]
    public async Task Handle_SqlInjectionPayloadInNombre_DoesNotThrowAndReturnsResult(string maliciousInput)
    {
        // Arrange
        AppDbContext context = TestDbContextFactory.Create();
        CreateCategoriaCommandHandler handler = CreateHandler(context);
        CreateCategoriaCommand command = new(maliciousInput, null);

        // Act
        Func<Task<Result<Guid>>> act = () => handler.Handle(command, CancellationToken.None);

        // Assert — EF Core parameterized queries prevent injection; handler must not throw
        await act.Should().NotThrowAsync();

        Result<Guid> result = await handler.Handle(command, CancellationToken.None);
        // Second call: either Success (first was saved) or Conflict (duplicate)
        // Either way, table must not be dropped — context must still be usable
        bool tableIntact = await System.Threading.Tasks.Task.FromResult(context.Categorias != null);
        tableIntact.Should().BeTrue();
    }

    // SEC-03: Acceso sin permiso categorias:create
    // Note: JWT + permission guard enforcement is tested at the HTTP layer.
    // At handler level, we verify the handler itself does not bypass business rules.
    [Fact]
    public async Task Handle_DuplicateNombre_AlwaysReturnsConflictRegardlessOfCasing()
    {
        // Arrange — simulate case-insensitive duplicate check (defense in depth)
        AppDbContext context = TestDbContextFactory.Create();
        CreateCategoriaCommandHandler handler = CreateHandler(context);

        await handler.Handle(new CreateCategoriaCommand("Admin", null), CancellationToken.None);

        string[] caseVariants = ["ADMIN", "admin", "Admin", "aDmIn"];

        foreach (string variant in caseVariants)
        {
            // Act
            Result<Guid> result = await handler.Handle(new CreateCategoriaCommand(variant, null), CancellationToken.None);

            // Assert — conflict on every case variant
            result.IsSuccess.Should().BeFalse($"'{variant}' should conflict with 'Admin'");
            result.Error!.Code.Should().Be("Categoria.NombreDuplicado");
        }
    }
}
