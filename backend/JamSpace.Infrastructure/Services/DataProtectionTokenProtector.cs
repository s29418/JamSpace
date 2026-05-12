using JamSpace.Application.Common.Interfaces;
using Microsoft.AspNetCore.DataProtection;

namespace JamSpace.Infrastructure.Services;

public class DataProtectionTokenProtector : ITokenProtector
{
    private const string Purpose = "JamSpace.ExternalMusicAccountTokens.v1";

    private readonly IDataProtector _protector;

    public DataProtectionTokenProtector(IDataProtectionProvider provider)
    {
        _protector = provider.CreateProtector(Purpose);
    }

    public string Protect(string token)
    {
        return _protector.Protect(token);
    }

    public string Unprotect(string protectedToken)
    {
        return _protector.Unprotect(protectedToken);
    }
}
