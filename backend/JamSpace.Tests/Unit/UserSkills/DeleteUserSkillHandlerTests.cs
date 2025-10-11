using FluentAssertions;
using Moq;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Features.UserSkills.Commands.DeleteUserSkill;

namespace JamSpace.Tests.Unit.UserSkills;

public class DeleteUserSkillHandlerTests
{
    private readonly Mock<IUserSkillRepository> _repo = new();

    [Fact]
    public async Task Handle_Should_Remove_Link_When_User_Has_Skill()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var skillId = Guid.NewGuid();
        var cmd = new DeleteUserSkillCommand(userId, skillId);

        _repo.Setup(r => r.UserHasSkillAsync(userId, skillId, It.IsAny<CancellationToken>()))
             .ReturnsAsync(true);

        _repo.Setup(r => r.RemoveUserSkillAsync(userId, skillId, It.IsAny<CancellationToken>()))
             .Returns(Task.CompletedTask);

        var sut = new DeleteUserSkillHandler(_repo.Object);

        // Act
        MediatR.Unit result = await sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.Should().Be(MediatR.Unit.Value);
        _repo.Verify(r => r.RemoveUserSkillAsync(userId, skillId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_Conflict_When_User_Does_Not_Have_Skill()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var skillId = Guid.NewGuid();
        var cmd = new DeleteUserSkillCommand(userId, skillId);

        _repo.Setup(r => r.UserHasSkillAsync(userId, skillId, It.IsAny<CancellationToken>()))
             .ReturnsAsync(false);

        var sut = new DeleteUserSkillHandler(_repo.Object);

        // Act
        var act = () => sut.Handle(cmd, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
                 .WithMessage("User does not have this skill.");
        _repo.Verify(r => r.RemoveUserSkillAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
