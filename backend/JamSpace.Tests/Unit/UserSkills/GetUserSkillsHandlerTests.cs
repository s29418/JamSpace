using FluentAssertions;
using Moq;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.UserSkills.DTOs;
using JamSpace.Application.Features.UserSkills.Queries.GetUserSkills;
using JamSpace.Domain.Entities; 

namespace JamSpace.Tests.Unit.UserSkills;

public class GetUserSkillsHandlerTests
{
    private readonly Mock<IUserSkillRepository> _repo = new();

    [Fact]
    public async Task Handle_Should_Return_Mapped_Dtos()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var rows = new List<UserSkill>
        {
            new UserSkill { UserId = userId, SkillId = Guid.NewGuid(), Skill = new Skill { Id = Guid.NewGuid(), Name = "Bass" } },
            new UserSkill { UserId = userId, SkillId = Guid.NewGuid(), Skill = new Skill { Id = Guid.NewGuid(), Name = "Drums" } },
        };

        _repo.Setup(r => r.GetAllUserSkillsAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rows);

        var sut = new GetUserSkillsHandler(_repo.Object);

        // Act
        List<UserSkillDto> result = await sut.Handle(new GetUserSkillsQuery(userId), CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result[0].SkillName.Should().Be("Bass");
        result[1].SkillName.Should().Be("Drums");
    }

    [Fact]
    public async Task Handle_Should_Return_Empty_List_When_No_Skills()
    {
        var userId = Guid.NewGuid();

        _repo.Setup(r => r.GetAllUserSkillsAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserSkill>());

        var sut = new GetUserSkillsHandler(_repo.Object);

        var result = await sut.Handle(new GetUserSkillsQuery(userId), CancellationToken.None);

        result.Should().NotBeNull().And.BeEmpty();
    }
}