using FluentAssertions;
using JamSpace.Application.Features.Teams.Commands.CreateTeam;

namespace JamSpace.Tests.Unit.Teams;

public class CreateTeamCommandValidatorTests
{
    private readonly CreateTeamCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        var result = _validator.Validate(new CreateTeamCommand("", null));
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Should_Have_Error_When_Name_Too_Short()
    {
        var result = _validator.Validate(new CreateTeamCommand("abc", null));
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Should_Have_Error_When_Name_Too_Long()
    {
        var longName = new string('a', 26);
        var result = _validator.Validate(new CreateTeamCommand(longName, null));
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Should_Pass_When_Name_Is_Valid()
    {
        var result = _validator.Validate(new CreateTeamCommand("ValidName", null));
        result.IsValid.Should().BeTrue();
    }
}