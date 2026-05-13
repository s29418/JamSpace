using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using MediatR;

namespace JamSpace.Application.Features.ExternalAccounts.Commands.Disconnect;

public class DisconnectExternalAccountHandler : IRequestHandler<DisconnectExternalAccountCommand>
{
    private readonly IUserExternalAccountRepository _externalAccounts;
    private readonly IUnitOfWork _uow;

    public DisconnectExternalAccountHandler(
        IUserExternalAccountRepository externalAccounts,
        IUnitOfWork uow)
    {
        _externalAccounts = externalAccounts;
        _uow = uow;
    }

    public async Task Handle(DisconnectExternalAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await _externalAccounts.GetActiveByUserAndProviderAsync(
            request.UserId,
            request.Provider,
            cancellationToken);

        if (account is null)
            throw new NotFoundException($"{request.Provider} account is not connected.");

        _externalAccounts.Disconnect(account, DateTimeOffset.UtcNow);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}
