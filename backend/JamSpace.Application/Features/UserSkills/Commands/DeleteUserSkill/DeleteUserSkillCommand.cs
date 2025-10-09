using MediatR;

namespace JamSpace.Application.Features.UserSkills.Commands.DeleteUserSkill;

public record DeleteUserSkillCommand(Guid UserId, Guid SkillId) : IRequest<Unit>;