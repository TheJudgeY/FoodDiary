namespace FoodDiary.Core.Interfaces;

public interface IEmailConfirmationTokenGenerator
{
    string GenerateToken();
    DateTime GetExpirationTime();
} 