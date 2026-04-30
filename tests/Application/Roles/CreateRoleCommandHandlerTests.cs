using FluentAssertions;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.UseCases.Roles.Commands.CreateRole;
using Intap.FirstProject.Infrastructure.Persistence;
using Intap.FirstProject.Infrastructure.Persistence.Repositories;
using Intap.FirstProject.Tests.Helpers;

namespace Intap.FirstProject.Tests.Application.Roles;

public sealed class CreateRoleCommandHandlerTests
{
    private static CreateRoleCommandHandler CreateHandler(AppDbContext context)
    {
        RoleReadRepository  readRepo  = new(context);
        RoleWriteRepository writeRepo = new(context);
        UnitOfWork          uow       = new(context);
        return new CreateRoleCommandHandler(readRepo, writeRepo, uow);
    }

    [Fact]
    public async Task Handle_WithUniqueName_CreatesRole()
    {
        // Arrange
        AppDbContext context = TestDbContextFactory.Create();
        CreateRoleCommandHandler handler = CreateHandler(context);
        CreateRoleCommand command = new("Viewer", "Read-only access");

        // Act
        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Guid.Empty);

        bool roleExists = context.Roles.Any(r => r.Name == "Viewer");
        roleExists.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithDuplicateName_ReturnsConflict()
    {
        // Arrange
        AppDbContext context = TestDbContextFactory.Create();
        CreateRoleCommandHandler handler = CreateHandler(context);

        CreateRoleCommand first = new("Auditor", "Audit access");
        await handler.Handle(first, CancellationToken.None);

        CreateRoleCommand duplicate = new("Auditor", "Another auditor role");

        // Act
        Result<Guid> result = await handler.Handle(duplicate, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorTypeResult.Conflict);
        result.Error.Code.Should().Be("Role.AlreadyExists");
    }
}
