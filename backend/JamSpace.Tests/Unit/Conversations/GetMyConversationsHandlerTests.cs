using FluentAssertions;
using Moq;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.Conversations.DTOs;
using JamSpace.Application.Features.Conversations.Queries.GetMyConversations;
using JamSpace.Application.Features.Conversations.Strategies;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;

namespace JamSpace.Tests.Unit.Conversations;

public class GetMyConversationsHandlerTests
{
    [Fact]
    public async Task Should_Return_Empty_Array_When_User_Has_No_Conversations()
    {
        var conversationRepo = new Mock<IConversationRepository>();
        var messageRepo = new Mock<IMessageRepository>();
        var resolver = new Mock<IConversationCardStrategyResolver>();

        var userId = Guid.NewGuid();

        conversationRepo
            .Setup(x => x.GetForUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Conversation>());

        var handler = new GetMyConversationsHandler(
            conversationRepo.Object,
            resolver.Object,
            messageRepo.Object);

        var result = await handler.Handle(
            new GetMyConversationsQuery(userId),
            CancellationToken.None);

        result.Should().BeEmpty();

        messageRepo.Verify(
            x => x.GetUnreadCountsAsync(It.IsAny<Guid>(), It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()),
            Times.Never);

        resolver.Verify(
            x => x.Resolve(It.IsAny<ChatType>()),
            Times.Never);
    }

    [Fact]
    public async Task Should_Map_All_Conversations_With_Unread_Counts()
    {
        var conversationRepo = new Mock<IConversationRepository>();
        var messageRepo = new Mock<IMessageRepository>();
        var resolver = new Mock<IConversationCardStrategyResolver>();
        var strategy = new Mock<IConversationCardStrategy>();

        var userId = Guid.NewGuid();

        var conversation1 = new Conversation
        {
            Id = Guid.NewGuid(),
            Type = ChatType.Direct
        };

        var conversation2 = new Conversation
        {
            Id = Guid.NewGuid(),
            Type = ChatType.Team
        };

        var conversations = new List<Conversation> { conversation1, conversation2 };

        conversationRepo
            .Setup(x => x.GetForUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversations);

        messageRepo
            .Setup(x => x.GetUnreadCountsAsync(
                userId,
                It.Is<List<Guid>>(ids => ids.Count == 2 && ids.Contains(conversation1.Id) && ids.Contains(conversation2.Id)),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Guid, int>
            {
                [conversation1.Id] = 3,
                [conversation2.Id] = 0
            });

        resolver
            .Setup(x => x.Resolve(It.IsAny<ChatType>()))
            .Returns(strategy.Object);

        strategy
            .Setup(x => x.Map(conversation1, userId, 3))
            .Returns(new ConversationCardDto
            {
                Id = conversation1.Id,
                UnreadCount = 3
            });

        strategy
            .Setup(x => x.Map(conversation2, userId, 0))
            .Returns(new ConversationCardDto
            {
                Id = conversation2.Id,
                UnreadCount = 0
            });

        var handler = new GetMyConversationsHandler(
            conversationRepo.Object,
            resolver.Object,
            messageRepo.Object);

        var result = await handler.Handle(
            new GetMyConversationsQuery(userId),
            CancellationToken.None);

        result.Should().HaveCount(2);
        result.Should().Contain(x => x.Id == conversation1.Id && x.UnreadCount == 3);
        result.Should().Contain(x => x.Id == conversation2.Id && x.UnreadCount == 0);

        resolver.Verify(x => x.Resolve(ChatType.Direct), Times.Once);
        resolver.Verify(x => x.Resolve(ChatType.Team), Times.Once);
        strategy.Verify(x => x.Map(conversation1, userId, 3), Times.Once);
        strategy.Verify(x => x.Map(conversation2, userId, 0), Times.Once);
    }

    [Fact]
    public async Task Should_Default_Unread_Count_To_Zero_When_Not_Present_In_Map()
    {
        var conversationRepo = new Mock<IConversationRepository>();
        var messageRepo = new Mock<IMessageRepository>();
        var resolver = new Mock<IConversationCardStrategyResolver>();
        var strategy = new Mock<IConversationCardStrategy>();

        var userId = Guid.NewGuid();

        var conversation = new Conversation
        {
            Id = Guid.NewGuid(),
            Type = ChatType.Direct
        };

        conversationRepo
            .Setup(x => x.GetForUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Conversation> { conversation });

        messageRepo
            .Setup(x => x.GetUnreadCountsAsync(userId, It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Guid, int>());

        resolver
            .Setup(x => x.Resolve(ChatType.Direct))
            .Returns(strategy.Object);

        strategy
            .Setup(x => x.Map(conversation, userId, 0))
            .Returns(new ConversationCardDto
            {
                Id = conversation.Id,
                UnreadCount = 0
            });

        var handler = new GetMyConversationsHandler(
            conversationRepo.Object,
            resolver.Object,
            messageRepo.Object);

        var result = await handler.Handle(
            new GetMyConversationsQuery(userId),
            CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].Id.Should().Be(conversation.Id);
        result[0].UnreadCount.Should().Be(0);
    }
}