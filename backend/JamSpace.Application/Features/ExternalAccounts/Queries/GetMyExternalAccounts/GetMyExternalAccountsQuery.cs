using JamSpace.Application.Features.ExternalAccounts.DTOs;
using MediatR;

namespace JamSpace.Application.Features.ExternalAccounts.Queries.GetMyExternalAccounts;

public sealed record GetMyExternalAccountsQuery(Guid UserId) : IRequest<IReadOnlyList<UserExternalAccountDto>>;
