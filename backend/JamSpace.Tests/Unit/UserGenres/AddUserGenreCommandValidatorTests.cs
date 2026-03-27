using FluentAssertions;
using JamSpace.Application.Features.UserGenres.Commands.AddUserGenre;

namespace JamSpace.Tests.Unit.UserGenres;

public class AddUserGenreCommandValidatorTests
{
    private readonly AddUserGenreCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_GenreName_Is_Empty()
    {
        var result = _validator.Validate(new AddUserGenreCommand(Guid.NewGuid(), ""));
        result.Errors.Should().Contain(e => e.PropertyName == "GenreName");
    }
    
    [Fact]
    public void Should_Have_Error_When_GenreName_Too_Short()
    {
        var result = _validator.Validate(new AddUserGenreCommand(Guid.NewGuid(), "ab"));
        result.Errors.Should().Contain(e => e.PropertyName == "GenreName");
    }

    [Fact]
    public void Should_Have_Error_When_GenreName_Too_Long()
    {
        var longName = new string('a', 51);
        var result = _validator.Validate(new AddUserGenreCommand(Guid.NewGuid(), longName));
        result.Errors.Should().Contain(e => e.PropertyName == "GenreName");
    }
}