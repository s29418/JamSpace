using FluentAssertions;
using JamSpace.Application.Features.Users.Commands.UpdateUserProfile;

namespace JamSpace.Tests.Unit.Users;

public class UpdateUserProfileCommandValidatorTests
{
    private readonly UpdateUserProfileCommandValidator _validator = new();

    [Fact]
    public void Should_Fail_When_DisplayName_Too_Long()
    {
        // Arrange
        var cmd = new UpdateUserProfileCommand(
            Guid.NewGuid(),
            true, new string('a', 51),
            false, null,
            false, null,
            false, null,
            false, null 
        );

        // Act
        var result = _validator.Validate(cmd);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == "DisplayName" &&
            e.ErrorMessage == "Display name cannot be longer than 50 characters.");
    }

    [Fact]
    public void Should_Fail_When_DisplayName_Too_Short()
    {
        // Arrange
        var cmd = new UpdateUserProfileCommand(
            Guid.NewGuid(),
            true, "ab", 
            false, null,
            false, null,
            false, null,
            false, null
        );

        // Act
        var result = _validator.Validate(cmd);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == "DisplayName" &&
            e.ErrorMessage == "Display name must be at least 3 characters long.");
    }

    [Fact]
    public void Should_Fail_When_DisplayName_Empty()
    {
        // Arrange
        var cmd = new UpdateUserProfileCommand(
            Guid.NewGuid(),
            true, "",
            false, null,
            false, null,
            false, null,
            false, null
        );

        // Act
        var result = _validator.Validate(cmd);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == "DisplayName" &&
            e.ErrorMessage == "Display name cannot be empty.");
    }

    [Fact]
    public void Should_Fail_When_Bio_Too_Long()
    {
        // Arrange
        var cmd = new UpdateUserProfileCommand(
            Guid.NewGuid(),
            false, null,
            true, new string('b', 171),
            false, null,
            false, null,
            false, null
        );

        // Act
        var result = _validator.Validate(cmd);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == "Bio" &&
            e.ErrorMessage == "Bio cannot be longer than 170 characters.");
    }

    [Fact]
    public void Should_Fail_When_Email_Invalid()
    {
        // Arrange
        var cmd = new UpdateUserProfileCommand(
            Guid.NewGuid(),
            false, null,
            false, null,
            false, null,
            false, null,
            true, "not-an-email"
        );

        // Act
        var result = _validator.Validate(cmd);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == "Email" &&
            e.ErrorMessage == "Email is not valid.");
    }

    [Fact]
    public void Should_Pass_When_No_Flags_Are_Set()
    {
        // Arrange
        var cmd = new UpdateUserProfileCommand(
            Guid.NewGuid(),
            false, null,
            false, null,
            false, null,
            false, null,
            false, null
        );

        // Act
        var result = _validator.Validate(cmd);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
