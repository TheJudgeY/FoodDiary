using Xunit;
using Microsoft.Extensions.Configuration;
using FoodDiary.Infrastructure.Auth;
using System.IdentityModel.Tokens.Jwt;

namespace FoodDiary.UnitTests.Infrastructure;

public class JwtTokenGeneratorTests
{
    private readonly IConfiguration _configuration;
    private readonly JwtTokenGenerator _jwtGenerator;

    public JwtTokenGeneratorTests()
    {
        var configValues = new Dictionary<string, string?>
        {
            {"JwtSettings:Secret", "test_secret_key_for_unit_tests_only_very_long_key"},
            {"JwtSettings:Issuer", "TestIssuer"},
            {"JwtSettings:Audience", "TestAudience"},
            {"JwtSettings:ExpiresInMinutes", "60"}
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configValues)
            .Build();

        _jwtGenerator = new JwtTokenGenerator(_configuration);
    }

    [Fact]
    public void GenerateToken_ShouldReturnValidJwtToken()
    {
        var userId = "test-user-id";
        var email = "test@example.com";
        var name = "Test User";

        var token = _jwtGenerator.GenerateToken(userId, email, name);

        Assert.NotNull(token);
        Assert.NotEmpty(token);
        
        var handler = new JwtSecurityTokenHandler();
        Assert.True(handler.CanReadToken(token));
    }

    [Fact]
    public void GenerateToken_ShouldContainCorrectClaims()
    {
        var userId = "test-user-id";
        var email = "test@example.com";
        var name = "Test User";

        var token = _jwtGenerator.GenerateToken(userId, email, name);

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        Assert.Equal(userId, jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value);
        Assert.Equal(email, jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value);
        Assert.Equal(name, jwtToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value);
        Assert.NotNull(jwtToken.Claims.FirstOrDefault(c => c.Type == "jti")?.Value);
    }

    [Fact]
    public void GenerateToken_ShouldHaveCorrectExpiration()
    {
        var userId = "test-user-id";
        var email = "test@example.com";
        var name = "Test User";

        var token = _jwtGenerator.GenerateToken(userId, email, name);

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var expectedExpiration = DateTime.UtcNow.AddMinutes(60);
        Assert.True(jwtToken.ValidTo > DateTime.UtcNow);
        Assert.True(jwtToken.ValidTo <= expectedExpiration);
    }

    [Fact]
    public void GenerateToken_WithDifferentUsers_ShouldGenerateDifferentTokens()
    {
        var user1 = ("user1", "user1@example.com", "User 1");
        var user2 = ("user2", "user2@example.com", "User 2");

        var token1 = _jwtGenerator.GenerateToken(user1.Item1, user1.Item2, user1.Item3);
        var token2 = _jwtGenerator.GenerateToken(user2.Item1, user2.Item2, user2.Item3);

        Assert.NotEqual(token1, token2);
    }

    [Fact]
    public void GenerateToken_WithMissingConfiguration_ShouldThrowException()
    {
        var emptyConfig = new ConfigurationBuilder().Build();
        var generator = new JwtTokenGenerator(emptyConfig);

        Assert.Throws<InvalidOperationException>(() => 
            generator.GenerateToken("user", "email", "name"));
    }
} 