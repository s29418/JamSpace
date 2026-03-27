using FluentAssertions;
using JamSpace.Application.Features.Authentication.Commands.Register;

namespace JamSpace.Tests.Unit.Authentication;

public class RegisterUserCommandValidatorTests
{
    private readonly RegisterUserCommandValidator _validator = new();

    [Fact]
    public void Should_Fail_When_Username_Empty()
    {
        var cmd = new RegisterUserCommand("user@example.com","", "Password123!");

        var result = _validator.Validate(cmd);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == "Username" &&
            e.ErrorMessage == "Username cannot be empty.");
    }

    [Fact]
    public void Should_Fail_When_Username_Too_Short()
    {
        var cmd = new RegisterUserCommand("user@example.com","ab" , "Password123!");

        var result = _validator.Validate(cmd);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == "Username" &&
            e.ErrorMessage == "Username must be at least 3 characters long.");
    }

    [Fact]
    public void Should_Fail_When_Username_Too_Long()
    {
        var cmd = new RegisterUserCommand("user@example.com", 
            new string('a', 51), "Password123!");

        var result = _validator.Validate(cmd);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == "Username" &&
            e.ErrorMessage == "Username cannot be longer than 50 characters.");
    }

    [Fact]
    public void Should_Fail_When_Email_Invalid()
    {
        var cmd = new RegisterUserCommand("user", "not-an-email", "Password123!");

        var result = _validator.Validate(cmd);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == "Email" &&
            e.ErrorMessage == "Email is not valid.");
    }

    [Fact]
    public void Should_Pass_When_Valid()
    {
        var cmd = new RegisterUserCommand("user@example.com","user", "Password123!");

        var result = _validator.Validate(cmd);

        result.IsValid.Should().BeTrue();
    }
}
