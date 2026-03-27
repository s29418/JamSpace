using FluentAssertions;
using Moq;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Models;
using JamSpace.Application.Features.Conversations.DTOs;
using JamSpace.Application.Features.Conversations.Queries.GetConversationMessages;
using JamSpace.Domain.Entities;

namespace JamSpace.Tests.Unit.Conversations;

public class GetConversationMessagesHandlerTests
{
    [Fact]
    public async Task Should_Throw_Forbidden_When_User_Is_Not_Participant()
    {
        var messageRepo = new Mock<IMessageRepository>();
        var conversationParticipantRepo = new Mock<IConversationParticipantRepository>();

        var conversationId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        conversationParticipantRepo
            .Setup(x => x.IsParticipantAsync(conversationId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new GetConversationMessagesHandler(
            messageRepo.Object,
            conversationParticipantRepo.Object);

        var act = async () => await handler.Handle(
            new GetConversationMessagesQuery(conversationId, userId, null, 20),
            CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }

    [Fact]
    public async Task Should_Return_Empty_Result_When_No_Messages_Found()
    {
        var messageRepo = new Mock<IMessageRepository>();
        var conversationParticipantRepo = new Mock<IConversationParticipantRepository>();

        var conversationId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        conversationParticipantRepo
            .Setup(x => x.IsParticipantAsync(conversationId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        messageRepo
            .Setup(x => x.GetMessagesAsync(conversationId, null, 21, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Message>());

        var handler = new GetConversationMessagesHandler(
            messageRepo.Object,
            conversationParticipantRepo.Object);

        var result = await handler.Handle(
            new GetConversationMessagesQuery(conversationId, userId, null, 20),
            CancellationToken.None);

        result.Items.Should().BeEmpty();
        result.HasMore.Should().BeFalse();
        result.NextBefore.Should().BeNull();
    }

    [Fact]
    public async Task Should_Clamp_Take_To_One_When_Value_Is_Too_Low()
    {
        var messageRepo = new Mock<IMessageRepository>();
        var conversationParticipantRepo = new Mock<IConversationParticipantRepository>();

        var conversationId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        conversationParticipantRepo
            .Setup(x => x.IsParticipantAsync(conversationId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        messageRepo
            .Setup(x => x.GetMessagesAsync(conversationId, null, 2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Message>());

        var handler = new GetConversationMessagesHandler(
            messageRepo.Object,
            conversationParticipantRepo.Object);

        await handler.Handle(
            new GetConversationMessagesQuery(conversationId, userId, null, 0),
            CancellationToken.None);

        messageRepo.Verify(
            x => x.GetMessagesAsync(conversationId, null, 2, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Should_Clamp_Take_To_Hundred_When_Value_Is_Too_High()
    {
        var messageRepo = new Mock<IMessageRepository>();
        var conversationParticipantRepo = new Mock<IConversationParticipantRepository>();

        var conversationId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        conversationParticipantRepo
            .Setup(x => x.IsParticipantAsync(conversationId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        messageRepo
            .Setup(x => x.GetMessagesAsync(conversationId, null, 101, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Message>());

        var handler = new GetConversationMessagesHandler(
            messageRepo.Object,
            conversationParticipantRepo.Object);

        await handler.Handle(
            new GetConversationMessagesQuery(conversationId, userId, null, 999),
            CancellationToken.None);

        messageRepo.Verify(
            x => x.GetMessagesAsync(conversationId, null, 101, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Should_Return_Messages_In_Ascending_Order_For_Page()
    {
        var messageRepo = new Mock<IMessageRepository>();
        var conversationParticipantRepo = new Mock<IConversationParticipantRepository>();

        var conversationId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var sender1 = new User
        {
            Id = Guid.NewGuid(),
            DisplayName = "User 1",
            ProfilePictureUrl = "1.png",
            UserName = "User 1"
        };
        var sender2 = new User
        {
            Id = Guid.NewGuid(), 
            DisplayName = "User 2", 
            ProfilePictureUrl = "2.png" ,
            UserName = "User 2"
        };

        var newer = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = conversationId,
            SenderUserId = sender1.Id,
            SenderUser = sender1,
            Content = "newer",
            CreatedAt = DateTimeOffset.UtcNow
        };

        var older = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = conversationId,
            SenderUserId = sender2.Id,
            SenderUser = sender2,
            Content = "older",
            CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-1)
        };

        conversationParticipantRepo
            .Setup(x => x.IsParticipantAsync(conversationId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        messageRepo
            .Setup(x => x.GetMessagesAsync(conversationId, null, 3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Message> { newer, older });

        var handler = new GetConversationMessagesHandler(
            messageRepo.Object,
            conversationParticipantRepo.Object);

        var result = await handler.Handle(
            new GetConversationMessagesQuery(conversationId, userId, null, 2),
            CancellationToken.None);

        result.Items.Should().HaveCount(2);
        result.Items[0].Content.Should().Be("older");
        result.Items[1].Content.Should().Be("newer");
        result.HasMore.Should().BeFalse();
        result.NextBefore.Should().Be(older.CreatedAt);
    }

    [Fact]
    public async Task Should_Return_HasMore_And_NextBefore_When_More_Messages_Exist()
    {
        var messageRepo = new Mock<IMessageRepository>();
        var conversationParticipantRepo = new Mock<IConversationParticipantRepository>();

        var conversationId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var sender = new User
        {
            Id = Guid.NewGuid(),
            DisplayName = "User",
            ProfilePictureUrl = "user.png",
            UserName = "User"
        };

        var newest = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = conversationId,
            SenderUserId = sender.Id,
            SenderUser = sender,
            Content = "msg3",
            CreatedAt = DateTimeOffset.UtcNow
        };

        var middle = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = conversationId,
            SenderUserId = sender.Id,
            SenderUser = sender,
            Content = "msg2",
            CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-1)
        };

        var oldest = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = conversationId,
            SenderUserId = sender.Id,
            SenderUser = sender,
            Content = "msg1",
            CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-2)
        };

        conversationParticipantRepo
            .Setup(x => x.IsParticipantAsync(conversationId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        messageRepo
            .Setup(x => x.GetMessagesAsync(conversationId, null, 3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Message> { newest, middle, oldest });

        var handler = new GetConversationMessagesHandler(
            messageRepo.Object,
            conversationParticipantRepo.Object);

        var result = await handler.Handle(
            new GetConversationMessagesQuery(conversationId, userId, null, 2),
            CancellationToken.None);

        result.Items.Should().HaveCount(2);
        result.Items[0].Content.Should().Be("msg2");
        result.Items[1].Content.Should().Be("msg3");
        result.HasMore.Should().BeTrue();
        result.NextBefore.Should().Be(middle.CreatedAt);
    }
}