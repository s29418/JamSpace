namespace JamSpace.Application.Common.Common.Exceptions;

public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException(string message) : base(message) { }
}