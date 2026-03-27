using FluentAssertions;
using Moq;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.Conversations.Queries.GetConversationParticipantUserIds;

namespace JamSpace.Tests.Unit.Conversations;

public class GetConversationParticipantUserIdsHandlerTests
{
    [Fact]
    public async Task Should_Return_Participant_User_Ids()
    {
        var conversationParticipantRepo = new Mock<IConversationParticipantRepository>();

        var conversationId = Guid.NewGuid();
        var userIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

        conversationParticipantRepo
            .Setup(x => x.GetUserIdsAsync(conversationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userIds);

        var handler = new GetConversationParticipantUserIdsHandler(conversationParticipantRepo.Object);

        var result = await handler.Handle(
            new GetConversationParticipantUserIdsQuery(conversationId),
            CancellationToken.None);

        result.Should().BeEquivalentTo(userIds);
    }

    [Fact]
    public async Task Should_Return_Empty_List_When_Conversation_Has_No_Participants()
    {
        var conversationParticipantRepo = new Mock<IConversationParticipantRepository>();

        var conversationId = Guid.NewGuid();

        conversationParticipantRepo
            .Setup(x => x.GetUserIdsAsync(conversationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Guid>());

        var handler = new GetConversationParticipantUserIdsHandler(conversationParticipantRepo.Object);

        var result = await handler.Handle(
            new GetConversationParticipantUserIdsQuery(conversationId),
            CancellationToken.None);

        result.Should().BeEmpty();
    }
}