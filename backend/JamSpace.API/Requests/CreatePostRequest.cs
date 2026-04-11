namespace JamSpace.API.Requests;

public class CreatePostRequest
{
    public string? Content { get; set; }
    public IFormFile? File { get; set; }
}