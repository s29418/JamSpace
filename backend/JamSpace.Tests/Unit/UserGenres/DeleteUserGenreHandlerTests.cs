using FluentAssertions;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.UserGenres.Commands.DeleteUserGenre;
using JamSpace.Domain.Entities;
using Moq;

namespace JamSpace.Tests.Unit.UserGenres;

public class DeleteUserGenreHandlerTests
{
    private readonly Mock<IUserGenreRepository> _userGenreRepo = new();
    private readonly Mock<IUnitOfWork> _uow = new();

    [Fact]
    public async Task Handle_Should_Remove_Link_When_User_Has_Genre()
    {
        var userId = Guid.NewGuid();
        var genreId = Guid.NewGuid();
        var cmd = new DeleteUserGenreCommand(userId, genreId);

        var link = new UserGenre { UserId = userId, GenreId = genreId };

        _userGenreRepo.Setup(r => r.GetUserGenreAsync(userId, genreId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(link);

        _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var sut = new DeleteUserGenreHandler(_userGenreRepo.Object, _uow.Object);

        var result = await sut.Handle(cmd, CancellationToken.None);

        result.Should().Be(MediatR.Unit.Value);
        _userGenreRepo.Verify(r => r.Remove(link), Times.Once);
        _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_Conflict_When_User_Does_Not_Have_Genre()
    {
        var userId = Guid.NewGuid();
        var genreId = Guid.NewGuid();
        var cmd = new DeleteUserGenreCommand(userId, genreId);

        _userGenreRepo.Setup(r => r.GetUserGenreAsync(userId, genreId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserGenre?)null);

        var sut = new DeleteUserGenreHandler(_userGenreRepo.Object, _uow.Object);

        Func<Task> act = () => sut.Handle(cmd, CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("User does not have this genre.");

        _userGenreRepo.Verify(r => r.Remove(It.IsAny<UserGenre>()), Times.Never);
        _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}