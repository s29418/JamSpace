using JamSpace.Application.Features.Conversations.DTOs;
using MediatR;

namespace JamSpace.Application.Features.Conversations.Queries.GetConversationDetails;

public record GetConversationDetailsQuery(Guid Id, Guid UserId) : IRequest<ConversationDetailsDto>;