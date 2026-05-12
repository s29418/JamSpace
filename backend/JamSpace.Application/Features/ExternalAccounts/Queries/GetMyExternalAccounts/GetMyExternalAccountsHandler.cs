using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.ExternalAccounts.DTOs;
using MediatR;

namespace JamSpace.Application.Features.ExternalAccounts.Queries.GetMyExternalAccounts;

public class GetMyExternalAccountsHandler
    : IRequestHandler<GetMyExternalAccountsQuery, IReadOnlyList<UserExternalAccountDto>>
{
    private readonly IUserExternalAccountRepository _externalAccounts;

    public GetMyExternalAccountsHandler(IUserExternalAccountRepository externalAccounts)
    {
        _externalAccounts = externalAccounts;
    }

    public async Task<IReadOnlyList<UserExternalAccountDto>> Handle(
        GetMyExternalAccountsQuery request,
        CancellationToken cancellationToken)
    {
        var accounts = await _externalAccounts.GetActiveByUserIdAsync(request.UserId, cancellationToken);

        return accounts
            .Select(x => new UserExternalAccountDto(
                x.Id,
                x.Provider.ToString(),
                x.ExternalUserId,
                x.DisplayName,
                x.ProfileUrl,
                x.AvatarUrl,
                x.ConnectedAt,
                x.UpdatedAt))
            .ToList();
    }
}
