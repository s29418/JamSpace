using JamSpace.Application.Features.UserSkills.DTOs;
using MediatR;

namespace JamSpace.Application.Features.UserSkills.Commands.AddUserSkill;

public record AddUserSkillCommand(Guid UserId, string SkillName) : IRequest<UserSkillDto>;