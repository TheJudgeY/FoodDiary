namespace FoodDiary.Core.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(string userId, string email, string name);
} 