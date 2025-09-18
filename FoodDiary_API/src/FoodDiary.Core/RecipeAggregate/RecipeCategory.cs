namespace FoodDiary.Core.RecipeAggregate;

public enum RecipeCategory
{
    Breakfast,
    Lunch,
    Dinner,
    Snack,
    Dessert,
    Appetizer,
    Soup,
    Salad,
    MainCourse,
    SideDish,
    Beverage,
    Smoothie,
    Bread,
    Pasta,
    Rice,
    Meat,
    Fish,
    Vegetarian,
    Vegan,
    GlutenFree,
    LowCarb,
    HighProtein,
    QuickMeal,
    SlowCooker,
    Other
}

public static class RecipeCategoryExtensions
{
    public static string GetDisplayName(this RecipeCategory category)
    {
        return category switch
        {
            RecipeCategory.Breakfast => "Breakfast",
            RecipeCategory.Lunch => "Lunch",
            RecipeCategory.Dinner => "Dinner",
            RecipeCategory.Snack => "Snack",
            RecipeCategory.Dessert => "Dessert",
            RecipeCategory.Appetizer => "Appetizer",
            RecipeCategory.Soup => "Soup",
            RecipeCategory.Salad => "Salad",
            RecipeCategory.MainCourse => "Main Course",
            RecipeCategory.SideDish => "Side Dish",
            RecipeCategory.Beverage => "Beverage",
            RecipeCategory.Smoothie => "Smoothie",
            RecipeCategory.Bread => "Bread",
            RecipeCategory.Pasta => "Pasta",
            RecipeCategory.Rice => "Rice",
            RecipeCategory.Meat => "Meat",
            RecipeCategory.Fish => "Fish",
            RecipeCategory.Vegetarian => "Vegetarian",
            RecipeCategory.Vegan => "Vegan",
            RecipeCategory.GlutenFree => "Gluten Free",
            RecipeCategory.LowCarb => "Low Carb",
            RecipeCategory.HighProtein => "High Protein",
            RecipeCategory.QuickMeal => "Quick Meal",
            RecipeCategory.SlowCooker => "Slow Cooker",
            RecipeCategory.Other => "Other",
            _ => category.ToString()
        };
    }
} 