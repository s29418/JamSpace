using FluentAssertions;
using Moq;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.UserGenres.DTOs;
using JamSpace.Application.Features.UserGenres.Queries.GetUsersGenres;
using JamSpace.Domain.Entities;

namespace JamSpace.Tests.Unit.UserGenres;

public class GetUserGenresHandlerTests
{
    private readonly Mock<IUserGenreRepository> _userGenreRepo = new();

    [Fact]
    public async Task Handle_Should_Return_Mapped_Dtos()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var list = new List<UserGenre>
        {
            new UserGenre
            {
                UserId = userId,
                GenreId = Guid.NewGuid(),
                Genre = new Genre { Id = Guid.NewGuid(), Name = "Rap" }
            },
            new UserGenre
            {
                UserId = userId,
                GenreId = Guid.NewGuid(),
                Genre = new Genre { Id = Guid.NewGuid(), Name = "Pop" }
            }
        };

        _userGenreRepo
            .Setup(r => r.GetAllUserGenresAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);

        var sut = new GetUserGenresHandler(_userGenreRepo.Object);

        // Act
        var result = await sut.Handle(new GetUserGenresQuery(userId), CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result[0].GenreName.Should().Be("Rap");
        result[1].GenreName.Should().Be("Pop");
        result.Should().AllBeOfType<UserGenreDto>();
    }

    [Fact]
    public async Task Handle_Should_Return_Empty_List_When_No_Genres()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _userGenreRepo
            .Setup(r => r.GetAllUserGenresAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserGenre>()); 

        var sut = new GetUserGenresHandler(_userGenreRepo.Object);

        // Act
        var result = await sut.Handle(new GetUserGenresQuery(userId), CancellationToken.None);

        // Assert
        result.Should().NotBeNull().And.BeEmpty();
    }
}