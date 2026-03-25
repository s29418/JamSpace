using FluentAssertions;
using Moq;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.Conversations.Commands.MarkConversationAsRead;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;

namespace JamSpace.Tests.Unit.Conversations;

public class MarkConversationAsReadHandlerTests
{
    [Fact]
    public async Task Should_Throw_NotFound_When_Conversation_Does_Not_Exist()
    {
        var conversationRepo = new Mock<IConversationRepository>();
        var messageRepo = new Mock<IMessageRepository>();
        var uow = new Mock<IUnitOfWork>();

        conversationRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Conversation?)null);

        var handler = new MarkConversationAsReadHandler(
            conversationRepo.Object,
            uow.Object,
            messageRepo.Object);

        var command = new MarkConversationAsReadCommand(Guid.NewGuid(), Guid.NewGuid(), null);

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Should_Throw_Forbidden_When_User_Is_Not_Participant()
    {
        var conversationId = Guid.NewGuid();
        var requestingUserId = Guid.NewGuid();

        var conversationRepo = new Mock<IConversationRepository>();
        var messageRepo = new Mock<IMessageRepository>();
        var uow = new Mock<IUnitOfWork>();

        conversationRepo.Setup(r => r.GetByIdAsync(conversationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Conversation
            {
                Id = conversationId,
                Type = ChatType.Direct,
                Participants = new List<ConversationParticipant>()
            });

        var handler = new MarkConversationAsReadHandler(
            conversationRepo.Object,
            uow.Object,
            messageRepo.Object);

        var command = new MarkConversationAsReadCommand(conversationId, requestingUserId, null);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Should_Throw_NotFound_When_Message_Does_Not_Exist()
    {
        var conversationId = Guid.NewGuid();
        var requestingUserId = Guid.NewGuid();
        var messageId = Guid.NewGuid();

        var participant = new ConversationParticipant
        {
            ConversationId = conversationId,
            UserId = requestingUserId
        };

        var conversationRepo = new Mock<IConversationRepository>();
        var messageRepo = new Mock<IMessageRepository>();
        var uow = new Mock<IUnitOfWork>();

        conversationRepo.Setup(r => r.GetByIdAsync(conversationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Conversation
            {
                Id = conversationId,
                Type = ChatType.Direct,
                Participants = new List<ConversationParticipant> { participant }
            });

        messageRepo.Setup(r => r.GetByIdAsync(messageId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Message?)null);

        var handler = new MarkConversationAsReadHandler(
            conversationRepo.Object,
            uow.Object,
            messageRepo.Object);

        var command = new MarkConversationAsReadCommand(conversationId, requestingUserId, messageId);

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Should_Throw_NotFound_When_Message_Belongs_To_Other_Conversation()
    {
        var conversationId = Guid.NewGuid();
        var otherConversationId = Guid.NewGuid();
        var requestingUserId = Guid.NewGuid();
        var messageId = Guid.NewGuid();

        var participant = new ConversationParticipant
        {
            ConversationId = conversationId,
            UserId = requestingUserId
        };

        var conversationRepo = new Mock<IConversationRepository>();
        var messageRepo = new Mock<IMessageRepository>();
        var uow = new Mock<IUnitOfWork>();

        conversationRepo.Setup(r => r.GetByIdAsync(conversationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Conversation
            {
                Id = conversationId,
                Type = ChatType.Direct,
                Participants = new List<ConversationParticipant> { participant }
            });

        messageRepo.Setup(r => r.GetByIdAsync(messageId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Message
            {
                Id = messageId,
                ConversationId = otherConversationId,
                Content = "null",
                CreatedAt = DateTimeOffset.Now
            });

        var handler = new MarkConversationAsReadHandler(
            conversationRepo.Object,
            uow.Object,
            messageRepo.Object);

        var command = new MarkConversationAsReadCommand(conversationId, requestingUserId, messageId);

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Should_Mark_Conversation_As_Read_Using_Last_Conversation_Message_When_Message_Id_Not_Provided()
    {
        var conversationId = Guid.NewGuid();
        var requestingUserId = Guid.NewGuid();
        var lastMessageId = Guid.NewGuid();
        var lastMessageAt = DateTimeOffset.UtcNow;

        var participant = new ConversationParticipant
        {
            ConversationId = conversationId,
            UserId = requestingUserId
        };

        var conversationRepo = new Mock<IConversationRepository>();
        var messageRepo = new Mock<IMessageRepository>();
        var uow = new Mock<IUnitOfWork>();

        conversationRepo.Setup(r => r.GetByIdAsync(conversationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Conversation
            {
                Id = conversationId,
                Type = ChatType.Direct,
                LastMessageId = lastMessageId,
                LastMessageAt = lastMessageAt,
                Participants = new List<ConversationParticipant> { participant }
            });

        uow.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new MarkConversationAsReadHandler(
            conversationRepo.Object,
            uow.Object,
            messageRepo.Object);

        var result = await handler.Handle(
            new MarkConversationAsReadCommand(conversationId, requestingUserId, null),
            CancellationToken.None);

        result.ConversationId.Should().Be(conversationId);
        result.UserId.Should().Be(requestingUserId);
        result.LastReadMessageId.Should().Be(lastMessageId);
        result.LastReadAt.Should().Be(lastMessageAt);

        participant.LastReadMessageId.Should().Be(lastMessageId);
        participant.LastReadAt.Should().Be(lastMessageAt);

        uow.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_Mark_Conversation_As_Read_Using_Provided_Message()
    {
        var conversationId = Guid.NewGuid();
        var requestingUserId = Guid.NewGuid();
        var messageId = Guid.NewGuid();
        var messageCreatedAt = DateTimeOffset.UtcNow;

        var participant = new ConversationParticipant
        {
            ConversationId = conversationId,
            UserId = requestingUserId
        };

        var conversationRepo = new Mock<IConversationRepository>();
        var messageRepo = new Mock<IMessageRepository>();
        var uow = new Mock<IUnitOfWork>();

        conversationRepo.Setup(r => r.GetByIdAsync(conversationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Conversation
            {
                Id = conversationId,
                Type = ChatType.Direct,
                Participants = new List<ConversationParticipant> { participant }
            });

        messageRepo.Setup(r => r.GetByIdAsync(messageId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Message
            {
                Id = messageId,
                ConversationId = conversationId,
                CreatedAt = messageCreatedAt,
                Content = "null"
            });

        uow.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new MarkConversationAsReadHandler(
            conversationRepo.Object,
            uow.Object,
            messageRepo.Object);

        var result = await handler.Handle(
            new MarkConversationAsReadCommand(conversationId, requestingUserId, messageId),
            CancellationToken.None);

        result.LastReadMessageId.Should().Be(messageId);
        result.LastReadAt.Should().Be(messageCreatedAt);

        participant.LastReadMessageId.Should().Be(messageId);
        participant.LastReadAt.Should().Be(messageCreatedAt);

        uow.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}