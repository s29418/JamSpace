namespace JamSpace.Application.Features.Conversations.Helpers;

public static class DirectKeyBuilder
{
    public static string Build(Guid userId, Guid otherUserId)
    {
        var ordered = new[] { userId, otherUserId }
            .OrderBy(x => x)
            .ToArray();

        return $"{ordered[0]:N}_{ordered[1]:N}";
    }
}