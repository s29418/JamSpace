namespace JamSpace.API.Requests;

public class EditTeamEventRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTimeOffset? StartDateTime { get; set; }
    public int? DurationMinutes { get; set; }
}