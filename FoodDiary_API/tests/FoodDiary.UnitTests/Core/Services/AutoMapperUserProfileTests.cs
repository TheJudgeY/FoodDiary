using Xunit;
using AutoMapper;
using FoodDiary.Core.UserAggregate;
using FoodDiary.UseCases.Users;

namespace FoodDiary.UnitTests.Core.Services;

public class AutoMapperUserProfileTests
{
    private readonly IMapper _mapper;

    public AutoMapperUserProfileTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<UserProfile>();
        });
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void User_Maps_To_UserDTO_Correctly()
    {
        var user = new User("test@example.com", "hashedPassword", "Test User");

        var dto = _mapper.Map<UserDTO>(user);

        Assert.Equal(user.Id, dto.Id);
        Assert.Equal(user.Email, dto.Email);
        Assert.Equal(user.Name, dto.Name);
    }
} 