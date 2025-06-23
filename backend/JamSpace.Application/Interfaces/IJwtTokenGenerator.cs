using DefaultNamespace;

namespace JamSpace.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
