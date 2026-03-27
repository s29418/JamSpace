using FluentAssertions;
using Moq;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.Conversations.Commands.GetOrCreateDirectConversation;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;

namespace JamSpace.Tests.Unit.Conversations;

public class GetOrCreateDirectConversationHandlerTests
{
    [Fact]
    public async Task Should_Throw_NotFound_When_Other_User_Does_Not_Exist()
    {
        var conversationRepo = new Mock<IConversationRepository>();
        var participantRepo = new Mock<IConversationParticipantRepository>();
        var userRepo = new Mock<IUserRepository>();
        var uow = new Mock<IUnitOfWork>();

        userRepo.Setup(r => r.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new GetOrCreateDirectConversationHandler(
            conversationRepo.Object,
            uow.Object,
            participantRepo.Object,
            userRepo.Object);

        var command = new GetOrCreateDirectConversationCommand(Guid.NewGuid(), Guid.NewGuid());

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Should_Return_Existing_Conversation_Id_When_Already_Exists()
    {
        var requestingUserId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var existingConversationId = Guid.NewGuid();

        var conversationRepo = new Mock<IConversationRepository>();
        var participantRepo = new Mock<IConversationParticipantRepository>();
        var userRepo = new Mock<IUserRepository>();
        var uow = new Mock<IUnitOfWork>();

        userRepo.Setup(r => r.ExistsAsync(otherUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        conversationRepo.Setup(r => r.GetIdForDirectAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingConversationId);

        var handler = new GetOrCreateDirectConversationHandler(
            conversationRepo.Object,
            uow.Object,
            participantRepo.Object,
            userRepo.Object);

        var result = await handler.Handle(
            new GetOrCreateDirectConversationCommand(requestingUserId, otherUserId),
            CancellationToken.None);

        result.Should().Be(existingConversationId);

        conversationRepo.Verify(r => r.AddAsync(It.IsAny<Conversation>(), It.IsAny<CancellationToken>()), Times.Never);
        participantRepo.Verify(r => r.AddAsync(It.IsAny<ConversationParticipant>(), It.IsAny<CancellationToken>()), Times.Never);
        uow.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Should_Create_Direct_Conversation_When_Not_Exists()
    {
        var requestingUserId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();

        var conversationRepo = new Mock<IConversationRepository>();
        var participantRepo = new Mock<IConversationParticipantRepository>();
        var userRepo = new Mock<IUserRepository>();
        var uow = new Mock<IUnitOfWork>();

        userRepo.Setup(r => r.ExistsAsync(otherUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        conversationRepo.Setup(r => r.GetIdForDirectAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid?)null);

        conversationRepo.Setup(r => r.AddAsync(It.IsAny<Conversation>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        participantRepo.Setup(r => r.AddAsync(It.IsAny<ConversationParticipant>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        uow.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new GetOrCreateDirectConversationHandler(
            conversationRepo.Object,
            uow.Object,
            participantRepo.Object,
            userRepo.Object);

        var result = await handler.Handle(
            new GetOrCreateDirectConversationCommand(requestingUserId, otherUserId),
            CancellationToken.None);

        result.Should().NotBe(Guid.Empty);

        conversationRepo.Verify(r => r.AddAsync(
            It.Is<Conversation>(c =>
                c.Id == result &&
                c.Type == ChatType.Direct &&
                c.DirectKey != null),
            It.IsAny<CancellationToken>()), Times.Once);

        participantRepo.Verify(r => r.AddAsync(
            It.Is<ConversationParticipant>(cp =>
                cp.ConversationId == result &&
                cp.UserId == requestingUserId),
            It.IsAny<CancellationToken>()), Times.Once);

        participantRepo.Verify(r => r.AddAsync(
            It.Is<ConversationParticipant>(cp =>
                cp.ConversationId == result &&
                cp.UserId == otherUserId),
            It.IsAny<CancellationToken>()), Times.Once);

        uow.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_Return_Existing_Id_After_Race_Condition()
    {
        var requestingUserId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var existingConversationId = Guid.NewGuid();

        var conversationRepo = new Mock<IConversationRepository>();
        var participantRepo = new Mock<IConversationParticipantRepository>();
        var userRepo = new Mock<IUserRepository>();
        var uow = new Mock<IUnitOfWork>();

        userRepo.Setup(r => r.ExistsAsync(otherUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        conversationRepo.SetupSequence(r => r.GetIdForDirectAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid?)null)
            .ReturnsAsync(existingConversationId);

        conversationRepo.Setup(r => r.AddAsync(It.IsAny<Conversation>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        participantRepo.Setup(r => r.AddAsync(It.IsAny<ConversationParticipant>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        uow.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UniqueConstraintViolationException("Unique constraint violated."));

        var handler = new GetOrCreateDirectConversationHandler(
            conversationRepo.Object,
            uow.Object,
            participantRepo.Object,
            userRepo.Object);

        var result = await handler.Handle(
            new GetOrCreateDirectConversationCommand(requestingUserId, otherUserId),
            CancellationToken.None);

        result.Should().Be(existingConversationId);
    }
}