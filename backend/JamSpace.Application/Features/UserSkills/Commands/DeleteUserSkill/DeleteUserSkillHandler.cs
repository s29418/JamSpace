using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using MediatR;

namespace JamSpace.Application.Features.UserSkills.Commands.DeleteUserSkill;

public class DeleteUserSkillHandler : IRequestHandler<DeleteUserSkillCommand, Unit>
{
    private readonly IUserSkillRepository _repo;
    private readonly IUnitOfWork _uow;

    public DeleteUserSkillHandler(IUserSkillRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Unit> Handle(DeleteUserSkillCommand request, CancellationToken ct)
    {
        var userSkill = await _repo.GetUserSkillAsync(request.UserId, request.SkillId, ct);

        if (userSkill is null)
            throw new ConflictException("User does not have this skill.");

        _repo.Remove(userSkill);
        await _uow.SaveChangesAsync(ct);

        return Unit.Value;
    }
}