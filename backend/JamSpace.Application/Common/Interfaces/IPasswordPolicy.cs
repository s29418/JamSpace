namespace JamSpace.Application.Common.Interfaces;

public interface IPasswordPolicy
{
    (bool IsValid, Dictionary<string, string> Errors) Validate(string passwordPlain);
}