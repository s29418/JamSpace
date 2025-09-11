using FluentAssertions;
using Moq;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.UserGenres.Commands.DeleteUserGenre;

namespace JamSpace.Tests.Unit.UserGenres;

public class DeleteUserGenreHandlerTests
{
    private readonly Mock<IUserGenreRepository> _userGenreRepo = new();

    [Fact]
    public async Task Handle_Should_Remove_Link_When_User_Has_Genre()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var genreId = Guid.NewGuid();
        var cmd = new DeleteUserGenreCommand(userId, genreId);

        _userGenreRepo.Setup(r => r.UserHasGenreAsync(
                userId, genreId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(true);

        _userGenreRepo.Setup(r => r.RemoveUserGenreAsync(
                userId, genreId, It.IsAny<CancellationToken>()))
                      .Returns(Task.CompletedTask);

        var sut = new DeleteUserGenreHandler(_userGenreRepo.Object);

        // Act
        MediatR.Unit result = await sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.Should().Be(MediatR.Unit.Value);
        _userGenreRepo.Verify(r => r.RemoveUserGenreAsync(
            userId, genreId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_Conflict_When_User_Does_Not_Have_Genre()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var genreId = Guid.NewGuid();
        var cmd = new DeleteUserGenreCommand(userId, genreId);

        _userGenreRepo.Setup(r => r.UserHasGenreAsync(
                userId, genreId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(false);

        var sut = new DeleteUserGenreHandler(_userGenreRepo.Object);

        // Act
        Func<Task> act = () => sut.Handle(cmd, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
                 .WithMessage("User does not have this genre.");
        _userGenreRepo.Verify(r => r.RemoveUserGenreAsync(
            It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
