namespace JamSpace.Application.Common.Interfaces;

public interface ICountryCodeResolver
{
    string? ResolveCountryCode(string input);
}