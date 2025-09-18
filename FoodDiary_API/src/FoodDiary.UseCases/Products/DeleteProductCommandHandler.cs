using MediatR;
using Ardalis.Result;
using FoodDiary.Core.ProductAggregate;
using FoodDiary.Core.FoodEntryAggregate;
using FoodDiary.Core.RecipeAggregate;

namespace FoodDiary.UseCases.Products;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Result<DeleteProductResponse>>
{
    private readonly FoodDiary.Core.Interfaces.IRepository<Product> _productRepository;
    private readonly FoodDiary.Core.Interfaces.IRepository<FoodEntry> _foodEntryRepository;
    private readonly FoodDiary.Core.Interfaces.IRepository<RecipeIngredient> _recipeIngredientRepository;

    public DeleteProductCommandHandler(
        FoodDiary.Core.Interfaces.IRepository<Product> productRepository,
        FoodDiary.Core.Interfaces.IRepository<FoodEntry> foodEntryRepository,
        FoodDiary.Core.Interfaces.IRepository<RecipeIngredient> recipeIngredientRepository)
    {
        _productRepository = productRepository;
        _foodEntryRepository = foodEntryRepository;
        _recipeIngredientRepository = recipeIngredientRepository;
    }

    public async Task<Result<DeleteProductResponse>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var productResult = await _productRepository.GetByIdAsync(request.ProductId);
        
        if (!productResult.IsSuccess)
        {
            var notFoundResponse = new DeleteProductResponse
            {
                CanDelete = false,
                Message = $"Product with ID {request.ProductId} not found",
                FoodEntryCount = 0,
                RecipeIngredientCount = 0,
                References = new List<string>()
            };
            return Result<DeleteProductResponse>.Success(notFoundResponse);
        }

        var product = productResult.Value;

        var foodEntriesResult = await _foodEntryRepository.ListAsync();
        var foodEntries = foodEntriesResult.IsSuccess ? foodEntriesResult.Value : new List<FoodEntry>();
        var foodEntryCount = foodEntries.Count(fe => fe.ProductId == request.ProductId);

        var recipeIngredientsResult = await _recipeIngredientRepository.ListAsync();
        var recipeIngredients = recipeIngredientsResult.IsSuccess ? recipeIngredientsResult.Value : new List<RecipeIngredient>();
        var recipeIngredientCount = recipeIngredients.Count(ri => ri.ProductId == request.ProductId);

        var canDelete = foodEntryCount == 0 && recipeIngredientCount == 0;
        var references = new List<string>();

        if (foodEntryCount > 0)
        {
            references.Add($"{foodEntryCount} food entr{(foodEntryCount == 1 ? "y" : "ies")}");
        }

        if (recipeIngredientCount > 0)
        {
            references.Add($"{recipeIngredientCount} recipe ingredient{(recipeIngredientCount == 1 ? "" : "s")}");
        }

        if (!canDelete)
        {
            var message = $"Cannot delete product '{product.Name}' because it is referenced by: {string.Join(", ", references)}";
            
            var response = new DeleteProductResponse
            {
                CanDelete = false,
                Message = message,
                FoodEntryCount = foodEntryCount,
                RecipeIngredientCount = recipeIngredientCount,
                References = references
            };

            return Result<DeleteProductResponse>.Success(response);
        }

        var deleteResult = await _productRepository.DeleteAsync(request.ProductId);
        
        if (!deleteResult.IsSuccess)
        {
            var errorResponse = new DeleteProductResponse
            {
                CanDelete = false,
                Message = $"Failed to delete product: {deleteResult.Errors.FirstOrDefault()}",
                FoodEntryCount = 0,
                RecipeIngredientCount = 0,
                References = new List<string>()
            };
            return Result<DeleteProductResponse>.Success(errorResponse);
        }

        var successResponse = new DeleteProductResponse
        {
            CanDelete = true,
            Message = $"Product '{product.Name}' deleted successfully",
            FoodEntryCount = 0,
            RecipeIngredientCount = 0,
            References = new List<string>()
        };

        return Result<DeleteProductResponse>.Success(successResponse);
    }
}
