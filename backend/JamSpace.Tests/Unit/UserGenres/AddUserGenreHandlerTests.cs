using FluentAssertions;
using Moq;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.UserGenres.Commands.AddUserGenre;
using JamSpace.Domain.Entities;

namespace JamSpace.Tests.Unit.UserGenres;

public class AddUserGenreHandlerTests
{
    private readonly Mock<IUserGenreRepository> _userGenreRepo = new();
    private readonly Mock<IGenreRepository> _genreRepo = new();

    [Fact]
    public async Task Handle_Should_Create_Link_When_User_Does_Not_Have_Genre_And_Genre_Exists()
    {
        var userId = Guid.NewGuid();
        var genreId = Guid.NewGuid();
        var cmd = new AddUserGenreCommand(userId, "Rap");

        _genreRepo.Setup(r => r.GetGenreByNameAsync("Rap", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Genre { Id = genreId, Name = "Rap" });

        _userGenreRepo.Setup(r => r.UserHasGenreAsync(
                userId, genreId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _userGenreRepo.Setup(r => r.AddUserGenreAsync(
                userId, genreId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserGenre { UserId = userId, GenreId = genreId });

        var sut = new AddUserGenreHandler(_userGenreRepo.Object, _genreRepo.Object);

        var result = await sut.Handle(cmd, CancellationToken.None);

        result.GenreId.Should().Be(genreId);
        result.GenreName.Should().Be("Rap");
    }


    [Fact]
    public async Task Handle_Should_Create_Genre_If_Not_Exists_Then_Link()
    {
        var userId = Guid.NewGuid();
        var newGenreId = Guid.NewGuid();
        var cmd = new AddUserGenreCommand(userId, "Synthwave");

        _genreRepo.Setup(r => r.GetGenreByNameAsync("Synthwave", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Genre?)null);

        _genreRepo.Setup(r => r.CreateGenreAsync("Synthwave", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Genre { Id = newGenreId, Name = "Synthwave" });

        _userGenreRepo.Setup(r => r.UserHasGenreAsync(
                userId, newGenreId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var sut = new AddUserGenreHandler(_userGenreRepo.Object, _genreRepo.Object);

        var dto = await sut.Handle(cmd, CancellationToken.None);

        dto.GenreId.Should().Be(newGenreId);
        dto.GenreName.Should().Be("Synthwave");
        _userGenreRepo.Verify(r => r.AddUserGenreAsync(
            userId, newGenreId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_When_User_Already_Has_Genre()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var genreId = Guid.NewGuid();
        var cmd = new AddUserGenreCommand(userId, "Pop");

        _genreRepo.Setup(r => r.GetGenreByNameAsync("Pop", It.IsAny<CancellationToken>()))
                  .ReturnsAsync(new Genre { Id = genreId, Name = "Pop" });

        _userGenreRepo.Setup(r => r.UserHasGenreAsync(
                userId, genreId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(true);

        var sut = new AddUserGenreHandler(_userGenreRepo.Object, _genreRepo.Object);

        // Act
        Func<Task> act = () => sut.Handle(cmd, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("User already has this genre.");
        _userGenreRepo.Verify(r => r.AddUserGenreAsync(
            It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
