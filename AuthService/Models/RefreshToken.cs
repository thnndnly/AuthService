namespace ChatCommand.Models;

using System.ComponentModel.DataAnnotations;

public class RefreshToken
{
    [Key]
    public string Token { get; set; } = string.Empty;

    [Required]
    public string Username { get; set; } = string.Empty;

    public DateTime Expiration { get; set; }
    
    [Required]
    public string IpAddress { get; set; } = string.Empty;
}

