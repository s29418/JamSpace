namespace JamSpace.Application.Common.Models;

public sealed class FileUpload
{
    public required Stream Content { get; init; }
    public required string FileName { get; init; }
    public string? ContentType { get; init; }
    public long Length { get; init; }
}