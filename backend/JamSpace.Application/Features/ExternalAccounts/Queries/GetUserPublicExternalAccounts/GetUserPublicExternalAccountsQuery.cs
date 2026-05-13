using JamSpace.Application.Features.ExternalAccounts.DTOs;
using MediatR;

namespace JamSpace.Application.Features.ExternalAccounts.Queries.GetUserPublicExternalAccounts;

public sealed record GetUserPublicExternalAccountsQuery(Guid UserId)
    : IRequest<IReadOnlyList<PublicUserExternalAccountDto>>;
