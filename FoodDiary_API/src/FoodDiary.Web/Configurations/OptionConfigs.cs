﻿using Ardalis.ListStartupServices;
using FoodDiary.Infrastructure.Email;

namespace FoodDiary.Web.Configurations;

public static class OptionConfigs
{
  public static IServiceCollection AddOptionConfigs(this IServiceCollection services,
                                                    IConfiguration configuration,
                                                    Microsoft.Extensions.Logging.ILogger logger,
                                                    WebApplicationBuilder builder)
  {
    services.Configure<MailserverConfiguration>(configuration.GetSection("Mailserver"))
    .Configure<CookiePolicyOptions>(options =>
    {
      options.CheckConsentNeeded = context => true;
      options.MinimumSameSitePolicy = SameSiteMode.None;
    });

    if (builder.Environment.IsDevelopment())
    {
      services.Configure<ServiceConfig>(config =>
      {
        config.Services = new List<ServiceDescriptor>(builder.Services);

        config.Path = "/listservices";
      });
    }

    logger.LogInformation("{Project} were configured", "Options");

    return services;
  }
}
