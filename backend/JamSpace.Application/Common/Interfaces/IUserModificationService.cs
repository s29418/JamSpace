namespace JamSpace.Application.Common.Interfaces;

public interface IUserModificationService
{
    Task UpdateUserProfilePictureAsync(Guid userId, string pictureUrl, CancellationToken ct);
}