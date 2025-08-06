using FluentAssertions;
using JamSpace.Application.Features.TeamMembers.Commands.EditTeamMemberMusicalRole;

namespace JamSpace.Tests.Unit.TeamMembers;

public class EditTeamMemberMusicalRoleCommandValidatorTests
{
    private readonly EditTeamMemberMusicalRoleCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_MusicalRole_Is_Empty()
    {
        var result = _validator.Validate(
            new EditTeamMemberMusicalRoleCommand(
                Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "")
        );
        result.Errors.Should().Contain(e => e.PropertyName == "MusicalRole");
    }

    [Fact]
    public void Should_Pass_When_MusicalRole_Is_Valid()
    {
        var result = _validator.Validate(
            new EditTeamMemberMusicalRoleCommand(
                Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Guitarist")
        );
        result.IsValid.Should().BeTrue();
    }
}