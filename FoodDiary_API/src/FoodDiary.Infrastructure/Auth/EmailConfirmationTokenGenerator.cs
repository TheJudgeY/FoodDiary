using FoodDiary.Core.Interfaces;
using System.Security.Cryptography;

namespace FoodDiary.Infrastructure.Auth;

public class EmailConfirmationTokenGenerator : IEmailConfirmationTokenGenerator
{
    private const int TokenLength = 32;
    private const int ExpirationHours = 24;

    public string GenerateToken()
    {
        var randomBytes = new byte[TokenLength];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        return Convert.ToBase64String(randomBytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
    }

    public DateTime GetExpirationTime()
    {
        return DateTime.UtcNow.AddHours(ExpirationHours);
    }
} 