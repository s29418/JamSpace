using MediatR;

namespace JamSpace.Application.Features.Authentication.Queries.VerifyPassword;

public record VerifyPasswordQuery(
    Guid Id,
    string Password) 
    : IRequest<Unit>;