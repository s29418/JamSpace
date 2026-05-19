namespace JamSpace.API.Requests;

public class UploadProjectAudioVersionRequest
{
    public string Name { get; set; } = "";
    public int? DurationSeconds { get; set; }
    public IFormFile? File { get; set; }
}
