namespace JamSpace.Application.Features.Conversations.Helpers;

public static class MessagePreviewBuilder
{
    public static string Build(string content) => content.Length <= 120 ? content : content[..120];
}