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

/// <summary>
/// Integration-level tests for Login that verify permission propagation from the
/// Role navigation property through to the LoginResponse.
/// </summary>
public sealed class LoginWithRolePermissionsTests
{
    private readonly Mock<IUserReadRepository>  _userRepo            = new();
    private readonly Mock<ITokenService>        _tokenService        = new();
    private readonly Mock<IUnitOfWork>          _uow                 = new();
    private readonly Mock<IHttpContextAccessor> _httpContextAccessor = new();

    private LoginCommandHandler CreateHandler() =>
        new(_userRepo.Object, _tokenService.Object, _uow.Object, _httpContextAccessor.Object);

    private static void SetAssignedRole(User user, Role role)
    {
        PropertyInfo? property = typeof(User).GetProperty(
            nameof(User.AssignedRole),
            BindingFlags.Public | BindingFlags.Instance);

        property!.SetValue(user, role);
    }

    [Fact]
    public async Task Handle_WhenUserHasRoleWithPermissions_ReturnsCorrectPermissions()
    {
        // Arrange
        Role role = Role.Create("ProjectManager", "Manages projects");
        role.AssignPermissions(new[]
        {
            "view_projects",
            "create_projects",
            "edit_projects",
            "delete_projects",
        });

        string passwordHash = BCrypt.Net.BCrypt.HashPassword("Secure@Pass1");
        User user = User.Create("Alice Smith", "alice@example.com", passwordHash, role.Id);
        SetAssignedRole(user, role);

        _userRepo.Setup(r => r.GetByEmailWithRoleAsync("alice@example.com", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(user);

        _tokenService.Setup(t => t.GenerateToken(user)).Returns("jwt-token");
        _httpContextAccessor.Setup(h => h.HttpContext).Returns((HttpContext?)null);

        LoginCommand command = new("alice@example.com", "Secure@Pass1");

        // Act
        Result<LoginResponse> result = await CreateHandler().Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        result.Value!.Permissions.Should().BeEquivalentTo(new[]
        {
            "view_projects",
            "create_projects",
            "edit_projects",
            "delete_projects",
        });

        result.Value.Role.Should().Be("ProjectManager");
        result.Value.Email.Should().Be("alice@example.com");
    }
}
