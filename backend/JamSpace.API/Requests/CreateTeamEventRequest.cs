namespace JamSpace.API.Requests;

public class CreateTeamEventRequest
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public DateTimeOffset StartDateTime { get; set; }
    public int DurationMinutes { get; set; }
}