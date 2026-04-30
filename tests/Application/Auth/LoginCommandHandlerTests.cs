using FluentAssertions;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts;
using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Application.UseCases.Auth.Commands.Login;
using Intap.FirstProject.Application.UseCases.Auth.DTOs;
using Intap.FirstProject.Domain.Roles;
using Intap.FirstProject.Domain.Users;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Reflection;

namespace Intap.FirstProject.Tests.Application.Auth;

public sealed class LoginCommandHandlerTests
{
    private readonly Mock<IUserReadRepository>  _userRepo            = new();
    private readonly Mock<ITokenService>        _tokenService        = new();
    private readonly Mock<IUnitOfWork>          _uow                 = new();
    private readonly Mock<IHttpContextAccessor> _httpContextAccessor = new();

    private LoginCommandHandler CreateHandler() =>
        new(_userRepo.Object, _tokenService.Object, _uow.Object, _httpContextAccessor.Object);

    /// <summary>
    /// Sets the AssignedRole navigation property on a User via reflection,
    /// since the property has a private setter and no public assignment method exists.
    /// </summary>
    private static void SetAssignedRole(User user, Role? role)
    {
        PropertyInfo? property = typeof(User).GetProperty(
            nameof(User.AssignedRole),
            BindingFlags.Public | BindingFlags.Instance);

        property!.SetValue(user, role);
    }

    [Fact]
    public async Task Handle_WhenUserHasNullRole_ReturnsUnauthorized()
    {
        // Arrange
        User user = User.Create("Test User", "test@example.com", BCrypt.Net.BCrypt.HashPassword("Pass@1"), Guid.NewGuid());
        SetAssignedRole(user, null);

        _userRepo.Setup(r => r.GetByEmailWithRoleAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(user);

        LoginCommand command = new("test@example.com", "Pass@1");

        // Act
        Result<LoginResponse> result = await CreateHandler().Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorTypeResult.Unauthorized);
    }

    [Fact]
    public async Task Handle_WhenRoleIsInactive_ReturnsFailure()
    {
        // Arrange
        Role role = Role.Create("Inactive Role", "A deactivated role");
        role.Deactivate();

        User user = User.Create("Test User", "test@example.com", BCrypt.Net.BCrypt.HashPassword("Pass@1"), role.Id);
        SetAssignedRole(user, role);

        _userRepo.Setup(r => r.GetByEmailWithRoleAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(user);

        LoginCommand command = new("test@example.com", "Pass@1");

        // Act
        Result<LoginResponse> result = await CreateHandler().Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorTypeResult.Unauthorized);
    }

    [Fact]
    public async Task Handle_WhenLoginSucceeds_ReturnsPermissionsFromRole()
    {
        // Arrange
        Role role = Role.Create("Manager", "Project manager");
        role.AssignPermissions(new[] { "view_projects", "create_projects", "edit_projects" });

        string passwordHash = BCrypt.Net.BCrypt.HashPassword("Pass@1");
        User user = User.Create("Test User", "test@example.com", passwordHash, role.Id);
        SetAssignedRole(user, role);

        _userRepo.Setup(r => r.GetByEmailWithRoleAsync("test@example.com", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(user);

        _tokenService.Setup(t => t.GenerateToken(user)).Returns("fake-jwt-token");
        _httpContextAccessor.Setup(h => h.HttpContext).Returns((HttpContext?)null);

        LoginCommand command = new("test@example.com", "Pass@1");

        // Act
        Result<LoginResponse> result = await CreateHandler().Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Permissions.Should().BeEquivalentTo(new[] { "view_projects", "create_projects", "edit_projects" });
    }

    [Fact]
    public async Task Handle_WhenLoginSucceeds_ReturnsRoleName()
    {
        // Arrange
        Role role = Role.Create("Manager", "Project manager");
        role.AssignPermissions(new[] { "view_projects" });

        string passwordHash = BCrypt.Net.BCrypt.HashPassword("Pass@1");
        User user = User.Create("Test User", "test@example.com", passwordHash, role.Id);
        SetAssignedRole(user, role);

        _userRepo.Setup(r => r.GetByEmailWithRoleAsync("test@example.com", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(user);

        _tokenService.Setup(t => t.GenerateToken(user)).Returns("fake-jwt-token");
        _httpContextAccessor.Setup(h => h.HttpContext).Returns((HttpContext?)null);

        LoginCommand command = new("test@example.com", "Pass@1");

        // Act
        Result<LoginResponse> result = await CreateHandler().Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Role.Should().Be("Manager");
    }
}
