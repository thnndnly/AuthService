using ChatCommand.Models;

namespace ChatCommand.Data;

public class RefreshTokenRepository
{
    private readonly AppDbContext _dbContext;

    public RefreshTokenRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void SaveRefreshToken(string username, string token, DateTime expiration, string ipAddress)
    {
        var refreshToken = new RefreshToken
        {
            Username = username,
            Token = token,
            Expiration = expiration,
            IpAddress = ipAddress
        };

        _dbContext.RefreshTokens.Add(refreshToken);
        _dbContext.SaveChanges();
    }

    public RefreshToken? GetRefreshToken(string token)
    {
        return _dbContext.RefreshTokens.FirstOrDefault(rt => rt.Token == token);
    }

    public void RemoveRefreshToken(string token)
    {
        var refreshToken = _dbContext.RefreshTokens.FirstOrDefault(rt => rt.Token == token);
        if (refreshToken != null)
        {
            _dbContext.RefreshTokens.Remove(refreshToken);
            _dbContext.SaveChanges();
        }
    }
}

