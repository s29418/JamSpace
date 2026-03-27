using FluentAssertions;
using FluentValidation;
using Moq;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.Conversations.Commands.SendMessage;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;

namespace JamSpace.Tests.Unit.Conversations;

public class SendMessageHandlerTests
{
    [Fact]
    public async Task Should_Throw_ValidationException_When_Content_Is_Empty()
    {
        var conversationRepo = new Mock<IConversationRepository>();
        var participantRepo = new Mock<IConversationParticipantRepository>();
        var messageRepo = new Mock<IMessageRepository>();
        var uow = new Mock<IUnitOfWork>();

        var handler = new SendMessageHandler(
            conversationRepo.Object,
            participantRepo.Object,
            messageRepo.Object,
            uow.Object);

        var command = new SendMessageCommand(Guid.NewGuid(), Guid.NewGuid(), "   ", null);

        await Assert.ThrowsAsync<ValidationException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Should_Throw_Forbidden_When_User_Is_Not_Participant()
    {
        var conversationRepo = new Mock<IConversationRepository>();
        var participantRepo = new Mock<IConversationParticipantRepository>();
        var messageRepo = new Mock<IMessageRepository>();
        var uow = new Mock<IUnitOfWork>();

        participantRepo.Setup(r => r.IsParticipantAsync(
                It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new SendMessageHandler(
            conversationRepo.Object,
            participantRepo.Object,
            messageRepo.Object,
            uow.Object);

        var command = new SendMessageCommand(Guid.NewGuid(), Guid.NewGuid(), "Hello", null);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Should_Throw_NotFound_When_Conversation_Does_Not_Exist()
    {
        var conversationId = Guid.NewGuid();
        var senderUserId = Guid.NewGuid();

        var conversationRepo = new Mock<IConversationRepository>();
        var participantRepo = new Mock<IConversationParticipantRepository>();
        var messageRepo = new Mock<IMessageRepository>();
        var uow = new Mock<IUnitOfWork>();

        participantRepo.Setup(r => r.IsParticipantAsync(conversationId, senderUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        conversationRepo.Setup(r => r.GetByIdAsync(conversationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Conversation?)null);

        var handler = new SendMessageHandler(
            conversationRepo.Object,
            participantRepo.Object,
            messageRepo.Object,
            uow.Object);

        var command = new SendMessageCommand(conversationId, senderUserId, "Hello", null);

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Should_Send_Message_When_Request_Is_Valid()
    {
        var conversationId = Guid.NewGuid();
        var senderUserId = Guid.NewGuid();

        var conversation = new Conversation
        {
            Id = conversationId,
            Type = ChatType.Direct
        };

        var conversationRepo = new Mock<IConversationRepository>();
        var participantRepo = new Mock<IConversationParticipantRepository>();
        var messageRepo = new Mock<IMessageRepository>();
        var uow = new Mock<IUnitOfWork>();

        participantRepo.Setup(r => r.IsParticipantAsync(conversationId, senderUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        conversationRepo.Setup(r => r.GetByIdAsync(conversationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);

        messageRepo.Setup(r => r.AddAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        uow.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new SendMessageHandler(
            conversationRepo.Object,
            participantRepo.Object,
            messageRepo.Object,
            uow.Object);

        var command = new SendMessageCommand(conversationId, senderUserId, "  Hello world  ", null);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.ConversationId.Should().Be(conversationId);
        result.SenderUserId.Should().Be(senderUserId);
        result.Content.Should().Be("Hello world");
        result.ReplyToMessageId.Should().BeNull();

        conversation.LastMessageId.Should().Be(result.Id);
        conversation.LastMessageAt.Should().Be(result.CreatedAt);
        conversation.LastMessagePreview.Should().Be("Hello world");

        messageRepo.Verify(r => r.AddAsync(
            It.Is<Message>(m =>
                m.ConversationId == conversationId &&
                m.SenderUserId == senderUserId &&
                m.Content == "Hello world" &&
                m.ReplyToMessageId == null),
            It.IsAny<CancellationToken>()), Times.Once);

        uow.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_Send_Message_With_ReplyToMessageId_When_Provided()
    {
        var conversationId = Guid.NewGuid();
        var senderUserId = Guid.NewGuid();
        var replyToMessageId = Guid.NewGuid();

        var conversation = new Conversation
        {
            Id = conversationId,
            Type = ChatType.Direct
        };

        var conversationRepo = new Mock<IConversationRepository>();
        var participantRepo = new Mock<IConversationParticipantRepository>();
        var messageRepo = new Mock<IMessageRepository>();
        var uow = new Mock<IUnitOfWork>();

        participantRepo.Setup(r => r.IsParticipantAsync(conversationId, senderUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        conversationRepo.Setup(r => r.GetByIdAsync(conversationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);

        messageRepo.Setup(r => r.AddAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        uow.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new SendMessageHandler(
            conversationRepo.Object,
            participantRepo.Object,
            messageRepo.Object,
            uow.Object);

        var command = new SendMessageCommand(conversationId, senderUserId, "Reply message", replyToMessageId);

        var result = await handler.Handle(command, CancellationToken.None);

        result.ReplyToMessageId.Should().Be(replyToMessageId);

        messageRepo.Verify(r => r.AddAsync(
            It.Is<Message>(m => m.ReplyToMessageId == replyToMessageId),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}