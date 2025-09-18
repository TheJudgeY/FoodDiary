using FoodDiary.Core.UserAggregate;

namespace FoodDiary.UseCases.Users;

public interface ITimezoneService
{
    DateTime GetLocalTime(User user, DateTime utcTime);
    DateTime GetUtcTime(User user, DateTime localTime);
}

public class TimezoneService : ITimezoneService
{
    public DateTime GetLocalTime(User user, DateTime utcTime)
    {
        try
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(user.TimeZoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, timeZone);
        }
        catch (TimeZoneNotFoundException)
        {
            return utcTime;
        }
    }

    public DateTime GetUtcTime(User user, DateTime localTime)
    {
        try
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(user.TimeZoneId);
            return TimeZoneInfo.ConvertTimeToUtc(localTime, timeZone);
        }
        catch (TimeZoneNotFoundException)
        {
            return localTime;
        }
    }
} 