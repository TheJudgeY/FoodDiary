using MediatR;
using Ardalis.Result;
using FoodDiary.Core.Interfaces;
using FoodDiary.Core.UserAggregate;
using AutoMapper;
using FoodDiary.UseCases.Users;

namespace FoodDiary.UseCases.Users.UpdateBodyMetrics;

public class UpdateBodyMetricsCommandHandler : IRequestHandler<UpdateBodyMetricsCommand, Result<UpdateBodyMetricsResponse>>
{
    private readonly FoodDiary.Core.Interfaces.IRepository<User> _userRepository;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public UpdateBodyMetricsCommandHandler(
        FoodDiary.Core.Interfaces.IRepository<User> userRepository,
        IMapper mapper,
        IUserService userService)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _userService = userService;
    }

    public async Task<Result<UpdateBodyMetricsResponse>> Handle(UpdateBodyMetricsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userResult = await _userRepository.GetByIdAsync(request.UserId);
            if (!userResult.IsSuccess)
            {
                return Result<UpdateBodyMetricsResponse>.NotFound($"User with ID {request.UserId} not found");
            }

            var user = userResult.Value;

            _userService.ValidateBodyMetrics(request.HeightCm, request.WeightKg, request.Age);

            user.UpdateBodyMetrics(
                request.HeightCm,
                request.WeightKg,
                request.Age,
                request.Gender,
                request.ActivityLevel
            );

            var updateResult = await _userRepository.UpdateAsync(user);
            if (!updateResult.IsSuccess)
            {
                return Result<UpdateBodyMetricsResponse>.Error("Failed to update user body metrics");
            }

            var bmi = _userService.CalculateBMI(user);
            var bmiCategory = _userService.GetBMICategory(user);
            var bmr = _userService.CalculateBMR(user);
            var tdee = _userService.CalculateTDEE(user);

            var response = new UpdateBodyMetricsResponse
            {
                UserId = user.Id,
                HeightCm = user.HeightCm!.Value,
                WeightKg = user.WeightKg!.Value,
                Age = user.Age!.Value,
                Gender = user.Gender!.Value,
                ActivityLevel = user.ActivityLevel!.Value,
                BMI = bmi,
                BMICategory = bmiCategory,
                BMR = bmr,
                TDEE = tdee,
                Message = "Body metrics updated successfully"
            };

            return Result<UpdateBodyMetricsResponse>.Success(response);
        }
        catch (ArgumentException ex)
        {
            return Result<UpdateBodyMetricsResponse>.Error($"Invalid body metrics: {ex.Message}");
        }
        catch (Exception ex)
        {
            return Result<UpdateBodyMetricsResponse>.Error($"Error updating body metrics: {ex.Message}");
        }
    }
} 