using ChatCommand.Service;
using Microsoft.AspNetCore.Authorization;

namespace ChatCommand.Controllers;

using Microsoft.AspNetCore.Mvc;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        var (accessToken, refreshToken) = await _authService.LoginAsync(request, ipAddress);

        if (accessToken == null || refreshToken == null)
            return Unauthorized("Invalid credentials");

        return Ok(new 
        { 
            AccessToken = accessToken, 
            RefreshToken = refreshToken 
        });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshRequest request)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        var tokens = await _authService.RefreshTokenAsync(request, ipAddress);

        if (tokens == null || tokens.Value.AccessToken == null || tokens.Value.RefreshToken == null)
            return Unauthorized("Invalid or expired refresh token");

        return Ok(new 
        { 
            AccessToken = tokens.Value.AccessToken, 
            RefreshToken = tokens.Value.RefreshToken 
        });
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var success = await _authService.ChangePasswordAsync(request);
        return success ? Ok("Password updated successfully.") : Unauthorized("Invalid credentials.");
    }

    [Authorize]
    [HttpPost("delete-user")]
    public async Task<IActionResult> DeleteUser([FromBody] DeleteUserRequest request)
    {
        var success = await _authService.DeleteUserAsync(request);
        return success ? Ok("User deleted successfully.") : NotFound("User not found.");
    }
}


public class BaseUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginRequest : BaseUserRequest { }

public class RefreshRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}

public class RegisterRequest : BaseUserRequest
{ 
    public string Role { get; set; } = "Nutzer";
}

public class ChangePasswordRequest
{
    public string Username { get; set; } = string.Empty;
    public string OldPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

public partial class DeleteUserRequest : BaseUserRequest { }