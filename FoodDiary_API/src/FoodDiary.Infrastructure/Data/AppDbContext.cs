using Microsoft.EntityFrameworkCore;
using System.Reflection;
using FoodDiary.Core.UserAggregate;
using FoodDiary.Core.ProductAggregate;
using FoodDiary.Core.FoodEntryAggregate;

using FoodDiary.Core.NotificationAggregate;
using FoodDiary.Core.RecipeAggregate;

namespace FoodDiary.Infrastructure.Data;
public class AppDbContext : DbContext
{
    private readonly IDomainEventDispatcher? _dispatcher;

    public AppDbContext(DbContextOptions<AppDbContext> options, IDomainEventDispatcher? dispatcher = null)
        : base(options)
    {
        _dispatcher = dispatcher;
    }

  public DbSet<User> Users => Set<User>();
  public DbSet<Product> Products => Set<Product>();
          public DbSet<FoodEntry> FoodEntries => Set<FoodEntry>();

        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<NotificationPreferences> NotificationPreferences => Set<NotificationPreferences>();
        public DbSet<Recipe> Recipes => Set<Recipe>();
        public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();
        public DbSet<RecipeFavorite> RecipeFavorites => Set<RecipeFavorite>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }

  public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
  {
    int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

    if (_dispatcher == null) return result;

    var entitiesWithEvents = ChangeTracker.Entries<HasDomainEventsBase>()
        .Select(e => e.Entity)
        .Where(e => e.DomainEvents.Any())
        .ToArray();

    await _dispatcher.DispatchAndClearEvents(entitiesWithEvents);

    return result;
  }

  public override int SaveChanges() =>
        SaveChangesAsync().GetAwaiter().GetResult();
}
