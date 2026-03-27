using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.Conversations.Helpers;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.Conversations.Commands.GetOrCreateDirectConversation;

public class GetOrCreateDirectConversationHandler : IRequestHandler<GetOrCreateDirectConversationCommand, Guid>
{
    private readonly IConversationRepository _conversation;
    private readonly IConversationParticipantRepository _conversationParticipant;
    private readonly IUserRepository _user;
    private readonly IUnitOfWork _uow;
    
    public GetOrCreateDirectConversationHandler(IConversationRepository conversation, IUnitOfWork uow, 
        IConversationParticipantRepository conversationParticipant, IUserRepository user)
    {
        _conversation = conversation;
        _conversationParticipant = conversationParticipant;
        _user = user;
        _uow = uow;
    }

    public async Task<Guid> Handle(GetOrCreateDirectConversationCommand request, CancellationToken ct)
    {
        var exists = await _user.ExistsAsync(request.OtherUserId, ct);
        if (!exists)
            throw new NotFoundException($"User with ID: {request.OtherUserId} not found.");
        
        var directKey = DirectKeyBuilder.Build(request.RequestingUserId, request.OtherUserId);
        
        var existingId = await _conversation.GetIdForDirectAsync(directKey, ct);

        if (existingId is not null)
            return existingId.Value;
        
        
        var conversationId = Guid.NewGuid();

        var conversation = new Conversation
        {
            Id = conversationId,
            Type = ChatType.Direct,
            DirectKey = directKey
        };

        await _conversation.AddAsync(conversation, ct);

        await _conversationParticipant.AddAsync(new ConversationParticipant
        {
            ConversationId = conversationId,
            UserId = request.RequestingUserId
        }, ct);
        
        await _conversationParticipant.AddAsync(new ConversationParticipant
        {
            ConversationId = conversationId,
            UserId = request.OtherUserId
        }, ct);

        try
        {
            await _uow.SaveChangesAsync(ct);
            return conversationId;
        }
        catch (UniqueConstraintViolationException)
        {
            var idAfterRace = await _conversation.GetIdForDirectAsync(directKey, ct);
            if (idAfterRace is null)
                throw;

            return idAfterRace.Value;
        }
    }
}