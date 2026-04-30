using FluentAssertions;
using Intap.FirstProject.Application.Contracts;
using Intap.FirstProject.Application.UseCases.Users.Commands.ChangeUserPassword;
using Intap.FirstProject.Domain.Users;
using Moq;

namespace Intap.FirstProject.Tests.Users;

public sealed class ChangeUserPasswordCommandHandlerTests
{
    private readonly Mock<IUserWriteRepository> _writeRepo = new();
    private readonly Mock<IUnitOfWork>          _uow       = new();

    private ChangeUserPasswordCommandHandler CreateHandler() =>
        new(_writeRepo.Object, _uow.Object);

    [Fact]
    public async Task Handle_UserNotFound_ReturnsFailure()
    {
        _writeRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync((User?)null);

        var command = new ChangeUserPasswordCommand(Guid.NewGuid(), "NewPass@1", "NewPass@1");
        var result  = await CreateHandler().Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("User.NotFound");
    }

    [Fact]
    public async Task Handle_ValidUser_ChangesPasswordAndSaves()
    {
        var user = User.Create("Test User", "test@test.com", "oldhash", Guid.NewGuid());

        _writeRepo.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(user);

        var command = new ChangeUserPasswordCommand(user.Id, "NewPass@1", "NewPass@1");
        var result  = await CreateHandler().Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        user.PasswordHash.Should().NotBe("oldhash");
        _uow.Verify(u => u.CompleteAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidUser_HashIsNotPlainTextPassword()
    {
        var user = User.Create("Test User", "test@test.com", "oldhash", Guid.NewGuid());

        _writeRepo.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(user);

        var command = new ChangeUserPasswordCommand(user.Id, "NewPass@1", "NewPass@1");
        await CreateHandler().Handle(command, CancellationToken.None);

        user.PasswordHash.Should().NotBe("NewPass@1");
        user.PasswordHash.Should().StartWith("$2");   // BCrypt prefix
    }
}
