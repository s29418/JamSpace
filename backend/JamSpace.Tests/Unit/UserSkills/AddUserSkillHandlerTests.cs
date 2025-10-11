using FluentAssertions;
using Moq;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Features.UserSkills.Commands.AddUserSkill;
using JamSpace.Application.Features.UserSkills.DTOs;
using JamSpace.Domain.Entities; 

namespace JamSpace.Tests.Unit.UserSkills;

public class AddUserSkillHandlerTests
{
    private readonly Mock<IUserSkillRepository> _userSkillRepo = new();
    private readonly Mock<ISkillRepository> _skillRepo = new();

    [Fact]
    public async Task Handle_Should_Link_Existing_Skill_When_User_Does_Not_Have_It()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var skill = new Skill { Id = Guid.NewGuid(), Name = "Guitar" };
        var cmd = new AddUserSkillCommand(userId, "Guitar");

        _skillRepo.Setup(r => r.GetSkillByNameAsync("Guitar", It.IsAny<CancellationToken>()))
                  .ReturnsAsync(skill);

        _userSkillRepo.Setup(r => r.UserHasSkillAsync(userId, skill.Id, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(false);

        _userSkillRepo
            .Setup(r => r.AddUserSkillAsync(userId, skill.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserSkill
            {
                UserId = userId,
                SkillId = skill.Id,
                Skill = skill  
            });

        var sut = new AddUserSkillHandler(_userSkillRepo.Object, _skillRepo.Object);

        // Act
        UserSkillDto dto = await sut.Handle(cmd, CancellationToken.None);

        // Assert
        dto.SkillId.Should().Be(skill.Id);
        dto.SkillName.Should().Be("Guitar");
        _userSkillRepo.Verify(r => r.AddUserSkillAsync(userId, skill.Id, It.IsAny<CancellationToken>()), Times.Once);
        _skillRepo.Verify(r => r.CreateSkillAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Create_Skill_If_Not_Exists_And_Link()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var created = new Skill { Id = Guid.NewGuid(), Name = "Piano" };
        var cmd = new AddUserSkillCommand(userId, "Piano");

        _skillRepo.Setup(r => r.GetSkillByNameAsync("Piano", It.IsAny<CancellationToken>()))
                  .ReturnsAsync((Skill?)null);

        _skillRepo.Setup(r => r.CreateSkillAsync("Piano", It.IsAny<CancellationToken>()))
                  .ReturnsAsync(created);

        _userSkillRepo
            .Setup(r => r.AddUserSkillAsync(userId, created.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserSkill
            {
                UserId = userId,
                SkillId = created.Id,
                Skill = created
            });

        var sut = new AddUserSkillHandler(_userSkillRepo.Object, _skillRepo.Object);

        // Act
        var dto = await sut.Handle(cmd, CancellationToken.None);

        // Assert
        dto.SkillId.Should().Be(created.Id);
        dto.SkillName.Should().Be("Piano");
        _userSkillRepo.Verify(r => r.AddUserSkillAsync(userId, created.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_Conflict_When_User_Already_Has_Skill()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var skill = new Skill { Id = Guid.NewGuid(), Name = "Vocal" };
        var cmd = new AddUserSkillCommand(userId, "Vocal");

        _skillRepo.Setup(r => r.GetSkillByNameAsync("Vocal", It.IsAny<CancellationToken>()))
                  .ReturnsAsync(skill);

        _userSkillRepo.Setup(r => r.UserHasSkillAsync(userId, skill.Id, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(true);

        var sut = new AddUserSkillHandler(_userSkillRepo.Object, _skillRepo.Object);

        // Act
        var act = () => sut.Handle(cmd, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
                 .WithMessage("User already has this skill.");
        _userSkillRepo.Verify(r => r.AddUserSkillAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
