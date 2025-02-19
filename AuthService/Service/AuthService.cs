using System.Security.Cryptography;
using ChatCommand.Controllers;
using ChatCommand.Data;
using ChatCommand.Helpers;

namespace ChatCommand.Service;

public class AuthService : IAuthService
{
    private readonly UserRepository _userRepository;
    private readonly RefreshTokenRepository _refreshTokenRepository;
    private readonly JwtService _jwtService;

    public AuthService(UserRepository userRepository, RefreshTokenRepository refreshTokenRepository, JwtService jwtService)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtService = jwtService;
    }

    public async Task<(string? AccessToken, string? RefreshToken)> LoginAsync(LoginRequest request, string ipAddress)
    {
        var user = _userRepository.GetUser(request.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return (null, null);

        var accessToken = _jwtService.GenerateToken(user);
        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        _refreshTokenRepository.SaveRefreshToken(user.Username, refreshToken, DateTime.UtcNow.AddDays(7), ipAddress);

        return (accessToken, refreshToken);
    }

    public async Task<string> RegisterAsync(RegisterRequest request)
    {
        if (_userRepository.GetUser(request.Username) != null)
            return "Username already exists.";
        if (IsValidPassword(request.Password))
        {
            _userRepository.CreateUser(request.Username, request.Password, request.Role);
            return "User registered successfully.";
        }
        return "Password is not valid.";
    }

    public async Task<(string? AccessToken, string? RefreshToken)?> RefreshTokenAsync(RefreshRequest request, string ipAddress)
    {
        var existingToken = _refreshTokenRepository.GetRefreshToken(request.RefreshToken);
        if (existingToken == null || existingToken.Expiration < DateTime.UtcNow)
            return null;

        if (existingToken.IpAddress != ipAddress)
            return null;

        var user = _userRepository.GetUser(existingToken.Username);
        if (user == null) return null;

        // Neues Access Token
        var newAccessToken = _jwtService.GenerateToken(user);

        // OPTIONAL: Neues Refresh Token generieren
        var newRefreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var newExpiration = DateTime.UtcNow.AddDays(7);
        _refreshTokenRepository.SaveRefreshToken(user.Username, newRefreshToken, newExpiration, ipAddress);

        // Altes Refresh Token entfernen
        _refreshTokenRepository.RemoveRefreshToken(request.RefreshToken);

        return (newAccessToken, newRefreshToken);
    }

    public async Task<bool> ChangePasswordAsync(ChangePasswordRequest request)
    {
        var user = _userRepository.GetUser(request.Username);
        if (user == null) return false;

        if (!BCrypt.Net.BCrypt.Verify(request.OldPassword, user.PasswordHash))
            return false;

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        _userRepository.UpdateUser(user);
        return true;
    }

    public async Task<bool> DeleteUserAsync(DeleteUserRequest request)
    {
        var user = _userRepository.GetUser(request.Username);
        if (user == null) return false;

        _userRepository.DeleteUser(request.Username);
        return true;
    }
    
    public static bool IsValidPassword(string password)
    {
        return password.Length >= 12 &&
               password.Any(char.IsUpper) &&
               password.Any(char.IsLower) &&
               password.Any(char.IsDigit) &&
               password.Any(ch => !char.IsLetterOrDigit(ch));
    }

}
