using Microsoft.Extensions.Configuration;

namespace FoodDiary.UseCases.Auth;

public class EmailConfiguration
{
    private readonly IConfiguration _configuration;

    public EmailConfiguration(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string FrontendUrl => _configuration[AuthConstants.Configuration.FrontendUrlKey] 
                               ?? AuthConstants.Configuration.DefaultFrontendUrl;

    public string SenderEmail => AuthConstants.EmailTemplates.ConfirmationSenderEmail;
    
    public string ConfirmationSubject => AuthConstants.EmailTemplates.ConfirmationSubject;

    public string BuildConfirmationUrl(string email, string token)
    {
        return $"{FrontendUrl}{AuthConstants.Configuration.ConfirmEmailPath}?email={Uri.EscapeDataString(email)}&token={token}";
    }

    public string BuildConfirmationEmailBody(string userName, string confirmationUrl)
    {
        return AuthConstants.EmailTemplates.GetConfirmationEmailBody(userName, confirmationUrl);
    }
}
