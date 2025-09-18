using MediatR;
using Ardalis.Result;
using FoodDiary.Core.Interfaces;
using FoodDiary.Core.UserAggregate;
using AutoMapper;

namespace FoodDiary.UseCases.Users.UpdateMacronutrientGoals;

public class UpdateMacronutrientGoalsCommandHandler : IRequestHandler<UpdateMacronutrientGoalsCommand, Result<UpdateMacronutrientGoalsResponse>>
{
    private readonly FoodDiary.Core.Interfaces.IRepository<User> _userRepository;
    private readonly IMapper _mapper;

    public UpdateMacronutrientGoalsCommandHandler(
        FoodDiary.Core.Interfaces.IRepository<User> userRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<Result<UpdateMacronutrientGoalsResponse>> Handle(UpdateMacronutrientGoalsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userResult = await _userRepository.GetByIdAsync(request.UserId);
            if (!userResult.IsSuccess)
            {
                return Result<UpdateMacronutrientGoalsResponse>.NotFound($"User with ID {request.UserId} not found");
            }

            var user = userResult.Value;

            user.UpdateMacronutrientGoals(
                request.DailyProteinGoal,
                request.DailyFatGoal,
                request.DailyCarbohydrateGoal,
                request.DailyCalorieGoal
            );

            var updateResult = await _userRepository.UpdateAsync(user);
            if (!updateResult.IsSuccess)
            {
                return Result<UpdateMacronutrientGoalsResponse>.Error("Failed to update user macronutrient goals");
            }

            var response = new UpdateMacronutrientGoalsResponse
            {
                UserId = user.Id,
                DailyCalorieGoal = user.DailyCalorieGoal,
                DailyProteinGoal = user.DailyProteinGoal,
                DailyFatGoal = user.DailyFatGoal,
                DailyCarbohydrateGoal = user.DailyCarbohydrateGoal,
                Message = "Macronutrient goals updated successfully"
            };

            return Result<UpdateMacronutrientGoalsResponse>.Success(response);
        }
        catch (ArgumentException ex)
        {
            return Result<UpdateMacronutrientGoalsResponse>.Error($"Invalid macronutrient goals: {ex.Message}");
        }
        catch (Exception ex)
        {
            return Result<UpdateMacronutrientGoalsResponse>.Error($"Error updating macronutrient goals: {ex.Message}");
        }
    }
} 