using JamSpace.Application.Features.UserSkills.DTOs;
using MediatR;

namespace JamSpace.Application.Features.UserSkills.Queries.GetUserSkills;

public record GetUserSkillsQuery(Guid UserId) : IRequest<List<UserSkillDto>>;