using FoodDiary.Core.FoodEntryAggregate;
using FoodDiary.UseCases.Analytics;
using FoodDiary.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FoodDiary.Infrastructure.Services;

public class NutritionalDataService : INutritionalDataService
{
    private readonly AppDbContext _context;

    public NutritionalDataService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<FoodEntry>> GetFoodEntriesForDateAsync(Guid userId, DateTime date)
    {
        var startOfDay = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
        var endOfDay = startOfDay.AddDays(1);

        return await _context.FoodEntries
            .Include(fe => fe.Product)
            .Where(fe => fe.UserId == userId && 
                        fe.ConsumedAt >= startOfDay && 
                        fe.ConsumedAt < endOfDay)
            .ToListAsync();
    }

    public async Task<List<FoodEntry>> GetFoodEntriesForPeriodAsync(Guid userId, DateTime startDate, DateTime endDate)
    {
        return await _context.FoodEntries
            .Include(fe => fe.Product)
            .Where(fe => fe.UserId == userId && 
                        fe.ConsumedAt >= startDate && 
                        fe.ConsumedAt <= endDate)
            .ToListAsync();
    }
}
