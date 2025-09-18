namespace FoodDiary.UseCases.Auth;

public static class AuthConstants
{
    public static class ErrorMessages
    {
        public const string UserAlreadyExists = "User with this email already exists";
        public const string RegistrationFailed = "Registration failed";
    }

    public static class SuccessMessages
    {
        public const string RegistrationSuccessful = "Registration successful! Please check your email to confirm your account.";
    }

    public static class EmailTemplates
    {
        public const string ConfirmationSubject = "Confirm your FoodDiary account";
        public const string ConfirmationSenderEmail = "noreply@fooddiary.com";
        
        public static string GetConfirmationEmailBody(string userName, string confirmationUrl) => $@"
Hello {userName},

Thank you for registering with FoodDiary! Please confirm your email address by clicking the link below:

{confirmationUrl}

This link will expire in 24 hours.

If you didn't create this account, please ignore this email.

Best regards,
The FoodDiary Team";
    }

    public static class Configuration
    {
        public const string FrontendUrlKey = "FrontendUrl";
        public const string DefaultFrontendUrl = "http://localhost:4173";
        public const string ConfirmEmailPath = "/confirm-email";
    }

    public static class Logging
    {
        public const string AttemptingSendEmail = "Attempting to send confirmation email to {Email}";
        public const string ConfirmationUrlLog = "Confirmation URL: {ConfirmationUrl}";
        public const string EmailSentSuccessfully = "Confirmation email sent successfully to {Email}";
        public const string EmailSendFailed = "Failed to send confirmation email to {Email}: {ErrorMessage}";
        public const string ExceptionDetails = "Exception details: {Exception}";
    }
}
