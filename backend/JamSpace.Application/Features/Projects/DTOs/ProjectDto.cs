namespace JamSpace.Application.Features.Projects.DTOs;

public class ProjectDto
{
    public Guid Id { get; set; }
    public Guid TeamId { get; set; }
    public string Name { get; set; } = null!;
    public string PictureUrl { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
}