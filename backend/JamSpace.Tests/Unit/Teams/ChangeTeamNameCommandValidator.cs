using FluentAssertions;
using JamSpace.Application.Features.Teams.Commands.ChangeTeamName;

namespace JamSpace.Tests.Unit.Teams;

public class ChangeTeamNameCommandValidatorTests
{
    private readonly ChangeTeamNameCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        var result = _validator.Validate(new ChangeTeamNameCommand(
            Guid.NewGuid(), Guid.NewGuid(), ""));
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Should_Have_Error_When_Name_Too_Short()
    {
        var result = _validator.Validate(new ChangeTeamNameCommand(
            Guid.NewGuid(), Guid.NewGuid(), "abc"));
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Should_Have_Error_When_Name_Too_Long()
    {
        var longName = new string('a', 26);
        var result = _validator.Validate(new ChangeTeamNameCommand(
            Guid.NewGuid(), Guid.NewGuid(), longName));
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Should_Pass_When_Name_Is_Valid()
    {
        var result = _validator.Validate(new ChangeTeamNameCommand(
            Guid.NewGuid(), Guid.NewGuid(), "ValidName"));
        result.IsValid.Should().BeTrue();
    }
}