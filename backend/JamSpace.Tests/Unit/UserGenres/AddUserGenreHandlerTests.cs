using FluentAssertions;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.UserGenres.Commands.AddUserGenre;
using JamSpace.Domain.Entities;
using Moq;

namespace JamSpace.Tests.Unit.UserGenres;

public class AddUserGenreHandlerTests
{
    private readonly Mock<IUserGenreRepository> _userGenreRepo = new();
    private readonly Mock<IGenreRepository> _genreRepo = new();
    private readonly Mock<IUnitOfWork> _uow = new();

    [Fact]
    public async Task Handle_Should_Create_Link_When_User_Does_Not_Have_Genre_And_Genre_Exists()
    {
        var userId = Guid.NewGuid();
        var genreId = Guid.NewGuid();
        var cmd = new AddUserGenreCommand(userId, "Rap");

        _genreRepo.Setup(r => r.GetGenreByNameAsync("Rap", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Genre { Id = genreId, Name = "Rap" });

        _userGenreRepo.Setup(r => r.UserHasGenreAsync(userId, genreId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var sut = new AddUserGenreHandler(_userGenreRepo.Object, _genreRepo.Object, _uow.Object);

        var result = await sut.Handle(cmd, CancellationToken.None);

        result.GenreId.Should().Be(genreId);
        result.GenreName.Should().Be("Rap");

        _genreRepo.Verify(r => r.AddAsync(It.IsAny<Genre>(), It.IsAny<CancellationToken>()), Times.Never);
        _userGenreRepo.Verify(r => r.AddAsync(It.IsAny<UserGenre>(), It.IsAny<CancellationToken>()), Times.Once);
        _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Create_Genre_If_Not_Exists_Then_Link()
    {
        var userId = Guid.NewGuid();
        var cmd = new AddUserGenreCommand(userId, "Synthwave");

        _genreRepo.Setup(r => r.GetGenreByNameAsync("Synthwave", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Genre?)null);

        _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        Genre? createdGenre = null;
        _genreRepo.Setup(r => r.AddAsync(It.IsAny<Genre>(), It.IsAny<CancellationToken>()))
            .Callback<Genre, CancellationToken>((g, _) => createdGenre = g)
            .Returns(Task.CompletedTask);

        _userGenreRepo.Setup(r => r.UserHasGenreAsync(userId, It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var sut = new AddUserGenreHandler(_userGenreRepo.Object, _genreRepo.Object, _uow.Object);

        var dto = await sut.Handle(cmd, CancellationToken.None);

        createdGenre.Should().NotBeNull();
        dto.GenreId.Should().Be(createdGenre!.Id);
        dto.GenreName.Should().Be(createdGenre.Name);

        _genreRepo.Verify(r => r.AddAsync(It.IsAny<Genre>(), It.IsAny<CancellationToken>()), Times.Once);
        _userGenreRepo.Verify(r => r.AddAsync(It.IsAny<UserGenre>(), It.IsAny<CancellationToken>()), Times.Once);
        _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_When_User_Already_Has_Genre()
    {
        var userId = Guid.NewGuid();
        var genreId = Guid.NewGuid();
        var cmd = new AddUserGenreCommand(userId, "Pop");

        _genreRepo.Setup(r => r.GetGenreByNameAsync("Pop", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Genre { Id = genreId, Name = "Pop" });

        _userGenreRepo.Setup(r => r.UserHasGenreAsync(userId, genreId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var sut = new AddUserGenreHandler(_userGenreRepo.Object, _genreRepo.Object, _uow.Object);

        Func<Task> act = () => sut.Handle(cmd, CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("User already has this genre.");

        _userGenreRepo.Verify(r => r.AddAsync(It.IsAny<UserGenre>(), It.IsAny<CancellationToken>()), Times.Never);
        _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}