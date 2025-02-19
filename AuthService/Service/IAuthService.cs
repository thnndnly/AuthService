using ChatCommand.Controllers;

namespace ChatCommand.Service;

public interface IAuthService
{
    Task<(string? AccessToken, string? RefreshToken)> LoginAsync(LoginRequest request, string ipAddress);
    Task<string> RegisterAsync(RegisterRequest request);
    Task<(string? AccessToken, string? RefreshToken)?> RefreshTokenAsync(RefreshRequest request, string ipAddress);
    Task<bool> ChangePasswordAsync(ChangePasswordRequest request);
    Task<bool> DeleteUserAsync(DeleteUserRequest request);
}
