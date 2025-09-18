using Xunit;
using AutoMapper;
using FoodDiary.Core.UserAggregate;
using FoodDiary.UseCases.Users;

namespace FoodDiary.UnitTests.Core.Services;

public class AutoMapperIntegrationTests
{
    private readonly IMapper _mapper;

    public AutoMapperIntegrationTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<UserProfile>();
        });
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void UserProfile_Configuration_ShouldBeValid()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<UserProfile>();
        });

        config.AssertConfigurationIsValid();
    }

    [Fact]
    public void User_Maps_To_UserDTO_WithAllProperties()
    {
        var user = new User("test@example.com", "shouldNotBeMapped", "Test User");
        user.ConfirmEmail();
        user.SetEmailConfirmationToken("token123", DateTime.UtcNow.AddHours(24));

        var dto = _mapper.Map<UserDTO>(user);

        Assert.Equal(user.Id, dto.Id);
        Assert.Equal(user.Email, dto.Email);
        Assert.Equal(user.Name, dto.Name);
        Assert.Null(dto.GetType().GetProperty("PasswordHash")?.GetValue(dto));
        Assert.Null(dto.GetType().GetProperty("EmailConfirmed")?.GetValue(dto));
        Assert.Null(dto.GetType().GetProperty("EmailConfirmationToken")?.GetValue(dto));
        Assert.Null(dto.GetType().GetProperty("EmailConfirmationTokenExpiresAt")?.GetValue(dto));
    }

    [Fact]
    public void User_Maps_To_UserDTO_WithNullValues()
    {
        var user = new User("", "hash", "");

        var dto = _mapper.Map<UserDTO>(user);

        Assert.Equal(user.Id, dto.Id);
        Assert.Equal(user.Email, dto.Email);
        Assert.Equal(user.Name, dto.Name);
    }

    [Fact]
    public void User_Maps_To_UserDTO_WithEmptyStrings()
    {
        var user = new User("", "hash", "");

        var dto = _mapper.Map<UserDTO>(user);

        Assert.Equal(user.Id, dto.Id);
        Assert.Equal(user.Email, dto.Email);
        Assert.Equal(user.Name, dto.Name);
    }

    [Fact]
    public void User_Maps_To_UserDTO_WithSpecialCharacters()
    {
        var user = new User("test+tag@example.com", "hash", "José María O'Connor-Smith");

        var dto = _mapper.Map<UserDTO>(user);

        Assert.Equal(user.Id, dto.Id);
        Assert.Equal(user.Email, dto.Email);
        Assert.Equal(user.Name, dto.Name);
    }

    [Fact]
    public void User_Maps_To_UserDTO_WithVeryLongStrings()
    {
        var longEmail = new string('a', 100) + "@example.com";
        var longName = new string('b', 200);
        var user = new User(longEmail, "hash", longName);

        var dto = _mapper.Map<UserDTO>(user);

        Assert.Equal(user.Id, dto.Id);
        Assert.Equal(user.Email, dto.Email);
        Assert.Equal(user.Name, dto.Name);
    }

    [Fact]
    public void User_Maps_To_UserDTO_WithMinimalData()
    {
        var user = new User("minimal@test.com", "password", "Minimal User");

        var dto = _mapper.Map<UserDTO>(user);

        Assert.Equal(user.Id, dto.Id);
        Assert.Equal(user.Email, dto.Email);
        Assert.Equal(user.Name, dto.Name);
    }

    [Fact]
    public void User_Maps_To_UserDTO_PreservesDataIntegrity()
    {
        var originalEmail = "integrity@test.com";
        var originalName = "Integrity Test User";
        
        var user = new User(originalEmail, "shouldNotAffectMapping", originalName);
        user.ConfirmEmail();

        var dto = _mapper.Map<UserDTO>(user);

        Assert.Equal(originalEmail, dto.Email);
        Assert.Equal(originalName, dto.Name);
        
        Assert.Equal(originalEmail, user.Email);
        Assert.Equal(originalName, user.Name);
        Assert.Equal("shouldNotAffectMapping", user.PasswordHash);
        Assert.True(user.EmailConfirmed);
    }
} 