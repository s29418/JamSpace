using FluentAssertions;
using FluentValidation.TestHelper;
using JamSpace.Application.Features.UserSkills.Commands.AddUserSkill;

namespace JamSpace.Tests.Unit.UserSkills;

public class AddUserSkillCommandValidatorTests
{
    private readonly AddUserSkillCommandValidator _validator = new();

    [Fact]
    public void Should_Pass_For_Valid_SkillName()
    {
        // Arrange
        var cmd = new AddUserSkillCommand(Guid.NewGuid(), "Guitar");

        // Act
        var result = _validator.TestValidate(cmd);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Fail_When_SkillName_Is_Empty()
    {
        var cmd = new AddUserSkillCommand(Guid.NewGuid(), string.Empty);

        var result = _validator.TestValidate(cmd);

        result.ShouldHaveValidationErrorFor(x => x.SkillName)
              .WithErrorMessage("Skill name cannot be empty.");
    }

    [Fact]
    public void Should_Fail_When_SkillName_Is_Too_Short()
    {
        var cmd = new AddUserSkillCommand(Guid.NewGuid(), "ab"); // 2 chars < min(3)

        var result = _validator.TestValidate(cmd);

        result.ShouldHaveValidationErrorFor(x => x.SkillName)
              .WithErrorMessage("Skill name must be at least 3 characters long.");
    }

    [Fact]
    public void Should_Fail_When_SkillName_Is_Too_Long()
    {
        var tooLong = new string('x', 21); // > max(20)
        var cmd = new AddUserSkillCommand(Guid.NewGuid(), tooLong);

        var result = _validator.TestValidate(cmd);

        result.ShouldHaveValidationErrorFor(x => x.SkillName)
              .WithErrorMessage("Skill name cannot be longer than 20 characters.");
    }
    
    [Fact]
    public void Should_Fail_When_SkillName_Is_Null()
    {
        var cmd = new AddUserSkillCommand(Guid.NewGuid(), null!);

        var result = _validator.TestValidate(cmd);

        result.ShouldHaveValidationErrorFor(x => x.SkillName)
              .WithErrorMessage("Skill name cannot be empty.");
    }
}
