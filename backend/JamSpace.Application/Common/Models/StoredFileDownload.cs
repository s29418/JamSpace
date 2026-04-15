namespace JamSpace.Application.Common.Models;

public sealed class StoredFileDownload
{
    public required Stream Content { get; init; }
    public required string ContentType { get; init; }
    public string? FileName { get; init; }
}