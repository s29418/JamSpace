using JamSpace.Domain.Entities;

namespace JamSpace.Application.Common.Interfaces;

public interface IExternalAccountTokenService
{
    Task<string> GetValidAccessTokenAsync(UserExternalAccount account, CancellationToken ct);
}
