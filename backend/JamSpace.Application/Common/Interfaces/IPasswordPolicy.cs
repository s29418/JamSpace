namespace JamSpace.Application.Common.Interfaces;

public interface IPasswordPolicy
{
    (bool IsValid, string? Error) Validate(string passwordPlain);
}