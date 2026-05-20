namespace JamSpace.API.Requests;

public class CreateProjectNoteRequest
{
    public string Content { get; set; } = "";
    public Guid? AudioVersionId { get; set; }
    public int? StartTimeSeconds { get; set; }
    public int? EndTimeSeconds { get; set; }
}
