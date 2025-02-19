using System.Text;
using ChatCommand.Data;
using ChatCommand.Helpers;
using ChatCommand.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ChatCommand;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        // 🔹 Zuerst DbContext registrieren (MUSS vor den Repositories sein!)
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite("Data Source=app.db"));
        
        // 🔹 Repositories (damit sie in Services verwendet werden können)
        builder.Services.AddScoped<UserRepository>();
        builder.Services.AddScoped<RefreshTokenRepository>();
        
        // 🔹 Services (AuthService benötigt UserRepository & RefreshTokenRepository)
        builder.Services.AddScoped<IAuthService, AuthService>(); // AuthService registrieren
        
        // 🔹 JwtService als Scoped Service registrieren
        var jwtSecret = builder.Configuration["Jwt:Secret"];
        var jwtExpiration = builder.Configuration["Jwt:ExpirationMinutes"];
        
        builder.Services.AddScoped<JwtService>(sp =>
        {
            if (string.IsNullOrEmpty(jwtSecret))
            {
                throw new Exception("Jwt:Secret not found in configuration!");
            }
            if (!int.TryParse(jwtExpiration, out var expMinutes))
            {
                throw new Exception("Jwt:ExpirationMinutes is invalid or not set!");
            }

            // Hier erstellen wir das JwtService-Objekt manuell:
            return new JwtService(jwtSecret, expMinutes);
        });
        
        // 🔹 Authentifizierung & Autorisierung
        builder.Services.AddAuthentication();
        builder.Services.AddAuthorization();
        
        var key = Encoding.UTF8.GetBytes(jwtSecret);

        builder.Services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }; 
            });

        builder.Services.AddAuthorization();

        var app = builder.Build();
        
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate(); // Erstellt die DB, falls sie nicht existiert
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.UseSwagger();
        app.UseSwaggerUI();

        app.Run();

    }
}