using JamSpace.Application.Common.Models;

namespace JamSpace.Application.Common.Interfaces;

public interface IOAuthPkceGenerator
{
    OAuthPkceParameters Generate();
}
