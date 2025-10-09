using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using MediatR;

namespace JamSpace.Application.Features.UserSkills.Commands.DeleteUserSkill;

public class DeleteUserSkillHandler : IRequestHandler<DeleteUserSkillCommand, Unit>
{
    private readonly IUserSkillRepository _repo;
    
    public DeleteUserSkillHandler(IUserSkillRepository repo)
    {
        _repo = repo;
    }
    
    public async Task<Unit> Handle(DeleteUserSkillCommand request, CancellationToken ct)
    {
        if (!await _repo.UserHasSkillAsync(request.UserId, request.SkillId, ct))
            throw new ConflictException("User does not have this skill.");
        
        await _repo.RemoveUserSkillAsync(request.UserId, request.SkillId, ct);
        return Unit.Value;
    }
}