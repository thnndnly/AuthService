using ChatCommand.Data;

namespace ChatCommand.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/admin")]
[ApiController]
[Authorize(Roles = "Admin")] // Nur Admins dürfen auf diesen Controller zugreifen
public class AdminController : ControllerBase
{
    private readonly UserRepository _userRepository;

    public AdminController(AppDbContext dbContext)
    {
        _userRepository = new UserRepository(dbContext);
    }

    /// <summary>
    /// Gibt eine Liste aller Benutzer zurück (nur für Admins).
    /// </summary>
    [HttpGet("users")]
    public IActionResult GetAllUsers()
    {
        var users = _userRepository.GetAllUsers();
        return Ok(users);
    }

    /// <summary>
    /// Ändert die Rolle eines Nutzers (nur für Admins).
    /// </summary>
    [HttpPost("update-role")]
    public IActionResult UpdateUserRole([FromBody] UpdateRoleRequest request)
    {
        var user = _userRepository.GetUser(request.Username);
        if (user == null)
            return NotFound("User not found.");

        user.Role = request.NewRole;
        _userRepository.UpdateUser(user);

        return Ok($"User {user.Username} is now a {user.Role}.");
    }

    /// <summary>
    /// Löscht einen Benutzer aus der Datenbank (nur für Admins).
    /// </summary>
    [HttpPost("delete-user")]
    public IActionResult DeleteUser([FromBody] DeleteUserRequest request)
    {
        var user = _userRepository.GetUser(request.Username);
        if (user == null)
            return NotFound("User not found.");

        _userRepository.DeleteUser(request.Username);
        return Ok($"User {request.Username} has been deleted.");
    }
}

/// <summary>
/// Datenmodell für die Änderung der Benutzerrolle.
/// </summary>
public class UpdateRoleRequest
{
    public string Username { get; set; } = string.Empty;
    public string NewRole { get; set; } = string.Empty;
}

/// <summary>
/// Datenmodell für das Löschen eines Benutzers.
/// </summary>
public partial class DeleteUserRequest { }
