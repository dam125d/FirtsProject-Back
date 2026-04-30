using FluentAssertions;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.UseCases.Roles.Commands.AssignRolePermissions;
using Intap.FirstProject.Domain.Permissions;
using Intap.FirstProject.Domain.Roles;
using Intap.FirstProject.Infrastructure.Persistence;
using Intap.FirstProject.Infrastructure.Persistence.Repositories;
using Intap.FirstProject.Tests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Intap.FirstProject.Tests.Application.Roles;

public sealed class AssignRolePermissionsCommandHandlerTests
{
    private static AssignRolePermissionsCommandHandler CreateHandler(AppDbContext context)
    {
        RoleWriteRepository      roleWriteRepo       = new(context);
        PermissionReadRepository permissionReadRepo  = new(context);
        UnitOfWork               uow                 = new(context);
        return new AssignRolePermissionsCommandHandler(roleWriteRepo, permissionReadRepo, uow);
    }

    /// <summary>Seeds a set of known permissions into the InMemory database.</summary>
    private static async Task SeedPermissionsAsync(AppDbContext context, params string[] codes)
    {
        foreach (string code in codes)
        {
            context.Permissions.Add(Permission.Create(code, code, "Test"));
        }
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task Handle_WithInvalidPermissionCode_ReturnsValidationError()
    {
        // Arrange
        AppDbContext context = TestDbContextFactory.Create();
        await SeedPermissionsAsync(context, "view_users", "edit_users");

        Role role = Role.Create("Tester", "Test role");
        context.Roles.Add(role);
        await context.SaveChangesAsync();

        AssignRolePermissionsCommandHandler handler = CreateHandler(context);
        AssignRolePermissionsCommand command = new(role.Id, new[] { "view_users", "nonexistent_code" });

        // Act
        Result result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorTypeResult.Validation);
        result.Error.Code.Should().Be("Permission.NotFound");
    }

    [Fact]
    public async Task Handle_WithValidCodes_ReplacesPermissions()
    {
        // Arrange
        AppDbContext context = TestDbContextFactory.Create();
        await SeedPermissionsAsync(context, "view_users", "edit_users", "view_projects");

        Role role = Role.Create("Reviewer", "Reviewer role");
        context.Roles.Add(role);
        await context.SaveChangesAsync();

        // Assign initial permissions
        AssignRolePermissionsCommandHandler handler = CreateHandler(context);
        AssignRolePermissionsCommand firstCommand = new(role.Id, new[] { "view_users" });
        await handler.Handle(firstCommand, CancellationToken.None);

        // Replace with new set
        AssignRolePermissionsCommand command = new(role.Id, new[] { "edit_users", "view_projects" });

        // Act
        Result result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        // Re-fetch to verify DB state
        Role? updated = context.Roles.Include(r => r.Permissions).FirstOrDefault(r => r.Id == role.Id);
        updated.Should().NotBeNull();
        updated!.GetPermissionCodes().Should().BeEquivalentTo(new[] { "edit_users", "view_projects" });
        updated.GetPermissionCodes().Should().NotContain("view_users");
    }

    [Fact]
    public async Task Handle_WithEmptyCodes_RemovesAllPermissions()
    {
        // Arrange
        AppDbContext context = TestDbContextFactory.Create();
        await SeedPermissionsAsync(context, "view_users", "edit_users");

        Role role = Role.Create("Cleaner", "Role to be cleared");
        context.Roles.Add(role);
        await context.SaveChangesAsync();

        // Assign initial permissions
        AssignRolePermissionsCommandHandler handler = CreateHandler(context);
        AssignRolePermissionsCommand firstCommand = new(role.Id, new[] { "view_users", "edit_users" });
        await handler.Handle(firstCommand, CancellationToken.None);

        AssignRolePermissionsCommand emptyCommand = new(role.Id, Array.Empty<string>());

        // Act
        Result result = await handler.Handle(emptyCommand, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        Role? updated = context.Roles.Include(r => r.Permissions).FirstOrDefault(r => r.Id == role.Id);
        updated.Should().NotBeNull();
        updated!.GetPermissionCodes().Should().BeEmpty();
    }
}
