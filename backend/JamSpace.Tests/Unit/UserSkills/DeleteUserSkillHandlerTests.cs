using FluentAssertions;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.UserSkills.Commands.DeleteUserSkill;
using JamSpace.Domain.Entities;
using Moq;

namespace JamSpace.Tests.Unit.UserSkills;

public class DeleteUserSkillHandlerTests
{
    private readonly Mock<IUserSkillRepository> _repo = new();
    private readonly Mock<IUnitOfWork> _uow = new();

    [Fact]
    public async Task Handle_Should_Remove_Link_When_User_Has_Skill()
    {
        var userId = Guid.NewGuid();
        var skillId = Guid.NewGuid();
        var cmd = new DeleteUserSkillCommand(userId, skillId);

        var link = new UserSkill { UserId = userId, SkillId = skillId };

        _repo.Setup(r => r.GetUserSkillAsync(userId, skillId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(link);

        _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var sut = new DeleteUserSkillHandler(_repo.Object, _uow.Object);

        var result = await sut.Handle(cmd, CancellationToken.None);

        result.Should().Be(MediatR.Unit.Value);
        _repo.Verify(r => r.Remove(link), Times.Once);
        _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_Conflict_When_User_Does_Not_Have_Skill()
    {
        var userId = Guid.NewGuid();
        var skillId = Guid.NewGuid();
        var cmd = new DeleteUserSkillCommand(userId, skillId);

        _repo.Setup(r => r.GetUserSkillAsync(userId, skillId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserSkill?)null);

        var sut = new DeleteUserSkillHandler(_repo.Object, _uow.Object);

        Func<Task> act = () => sut.Handle(cmd, CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("User does not have this skill.");

        _repo.Verify(r => r.Remove(It.IsAny<UserSkill>()), Times.Never);
        _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}