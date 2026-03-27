using FluentAssertions;
using Moq;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.Conversations.Queries.GetConversationDetails;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;

namespace JamSpace.Tests.Unit.Conversations;

public class GetConversationDetailsHandlerTests
{
    [Fact]
    public async Task Should_Throw_NotFound_When_Conversation_Does_Not_Exist()
    {
        var conversationRepo = new Mock<IConversationRepository>();

        conversationRepo
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Conversation?)null);

        var handler = new GetConversationDetailsHandler(conversationRepo.Object);

        var act = async () => await handler.Handle(
            new GetConversationDetailsQuery(Guid.NewGuid(), Guid.NewGuid()),
            CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Should_Throw_Forbidden_When_User_Is_Not_Participant()
    {
        var conversationRepo = new Mock<IConversationRepository>();

        var conversationId = Guid.NewGuid();
        var requesterId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();

        var conversation = new Conversation
        {
            Id = conversationId,
            Type = ChatType.Direct,
            Participants = new List<ConversationParticipant>
            {
                new()
                {
                    UserId = otherUserId,
                    User = new User
                    {
                        Id = otherUserId,
                        DisplayName = "Other user",
                        UserName = "Other user"
                    },
                    ConversationId = conversationId
                }
            }
        };

        conversationRepo
            .Setup(x => x.GetByIdAsync(conversationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);

        var handler = new GetConversationDetailsHandler(conversationRepo.Object);

        var act = async () => await handler.Handle(
            new GetConversationDetailsQuery(conversationId, requesterId),
            CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }

    [Fact]
    public async Task Should_Throw_InvalidOperation_When_Direct_Conversation_Does_Not_Have_Exactly_Two_Participants()
    {
        var conversationRepo = new Mock<IConversationRepository>();

        var conversationId = Guid.NewGuid();
        var requesterId = Guid.NewGuid();

        var conversation = new Conversation
        {
            Id = conversationId,
            Type = ChatType.Direct,
            Participants = new List<ConversationParticipant>
            {
                new()
                {
                    UserId = requesterId,
                    User = new User
                    {
                        Id = requesterId,
                        DisplayName = "Requester",
                        UserName = "Requester"
                    },
                    ConversationId = conversationId
                }
            }
        };

        conversationRepo
            .Setup(x => x.GetByIdAsync(conversationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);

        var handler = new GetConversationDetailsHandler(conversationRepo.Object);

        var act = async () => await handler.Handle(
            new GetConversationDetailsQuery(conversationId, requesterId),
            CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Should_Return_Details_For_Direct_Conversation()
    {
        var conversationRepo = new Mock<IConversationRepository>();

        var conversationId = Guid.NewGuid();
        var requesterId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();

        var conversation = new Conversation
        {
            Id = conversationId,
            Type = ChatType.Direct,
            Participants = new List<ConversationParticipant>
            {
                new()
                {
                    UserId = requesterId,
                    LastReadMessageId = Guid.NewGuid(),
                    LastReadAt = DateTimeOffset.UtcNow.AddMinutes(-5),
                    User = new User
                    {
                        Id = requesterId,
                        DisplayName = "Me",
                        UserName = "Me",
                        ProfilePictureUrl = "me.png"
                    },
                    ConversationId = conversationId
                },
                new()
                {
                    UserId = otherUserId,
                    LastReadMessageId = Guid.NewGuid(),
                    LastReadAt = DateTimeOffset.UtcNow.AddMinutes(-2),
                    User = new User
                    {
                        Id = otherUserId,
                        DisplayName = "Other user",
                        UserName = "Other user",
                        ProfilePictureUrl = "other.png"
                    },
                    ConversationId = conversationId
                }
            }
        };

        conversationRepo
            .Setup(x => x.GetByIdAsync(conversationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);

        var handler = new GetConversationDetailsHandler(conversationRepo.Object);

        var result = await handler.Handle(
            new GetConversationDetailsQuery(conversationId, requesterId),
            CancellationToken.None);

        result.Id.Should().Be(conversationId);
        result.Type.Should().Be(ChatType.Direct);
        result.TargetUserId.Should().Be(otherUserId);
        result.DisplayName.Should().Be("Other user");
        result.DisplayPictureUrl.Should().Be("other.png");
        result.Participants.Should().HaveCount(2);
    }

    [Fact]
    public async Task Should_Return_Details_For_Team_Conversation()
    {
        var conversationRepo = new Mock<IConversationRepository>();

        var conversationId = Guid.NewGuid();
        var requesterId = Guid.NewGuid();
        var teammateId = Guid.NewGuid();

        var conversation = new Conversation
        {
            Id = conversationId,
            Type = ChatType.Team,
            TeamId = Guid.NewGuid(),
            Team = new Team
            {
                Name = "JamSpace Team",
                TeamPictureUrl = "team.png"
            },
            Participants = new List<ConversationParticipant>
            {
                new()
                {
                    UserId = requesterId,
                    User = new User
                    {
                        Id = requesterId,
                        DisplayName = "Me",
                        UserName = "Me"
                    },
                    ConversationId = conversationId
                },
                new()
                {
                    UserId = teammateId,
                    User = new User
                    {
                        Id = teammateId,
                        DisplayName = "Teammate",
                        UserName = "Teammate"
                    },
                    ConversationId = conversationId
                }
            }
        };

        conversationRepo
            .Setup(x => x.GetByIdAsync(conversationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);

        var handler = new GetConversationDetailsHandler(conversationRepo.Object);

        var result = await handler.Handle(
            new GetConversationDetailsQuery(conversationId, requesterId),
            CancellationToken.None);

        result.Id.Should().Be(conversationId);
        result.Type.Should().Be(ChatType.Team);
        result.TargetUserId.Should().BeNull();
        result.DisplayName.Should().Be("JamSpace Team");
        result.DisplayPictureUrl.Should().Be("team.png");
        result.TeamId.Should().Be(conversation.TeamId);
        result.Participants.Should().HaveCount(2);
    }
}