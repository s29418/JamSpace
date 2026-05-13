using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.ExternalAccounts.DTOs;
using MediatR;

namespace JamSpace.Application.Features.ExternalAccounts.Queries.GetUserPublicExternalAccounts;

public class GetUserPublicExternalAccountsHandler
    : IRequestHandler<GetUserPublicExternalAccountsQuery, IReadOnlyList<PublicUserExternalAccountDto>>
{
    private readonly IUserRepository _users;
    private readonly IUserExternalAccountRepository _externalAccounts;

    public GetUserPublicExternalAccountsHandler(
        IUserRepository users,
        IUserExternalAccountRepository externalAccounts)
    {
        _users = users;
        _externalAccounts = externalAccounts;
    }

    public async Task<IReadOnlyList<PublicUserExternalAccountDto>> Handle(
        GetUserPublicExternalAccountsQuery request,
        CancellationToken cancellationToken)
    {
        if (!await _users.ExistsAsync(request.UserId, cancellationToken))
            throw new NotFoundException("User not found.");

        var accounts = await _externalAccounts.GetActiveByUserIdAsync(request.UserId, cancellationToken);

        return accounts
            .Select(x => new PublicUserExternalAccountDto(
                x.Provider.ToString(),
                x.DisplayName,
                x.ProfileUrl,
                x.AvatarUrl))
            .ToList();
    }
}
