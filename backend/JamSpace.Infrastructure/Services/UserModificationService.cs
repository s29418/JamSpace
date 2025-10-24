using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;

namespace JamSpace.Infrastructure.Services;

public class UserModificationService : IUserModificationService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public UserModificationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task UpdateUserProfilePictureAsync(Guid userId, string pictureUrl, CancellationToken ct)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId, ct)
                   ?? throw new NotFoundException("User not found.");

        user.ProfilePictureUrl = pictureUrl; 

        await _unitOfWork.SaveChangesAsync(ct); 
    }
}