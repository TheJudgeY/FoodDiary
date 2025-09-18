using MediatR;
using Ardalis.Result;
using FoodDiary.Core.Interfaces;
using FoodDiary.Core.UserAggregate;
using FoodDiary.UseCases.Users.UpdateUser;

namespace FoodDiary.UseCases.Users.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<UpdateUserResponse>>
{
    private readonly FoodDiary.Core.Interfaces.IRepository<User> _userRepository;
    private readonly IUserService _userService;

    public UpdateUserCommandHandler(FoodDiary.Core.Interfaces.IRepository<User> userRepository, IUserService userService)
    {
        _userRepository = userRepository;
        _userService = userService;
    }

    public async Task<Result<UpdateUserResponse>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userResult = await _userRepository.GetByIdAsync(request.UserId);
            if (!userResult.IsSuccess)
            {
                return Result<UpdateUserResponse>.NotFound($"User with ID {request.UserId} not found");
            }

            var user = userResult.Value;
            var hasChanges = false;

            if (!string.IsNullOrWhiteSpace(request.Name) && request.Name != user.Name)
            {
                user.UpdateName(request.Name);
                hasChanges = true;
            }

            if (request.HeightCm.HasValue && request.WeightKg.HasValue && 
                request.Age.HasValue && request.Gender.HasValue && request.ActivityLevel.HasValue)
            {
                _userService.ValidateBodyMetrics(request.HeightCm.Value, request.WeightKg.Value, request.Age.Value);

                user.UpdateBodyMetrics(
                    request.HeightCm.Value,
                    request.WeightKg.Value,
                    request.Age.Value,
                    request.Gender.Value,
                    request.ActivityLevel.Value
                );
                hasChanges = true;
            }

            if (request.FitnessGoal.HasValue)
            {
                user.UpdateFitnessGoal(request.FitnessGoal.Value, request.TargetWeightKg);
                hasChanges = true;
            }

            if (request.DailyProteinGoal.HasValue || request.DailyFatGoal.HasValue || request.DailyCarbohydrateGoal.HasValue || request.DailyCalorieGoal.HasValue)
            {
                user.UpdateMacronutrientGoals(
                    request.DailyProteinGoal,
                    request.DailyFatGoal,
                    request.DailyCarbohydrateGoal,
                    request.DailyCalorieGoal
                );
                hasChanges = true;
            }

            if (!string.IsNullOrWhiteSpace(request.TimeZoneId) && request.TimeZoneId != user.TimeZoneId)
            {
                user.UpdateTimeZone(request.TimeZoneId);
                hasChanges = true;
            }

            if (hasChanges)
            {
                var updateResult = await _userRepository.UpdateAsync(user);
                if (!updateResult.IsSuccess)
                {
                    return Result<UpdateUserResponse>.Error("Failed to update user profile");
                }
            }

            var bmi = _userService.CalculateBMI(user);
            var bmiCategory = _userService.GetBMICategory(user);
            var bmr = _userService.CalculateBMR(user);
            var tdee = _userService.CalculateTDEE(user);
            var recommendedCalories = _userService.CalculateRecommendedCalories(user);

            var response = new UpdateUserResponse
            {
                UserId = user.Id,
                Name = user.Name,
                
                HeightCm = user.HeightCm,
                WeightKg = user.WeightKg,
                Age = user.Age,
                Gender = user.Gender,
                ActivityLevel = user.ActivityLevel,
                
                FitnessGoal = user.FitnessGoal,
                TargetWeightKg = user.TargetWeightKg,
                
                DailyCalorieGoal = user.DailyCalorieGoal,
                DailyProteinGoal = user.DailyProteinGoal,
                DailyFatGoal = user.DailyFatGoal,
                DailyCarbohydrateGoal = user.DailyCarbohydrateGoal,
                
                BMI = bmi,
                BMICategory = bmiCategory,
                BMR = bmr,
                TDEE = tdee,
                RecommendedCalories = recommendedCalories,
                
                TimeZoneId = user.TimeZoneId,
                
                Message = hasChanges ? "User profile updated successfully" : "No changes to update"
            };

            return Result<UpdateUserResponse>.Success(response);
        }
        catch (ArgumentException ex)
        {
            return Result<UpdateUserResponse>.Error($"Invalid data: {ex.Message}");
        }
        catch (Exception ex)
        {
            return Result<UpdateUserResponse>.Error($"Error updating user profile: {ex.Message}");
        }
    }
}
