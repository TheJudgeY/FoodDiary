using MediatR;
using Ardalis.Result;
using FoodDiary.Core.RecipeAggregate;
using FoodDiary.Core.Interfaces;

namespace FoodDiary.UseCases.Recipes;

public class GetRecipeImageCommandHandler : IRequestHandler<GetRecipeImageCommand, Result<GetRecipeImageResponse>>
{
    private readonly IImageStorageService _imageStorageService;
    private readonly FoodDiary.Core.Interfaces.IRepository<Recipe> _recipeRepository;
    private readonly IRecipeService _recipeService;

    public GetRecipeImageCommandHandler(
        IImageStorageService imageStorageService,
        FoodDiary.Core.Interfaces.IRepository<Recipe> recipeRepository,
        IRecipeService recipeService)
    {
        _imageStorageService = imageStorageService;
        _recipeRepository = recipeRepository;
        _recipeService = recipeService;
    }

    public async Task<Result<GetRecipeImageResponse>> Handle(GetRecipeImageCommand request, CancellationToken cancellationToken)
    {
        var recipeResult = await _recipeRepository.GetByIdAsync(request.RecipeId);
        if (!recipeResult.IsSuccess || recipeResult.Value == null)
        {
            return Result<GetRecipeImageResponse>.NotFound($"Recipe with ID {request.RecipeId} not found");
        }

        var recipe = recipeResult.Value;

                    if (!_recipeService.HasImage(recipe))
        {
            return Result<GetRecipeImageResponse>.NotFound($"Recipe with ID {request.RecipeId} has no image");
        }

        var imageDataUrl = await _imageStorageService.GetImageDataUrlAsync(
            recipe.ImageData!, 
            recipe.ImageContentType!);

        var response = new GetRecipeImageResponse
        {
            RecipeId = recipe.Id,
            ImageDataUrl = imageDataUrl,
            ImageContentType = recipe.ImageContentType,
            ImageFileName = recipe.ImageFileName,
                            ImageSizeInBytes = _recipeService.GetImageSizeInBytes(recipe)
        };

        return Result<GetRecipeImageResponse>.Success(response);
    }
} 