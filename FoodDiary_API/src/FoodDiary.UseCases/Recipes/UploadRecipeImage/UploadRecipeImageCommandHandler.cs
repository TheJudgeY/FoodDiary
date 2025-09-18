using MediatR;
using Ardalis.Result;
using FoodDiary.Core.RecipeAggregate;
using FoodDiary.Core.Interfaces;

namespace FoodDiary.UseCases.Recipes;

public class UploadRecipeImageCommandHandler : IRequestHandler<UploadRecipeImageCommand, Result<UploadRecipeImageResponse>>
{
    private readonly IImageStorageService _imageStorageService;
    private readonly FoodDiary.Core.Interfaces.IRepository<Recipe> _recipeRepository;
    private readonly FoodDiary.Core.Interfaces.IRepository<RecipeFavorite> _recipeFavoriteRepository;
    private readonly IRecipeService _recipeService;

    public UploadRecipeImageCommandHandler(
        IImageStorageService imageStorageService,
        FoodDiary.Core.Interfaces.IRepository<Recipe> recipeRepository,
        FoodDiary.Core.Interfaces.IRepository<RecipeFavorite> recipeFavoriteRepository,
        IRecipeService recipeService)
    {
        _imageStorageService = imageStorageService;
        _recipeRepository = recipeRepository;
        _recipeFavoriteRepository = recipeFavoriteRepository;
        _recipeService = recipeService;
    }

    public async Task<Result<UploadRecipeImageResponse>> Handle(UploadRecipeImageCommand request, CancellationToken cancellationToken)
    {
        var recipeResult = await _recipeRepository.GetByIdAsync(request.RecipeId);
        if (!recipeResult.IsSuccess || recipeResult.Value == null)
        {
            return Result<UploadRecipeImageResponse>.NotFound($"Recipe with ID {request.RecipeId} not found");
        }

        var recipe = recipeResult.Value;

        var userRelationshipsResult = await _recipeFavoriteRepository.ListAsync();
        if (!userRelationshipsResult.IsSuccess)
        {
            return Result<UploadRecipeImageResponse>.Error($"Failed to check user relationships: {userRelationshipsResult.Errors.FirstOrDefault()}");
        }

        var isCreator = userRelationshipsResult.Value
            .Any(f => f.UserId == request.UserId && f.RecipeId == request.RecipeId && f.RelationshipType == RecipeUserRelationshipType.Creator);

        if (!isCreator)
        {
            return Result<UploadRecipeImageResponse>.Forbidden("Access denied. You can only upload images to recipes you created");
        }

        var validationResult = await _imageStorageService.ValidateImageAsync(
            request.ImageData, 
            request.ContentType, 
            request.FileName);

        if (!validationResult.IsValid)
        {
            return Result<UploadRecipeImageResponse>.Error(validationResult.ErrorMessage ?? "Invalid image");
        }

        recipe.UpdateImage(
            validationResult.ProcessedImageData,
            validationResult.ProcessedContentType,
            request.FileName);

        var updateResult = await _recipeRepository.UpdateAsync(recipe);
        if (!updateResult.IsSuccess)
        {
            return Result<UploadRecipeImageResponse>.Error("Failed to update recipe with image");
        }

        var response = new UploadRecipeImageResponse
        {
            RecipeId = recipe.Id,
            ImageFileName = recipe.ImageFileName,
            ImageContentType = recipe.ImageContentType,
                            ImageSizeInBytes = _recipeService.GetImageSizeInBytes(recipe)
        };

        return Result<UploadRecipeImageResponse>.Success(response);
    }
} 