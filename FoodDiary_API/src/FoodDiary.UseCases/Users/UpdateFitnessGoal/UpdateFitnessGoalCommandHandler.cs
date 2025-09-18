using MediatR;
using Ardalis.Result;
using FoodDiary.Core.Interfaces;
using FoodDiary.Core.UserAggregate;
using AutoMapper;
using FoodDiary.UseCases.Users;

namespace FoodDiary.UseCases.Users.UpdateFitnessGoal;

public class UpdateFitnessGoalCommandHandler : IRequestHandler<UpdateFitnessGoalCommand, Result<UpdateFitnessGoalResponse>>
{
    private readonly FoodDiary.Core.Interfaces.IRepository<User> _userRepository;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public UpdateFitnessGoalCommandHandler(
        FoodDiary.Core.Interfaces.IRepository<User> userRepository,
        IMapper mapper,
        IUserService userService)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _userService = userService;
    }

    public async Task<Result<UpdateFitnessGoalResponse>> Handle(UpdateFitnessGoalCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userResult = await _userRepository.GetByIdAsync(request.UserId);
            if (!userResult.IsSuccess)
            {
                return Result<UpdateFitnessGoalResponse>.NotFound($"User with ID {request.UserId} not found");
            }

            var user = userResult.Value;

            user.UpdateFitnessGoal(request.FitnessGoal, request.TargetWeightKg);

            var updateResult = await _userRepository.UpdateAsync(user);
            if (!updateResult.IsSuccess)
            {
                return Result<UpdateFitnessGoalResponse>.Error("Failed to update user fitness goal");
            }

            var recommendedCalories = _userService.CalculateRecommendedCalories(user);

            var response = new UpdateFitnessGoalResponse
            {
                UserId = user.Id,
                FitnessGoal = user.FitnessGoal!.Value,
                TargetWeightKg = user.TargetWeightKg,
                RecommendedCalories = recommendedCalories,
                Message = "Fitness goal updated successfully"
            };

            return Result<UpdateFitnessGoalResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<UpdateFitnessGoalResponse>.Error($"Error updating fitness goal: {ex.Message}");
        }
    }
} 