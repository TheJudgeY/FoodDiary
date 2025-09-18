using FoodDiary.Core.Interfaces;
using FoodDiary.Infrastructure;
using FoodDiary.Infrastructure.Email;

namespace FoodDiary.Web.Configurations;

public static class ServiceConfigs
{
  public static IServiceCollection AddServiceConfigs(this IServiceCollection services, Microsoft.Extensions.Logging.ILogger logger, WebApplicationBuilder builder)
  {

    services.AddScoped<IEmailSender, MimeKitEmailSender>();

    logger.LogInformation("{Project} services registered", "Mediatr and Email Sender");

    return services;
  }
}
