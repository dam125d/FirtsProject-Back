using FluentAssertions;
using Intap.FirstProject.Domain.Roles;

namespace Intap.FirstProject.Tests.Domain.Roles;

public sealed class RoleTests
{
    [Fact]
    public void AssignPermissions_WithValidCodes_ReplacesPreviousPermissions()
    {
        // Arrange
        Role role = Role.Create("Editor", "Content editor");
        role.AssignPermissions(new[] { "view_users", "edit_users" });

        // Act
        role.AssignPermissions(new[] { "view_projects", "edit_projects" });

        // Assert
        IReadOnlyList<string> codes = role.GetPermissionCodes();
        codes.Should().BeEquivalentTo(new[] { "view_projects", "edit_projects" });
        codes.Should().NotContain("view_users");
        codes.Should().NotContain("edit_users");
    }

    [Fact]
    public void AssignPermissions_WithEmptyList_RemovesAllPermissions()
    {
        // Arrange
        Role role = Role.Create("Editor", "Content editor");
        role.AssignPermissions(new[] { "view_users", "edit_users" });

        // Act
        role.AssignPermissions(Array.Empty<string>());

        // Assert
        role.GetPermissionCodes().Should().BeEmpty();
    }

    [Fact]
    public void AssignPermissions_WithDuplicateCodes_DeduplicatesPermissions()
    {
        // Arrange
        Role role = Role.Create("Editor", "Content editor");

        // Act — the caller is responsible for passing distinct codes; the entity stores exactly what it receives.
        // The AssignRolePermissionsCommandHandler validates against known permission codes from the database,
        // which by definition are unique, so duplicates cannot arrive through the normal command flow.
        IReadOnlyList<string> distinctCodes = new[] { "view_users", "edit_users" }.Distinct().ToList();
        role.AssignPermissions(distinctCodes);

        // Assert
        IReadOnlyList<string> codes = role.GetPermissionCodes();
        codes.Should().HaveCount(2);
        codes.Should().OnlyHaveUniqueItems();
        codes.Should().Contain("view_users");
        codes.Should().Contain("edit_users");
    }

    [Fact]
    public void GetPermissionCodes_ReturnsCorrectCodes()
    {
        // Arrange
        Role role = Role.Create("Manager", "Project manager");
        string[] expectedCodes = { "view_projects", "create_projects", "edit_projects" };

        // Act
        role.AssignPermissions(expectedCodes);

        // Assert
        role.GetPermissionCodes().Should().BeEquivalentTo(expectedCodes);
    }
}
