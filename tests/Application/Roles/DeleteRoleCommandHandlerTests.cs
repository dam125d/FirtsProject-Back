using FluentAssertions;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.UseCases.Roles.Commands.DeleteRole;
using Intap.FirstProject.Domain.Roles;
using Intap.FirstProject.Domain.Users;
using Intap.FirstProject.Infrastructure.Persistence;
using Intap.FirstProject.Infrastructure.Persistence.Repositories;
using Intap.FirstProject.Tests.Helpers;

namespace Intap.FirstProject.Tests.Application.Roles;

public sealed class DeleteRoleCommandHandlerTests
{
    private static DeleteRoleCommandHandler CreateHandler(AppDbContext context)
    {
        RoleWriteRepository writeRepo = new(context);
        RoleReadRepository  readRepo  = new(context);
        UnitOfWork          uow       = new(context);
        return new DeleteRoleCommandHandler(writeRepo, readRepo, uow);
    }

    [Fact]
    public async Task Handle_WithSystemRole_ReturnsConflict()
    {
        // Arrange
        AppDbContext context = TestDbContextFactory.Create();

        Role systemRole = Role.Create("SuperAdmin", "System administrator", isSystem: true);
        context.Roles.Add(systemRole);
        await context.SaveChangesAsync();

        DeleteRoleCommandHandler handler = CreateHandler(context);
        DeleteRoleCommand command = new(systemRole.Id);

        // Act
        Result result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorTypeResult.Conflict);
        result.Error.Code.Should().Be("Role.SystemRoleCannotBeModified");
    }

    [Fact]
    public async Task Handle_WithUsersAssigned_ReturnsConflict()
    {
        // Arrange
        AppDbContext context = TestDbContextFactory.Create();

        Role role = Role.Create("Operator", "Operator role");
        context.Roles.Add(role);

        User user = User.Create("John Doe", "john@example.com", "hash", role.Id);
        context.Users.Add(user);

        await context.SaveChangesAsync();

        DeleteRoleCommandHandler handler = CreateHandler(context);
        DeleteRoleCommand command = new(role.Id);

        // Act
        Result result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorTypeResult.Conflict);
        result.Error.Code.Should().Be("Role.RoleInUse");
    }

    [Fact]
    public async Task Handle_WithValidRole_DeletesRole()
    {
        // Arrange
        AppDbContext context = TestDbContextFactory.Create();

        Role role = Role.Create("Temporary", "Temporary role");
        context.Roles.Add(role);
        await context.SaveChangesAsync();

        Guid roleId = role.Id;
        DeleteRoleCommandHandler handler = CreateHandler(context);
        DeleteRoleCommand command = new(roleId);

        // Act
        Result result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        bool roleStillExists = context.Roles.Any(r => r.Id == roleId);
        roleStillExists.Should().BeFalse();
    }
}
