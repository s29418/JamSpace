namespace JamSpace.Application.Common.Interfaces;

public interface ITokenProtector
{
    string Protect(string token);
    string Unprotect(string protectedToken);
}
