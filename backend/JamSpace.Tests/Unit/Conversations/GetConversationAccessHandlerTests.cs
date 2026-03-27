using FluentAssertions;
using Moq;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.Conversations.Queries.GetConversationAccess;

namespace JamSpace.Tests.Unit.Conversations;

public class GetConversationAccessHandlerTests
{
    [Fact]
    public async Task Should_Return_True_When_User_Is_Participant()
    {
        var conversationParticipantRepo = new Mock<IConversationParticipantRepository>();

        var conversationId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        conversationParticipantRepo
            .Setup(x => x.IsParticipantAsync(conversationId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new GetConversationAccessHandler(conversationParticipantRepo.Object);

        var result = await handler.Handle(
            new GetConversationAccessQuery(conversationId, userId),
            CancellationToken.None);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task Should_Return_False_When_User_Is_Not_Participant()
    {
        var conversationParticipantRepo = new Mock<IConversationParticipantRepository>();

        var conversationId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        conversationParticipantRepo
            .Setup(x => x.IsParticipantAsync(conversationId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new GetConversationAccessHandler(conversationParticipantRepo.Object);

        var result = await handler.Handle(
            new GetConversationAccessQuery(conversationId, userId),
            CancellationToken.None);

        result.Should().BeFalse();
    }
}