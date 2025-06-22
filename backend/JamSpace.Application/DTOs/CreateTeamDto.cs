namespace DefaultNamespace;

public class CreateTeamDto
{
    public string Name { get; set; }
    public string TeamPictureUrl { get; set; }
    public Guid CreatorUserId { get; set; }
}