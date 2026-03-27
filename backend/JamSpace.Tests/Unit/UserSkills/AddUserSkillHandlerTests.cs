using FluentAssertions;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.UserSkills.Commands.AddUserSkill;
using JamSpace.Domain.Entities;
using Moq;

namespace JamSpace.Tests.Unit.UserSkills;

public class AddUserSkillHandlerTests
{
    private readonly Mock<IUserSkillRepository> _userSkillRepo = new();
    private readonly Mock<ISkillRepository> _skillRepo = new();
    private readonly Mock<IUnitOfWork> _uow = new();

    [Fact]
    public async Task Handle_Should_Link_Existing_Skill_When_User_Does_Not_Have_It()
    {
        var userId = Guid.NewGuid();
        var skill = new Skill { Id = Guid.NewGuid(), Name = "Guitar" };
        var cmd = new AddUserSkillCommand(userId, "Guitar");

        _skillRepo.Setup(r => r.GetSkillByNameAsync("Guitar", It.IsAny<CancellationToken>()))
            .ReturnsAsync(skill);

        _userSkillRepo.Setup(r => r.UserHasSkillAsync(userId, skill.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var sut = new AddUserSkillHandler(_userSkillRepo.Object, _skillRepo.Object, _uow.Object);

        var dto = await sut.Handle(cmd, CancellationToken.None);

        dto.SkillId.Should().Be(skill.Id);
        dto.SkillName.Should().Be("Guitar");

        _skillRepo.Verify(r => r.AddAsync(It.IsAny<Skill>(), It.IsAny<CancellationToken>()), Times.Never);
        _userSkillRepo.Verify(r => r.AddAsync(It.IsAny<UserSkill>(), It.IsAny<CancellationToken>()), Times.Once);
        _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Create_Skill_If_Not_Exists_And_Link()
    {
        var userId = Guid.NewGuid();
        var cmd = new AddUserSkillCommand(userId, "Piano");

        _skillRepo.Setup(r => r.GetSkillByNameAsync("Piano", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Skill?)null);

        _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        Skill? createdSkill = null;
        _skillRepo.Setup(r => r.AddAsync(It.IsAny<Skill>(), It.IsAny<CancellationToken>()))
            .Callback<Skill, CancellationToken>((s, _) => createdSkill = s)
            .Returns(Task.CompletedTask);

        _userSkillRepo.Setup(r => r.UserHasSkillAsync(userId, It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var sut = new AddUserSkillHandler(_userSkillRepo.Object, _skillRepo.Object, _uow.Object);

        var dto = await sut.Handle(cmd, CancellationToken.None);

        createdSkill.Should().NotBeNull();
        dto.SkillId.Should().Be(createdSkill!.Id);
        dto.SkillName.Should().Be(createdSkill.Name);

        _skillRepo.Verify(r => r.AddAsync(It.IsAny<Skill>(), It.IsAny<CancellationToken>()), Times.Once);
        _userSkillRepo.Verify(r => r.AddAsync(It.IsAny<UserSkill>(), It.IsAny<CancellationToken>()), Times.Once);
        _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_Conflict_When_User_Already_Has_Skill()
    {
        var userId = Guid.NewGuid();
        var skill = new Skill { Id = Guid.NewGuid(), Name = "Vocal" };
        var cmd = new AddUserSkillCommand(userId, "Vocal");

        _skillRepo.Setup(r => r.GetSkillByNameAsync("Vocal", It.IsAny<CancellationToken>()))
            .ReturnsAsync(skill);

        _userSkillRepo.Setup(r => r.UserHasSkillAsync(userId, skill.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var sut = new AddUserSkillHandler(_userSkillRepo.Object, _skillRepo.Object, _uow.Object);

        Func<Task> act = () => sut.Handle(cmd, CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("User already has this skill.");

        _userSkillRepo.Verify(r => r.AddAsync(It.IsAny<UserSkill>(), It.IsAny<CancellationToken>()), Times.Never);
        _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}