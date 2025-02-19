using ChatCommand.Models;

namespace ChatCommand.Data;

using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RefreshToken>().HasKey(rt => rt.Token);
        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique(); // Username darf nur einmal vorkommen
    }
}