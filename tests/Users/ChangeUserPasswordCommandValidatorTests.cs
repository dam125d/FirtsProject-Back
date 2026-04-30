    using FluentAssertions;
using Intap.FirstProject.Application.UseCases.Users.Commands.ChangeUserPassword;

namespace Intap.FirstProject.Tests.Users;

public sealed class ChangeUserPasswordCommandValidatorTests
{
    private readonly ChangeUserPasswordCommandValidator _validator = new();

    [Theory]
    [InlineData("", false)]                  // empty
    [InlineData("short1@", false)]           // too short (7 chars)
    [InlineData("alllowercase1@", false)]    // no uppercase
    [InlineData("NoNumbers@@@", false)]      // no number
    [InlineData("NoSpecial123", false)]      // no special char
    [InlineData("Valid@Pass1", true)]        // valid
    [InlineData("Another#Valid2", true)]     // valid
    public void Validate_NewPassword_Rules(string password, bool shouldBeValid)
    {
        var command = new ChangeUserPasswordCommand(Guid.NewGuid(), password, password);
        var result  = _validator.Validate(command);

        result.IsValid.Should().Be(shouldBeValid);
    }

    [Fact]
    public void Validate_PasswordsMismatch_ReturnsError()
    {
        var command = new ChangeUserPasswordCommand(Guid.NewGuid(), "Valid@Pass1", "Different@Pass1");
        var result  = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Passwords do not match.");
    }

    [Fact]
    public void Validate_EmptyUserId_ReturnsError()
    {
        var command = new ChangeUserPasswordCommand(Guid.Empty, "Valid@Pass1", "Valid@Pass1");
        var result  = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "UserId is required.");
    }

    [Fact]
    public void Validate_ValidCommand_NoErrors()
    {
        var command = new ChangeUserPasswordCommand(Guid.NewGuid(), "Valid@Pass1", "Valid@Pass1");
        var result  = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
}
