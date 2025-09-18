using FoodDiary.Web.Configurations;
using FoodDiary.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using FoodDiary.Infrastructure.Password;
using FoodDiary.Infrastructure.Auth;
using FoodDiary.Core.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FoodDiary.Core.UserAggregate;
using FoodDiary.Core.ProductAggregate;
using FoodDiary.Core.FoodEntryAggregate;
using FoodDiary.Core.RecipeAggregate;
using FoodDiary.Infrastructure.Email;
using FoodDiary.Infrastructure.Services;
using FoodDiary.UseCases.Users;
using FoodDiary.UseCases.Analytics;
using FoodDiary.UseCases.Products;
using FoodDiary.UseCases.Recipes;
using MediatR;
using FluentValidation;
using FastEndpoints;

var builder = WebApplication.CreateBuilder(args);

var logger = Log.Logger = new LoggerConfiguration()
  .Enrich.FromLogContext()
  .WriteTo.Console()
  .CreateLogger();

logger.Information("Starting web host");

builder.AddLoggerConfigs();

var appLogger = new SerilogLoggerFactory(logger)
    .CreateLogger<Program>();

builder.Services.AddOptionConfigs(builder.Configuration, appLogger, builder);
builder.Services.AddServiceConfigs(appLogger, builder);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

builder.Services.AddScoped<IEmailConfirmationTokenGenerator, EmailConfirmationTokenGenerator>();
builder.Services.AddScoped<IEmailSender, MimeKitEmailSender>();
builder.Services.AddScoped<FoodDiary.UseCases.Auth.EmailConfiguration>();

builder.Services.AddScoped<FoodDiary.Core.Interfaces.IRepository<User>, EfRepository<User>>();
builder.Services.AddScoped<FoodDiary.Core.Interfaces.IRepository<Product>, EfRepository<Product>>();
builder.Services.AddScoped<FoodDiary.Core.Interfaces.IRepository<FoodEntry>, EfRepository<FoodEntry>>();
builder.Services.AddScoped<FoodDiary.Core.Interfaces.IRepository<Recipe>, EfRepository<Recipe>>();
builder.Services.AddScoped<FoodDiary.Core.Interfaces.IRepository<RecipeIngredient>, EfRepository<RecipeIngredient>>();
builder.Services.AddScoped<FoodDiary.Core.Interfaces.IRepository<RecipeFavorite>, EfRepository<RecipeFavorite>>();

builder.Services.AddScoped<INotificationService, FoodDiary.Infrastructure.Services.NotificationService>();
builder.Services.AddScoped<INotificationSchedulerService, NotificationSchedulerService>();

builder.Services.AddScoped<IImageStorageService, ImageStorageService>();

builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<ITrendsService, TrendsService>();
builder.Services.AddScoped<IRecommendationsService, RecommendationsService>();
builder.Services.AddScoped<IAnalyticsCalculationService, AnalyticsCalculationService>();
builder.Services.AddScoped<INutritionalAnalysisService, NutritionalAnalysisService>();
builder.Services.AddScoped<INutritionalDataService, NutritionalDataService>();

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<FoodDiary.UseCases.Recipes.IRecipeService, FoodDiary.UseCases.Recipes.RecipeService>();
builder.Services.AddScoped<FoodDiary.UseCases.Recipes.IRecipeIngredientService, FoodDiary.UseCases.Recipes.RecipeIngredientService>();
builder.Services.AddScoped<FoodDiary.UseCases.Products.IProductService, FoodDiary.UseCases.Products.ProductService>();
builder.Services.AddScoped<FoodDiary.UseCases.FoodEntries.IFoodEntryService, FoodDiary.UseCases.FoodEntries.FoodEntryService>();
builder.Services.AddScoped<FoodDiary.UseCases.Notifications.INotificationService, FoodDiary.UseCases.Notifications.NotificationService>();

builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IProductManagementService, ProductManagementService>();
builder.Services.AddScoped<IRecipeManagementService, RecipeManagementService>();
builder.Services.AddScoped<IProductDisplayService, ProductDisplayService>();

builder.Services.AddHostedService<FoodDiary.Infrastructure.Services.NotificationBackgroundService>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(FoodDiary.UseCases.Auth.RegisterUserCommand).Assembly));

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(FoodDiary.UseCases.Validation.ValidationBehavior<,>));

builder.Services.AddAutoMapper(typeof(FoodDiary.UseCases.Users.UserProfile).Assembly, typeof(FoodDiary.Web.Mapping.EndpointMappingProfile).Assembly);

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var jwtSecret = jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret not configured");
var jwtIssuer = jwtSettings["Issuer"];
var jwtAudience = jwtSettings["Audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
    };
});

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:3000",
                "http://localhost:5001",
                "http://localhost:5173", 
                "http://localhost:4173",
                "http://localhost:8080",
                "https://localhost:3000",
                "https://localhost:5001",
                "https://localhost:5173",
                "https://localhost:4173",
                "https://localhost:8080"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .WithExposedHeaders("Content-Disposition");
    });
});

builder.Services.AddValidatorsFromAssembly(typeof(FoodDiary.Web.Validation.RegisterRequestValidator).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(FoodDiary.UseCases.Validation.RegisterUserCommandValidator).Assembly);

builder.Services.AddFastEndpoints()
                .SwaggerDocument(o =>
                {
                  o.ShortSchemaNames = true;
                });

var app = builder.Build();

app.UseStaticFiles();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints()
   .UseSwaggerGen();

app.Run();

public partial class Program { }