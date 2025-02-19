using ChatCommand.Models;

namespace ChatCommand.Data;

public class UserRepository
{
    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public User? GetUser(string username)
    {
        return _dbContext.Users.FirstOrDefault(u => u.Username == username);
    }

    public List<User> GetAllUsers()
    {
        return _dbContext.Users.ToList();
    }

    public void CreateUser(string username, string password, string role)
    {
        var user = new User
        {
            Username = username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = role
        };

        _dbContext.Users.Add(user);
        _dbContext.SaveChanges();
    }
    
    public void UpdateUser(User user)
    {
        _dbContext.Users.Update(user);
        _dbContext.SaveChanges();
    }

    public void DeleteUser(string username)
    {
        var user = _dbContext.Users.FirstOrDefault(u => u.Username == username);
        if (user != null)
        {
            _dbContext.Users.Remove(user);
            _dbContext.SaveChanges();
        }
    }
}
