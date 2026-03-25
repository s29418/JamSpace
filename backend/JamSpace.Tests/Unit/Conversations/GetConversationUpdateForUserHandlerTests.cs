using FluentAssertions;
using Moq;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.Conversations.Queries.GetConversationUpdateForUser;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;

namespace JamSpace.Tests.Unit.Conversations;

public class GetConversationUpdateForUserHandlerTests
{
    [Fact]
    public async Task Should_Throw_NotFound_When_Conversation_Does_Not_Exist()
    {
        var conversationParticipantRepo = new Mock<IConversationParticipantRepository>();
        var conversationRepo = new Mock<IConversationRepository>();
        var messageRepo = new Mock<IMessageRepository>();

        conversationRepo
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Conversation?)null);

        var handler = new GetConversationUpdateForUserHandler(
            conversationParticipantRepo.Object,
            conversationRepo.Object,
            messageRepo.Object);

        var act = async () => await handler.Handle(
            new GetConversationUpdateForUserQuery(Guid.NewGuid(), Guid.NewGuid()),
            CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Should_Throw_Forbidden_When_User_Is_Not_Participant()
    {
        var conversationParticipantRepo = new Mock<IConversationParticipantRepository>();
        var conversationRepo = new Mock<IConversationRepository>();
        var messageRepo = new Mock<IMessageRepository>();

        var conversationId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        conversationRepo
            .Setup(x => x.GetByIdAsync(conversationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Conversation
            {
                Id = conversationId,
                Type = ChatType.Direct
            });

        conversationParticipantRepo
            .Setup(x => x.IsParticipantAsync(conversationId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new GetConversationUpdateForUserHandler(
            conversationParticipantRepo.Object,
            conversationRepo.Object,
            messageRepo.Object);

        var act = async () => await handler.Handle(
            new GetConversationUpdateForUserQuery(conversationId, userId),
            CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }

    [Fact]
    public async Task Should_Return_Conversation_Update_For_User()
    {
        var conversationParticipantRepo = new Mock<IConversationParticipantRepository>();
        var conversationRepo = new Mock<IConversationRepository>();
        var messageRepo = new Mock<IMessageRepository>();

        var conversationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var lastReadAt = DateTimeOffset.UtcNow.AddMinutes(-10);
        var lastMessageAt = DateTimeOffset.UtcNow.AddMinutes(-1);

        var conversation = new Conversation
        {
            Id = conversationId,
            Type = ChatType.Direct,
            LastMessageAt = lastMessageAt,
            LastMessagePreview = "Latest preview"
        };

        conversationRepo
            .Setup(x => x.GetByIdAsync(conversationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);

        conversationParticipantRepo
            .Setup(x => x.IsParticipantAsync(conversationId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        conversationParticipantRepo
            .Setup(x => x.GetLastReadAt(conversationId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(lastReadAt);

        messageRepo
            .Setup(x => x.CountUnreadAsync(conversationId, userId, lastReadAt, It.IsAny<CancellationToken>()))
            .ReturnsAsync(5);

        var handler = new GetConversationUpdateForUserHandler(
            conversationParticipantRepo.Object,
            conversationRepo.Object,
            messageRepo.Object);

        var result = await handler.Handle(
            new GetConversationUpdateForUserQuery(conversationId, userId),
            CancellationToken.None);

        result.ConversationId.Should().Be(conversationId);
        result.LastMessageAt.Should().Be(lastMessageAt);
        result.LastMessagePreview.Should().Be("Latest preview");
        result.UnreadCount.Should().Be(5);
    }
}