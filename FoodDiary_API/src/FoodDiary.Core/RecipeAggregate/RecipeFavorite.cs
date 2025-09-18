using FoodDiary.Core.UserAggregate;

namespace FoodDiary.Core.RecipeAggregate;

public enum RecipeUserRelationshipType
{
    Creator = 0,
    Contributor = 1,
    Favorite = 2,
    Viewer = 3
}

public class RecipeFavorite
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid RecipeId { get; private set; }
    public RecipeUserRelationshipType RelationshipType { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    
    public User User { get; private set; } = null!;
    public Recipe Recipe { get; private set; } = null!;
    
    private RecipeFavorite() { }
    
    public RecipeFavorite(Guid userId, Guid recipeId, RecipeUserRelationshipType relationshipType = RecipeUserRelationshipType.Favorite)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        RecipeId = recipeId;
        RelationshipType = relationshipType;
        CreatedAt = DateTime.UtcNow;
    }
}
