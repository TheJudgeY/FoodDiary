using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using AutoMapper;
using FoodDiary.Core.Interfaces;
using FoodDiary.Core.UserAggregate;
using FoodDiary.UseCases.Users;

namespace FoodDiary.Web.Users;

[Authorize]
public class GetUserEndpoint : EndpointWithoutRequest<GetUserResponse>
{
    private readonly IMediator _mediator;
    private readonly IRepository<User> _userRepository;
    private readonly AutoMapper.IMapper _mapper;
    private readonly IUserService _userService;

    public GetUserEndpoint(IMediator mediator, IRepository<User> userRepository, AutoMapper.IMapper mapper, IUserService userService)
    {
        _mediator = mediator;
        _userRepository = userRepository;
        _mapper = mapper;
        _userService = userService;
    }

    public override void Configure()
    {
        Get("/api/users/me");
        Summary(s =>
        {
            s.Summary = "Get current user profile";
            s.Description = "Retrieves the current user's profile information using JWT token";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            await SendErrorsAsync(401, ct);
            return;
        }

        var userResult = await _userRepository.GetByIdAsync(userId);
        if (!userResult.IsSuccess)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var userDto = _mapper.Map<UserDTO>(userResult.Value);

        userDto.BMI = _userService.CalculateBMI(userResult.Value);
        userDto.BMICategory = _userService.GetBMICategory(userResult.Value);
        userDto.BMR = _userService.CalculateBMR(userResult.Value);
        userDto.TDEE = _userService.CalculateTDEE(userResult.Value);
        userDto.RecommendedCalories = _userService.CalculateRecommendedCalories(userResult.Value);
        userDto.HasCompleteProfile = _userService.HasCompleteProfile(userResult.Value);

        var response = new GetUserResponse
        {
            User = userDto
        };

        await SendAsync(response, 200, ct);
    }
}

public class GetUserResponse
{
    public UserDTO User { get; set; } = new();
} 