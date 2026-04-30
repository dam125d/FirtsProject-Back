using FluentAssertions;
using Intap.FirstProject.Domain.Roles;

namespace Intap.FirstProject.Tests.Roles;

public sealed class RoleDomainTests
{
    [Fact]
    public void Create_SetsPropertiesCorrectly()
    {
        var role = Role.Create("  Admin  ", "Administrator role");

        role.Name.Should().Be("Admin");
        role.Description.Should().Be("Administrator role");
        role.IsActive.Should().BeTrue();
        role.IsSystem.Should().BeFalse();
        role.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Create_SystemRole_SetsIsSystem()
    {
        var role = Role.Create("SuperAdmin", "System administrator", isSystem: true);

        role.IsSystem.Should().BeTrue();
    }

    [Fact]
    public void Update_ChangesNameAndDescription()
    {
        var role = Role.Create("OldName", "Old description");

        role.Update("NewName", "New description");

        role.Name.Should().Be("NewName");
        role.Description.Should().Be("New description");
        role.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Deactivate_SetsIsActiveFalse()
    {
        var role = Role.Create("TestRole", "A test role");

        role.Deactivate();

        role.IsActive.Should().BeFalse();
        role.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Activate_SetsIsActiveTrue()
    {
        var role = Role.Create("TestRole", "A test role");
        role.Deactivate();

        role.Activate();

        role.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_TrimsWhitespace_InNameAndDescription()
    {
        var role = Role.Create("  Editor  ", "  Content editor  ");

        role.Name.Should().Be("Editor");
        role.Description.Should().Be("Content editor");
    }
}
