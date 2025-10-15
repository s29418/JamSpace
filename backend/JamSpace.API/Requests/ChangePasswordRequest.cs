namespace JamSpace.API.Requests;

public record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword
    );