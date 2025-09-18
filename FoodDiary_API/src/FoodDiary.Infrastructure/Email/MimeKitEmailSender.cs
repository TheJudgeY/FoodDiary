using FoodDiary.Core.Interfaces;

namespace FoodDiary.Infrastructure.Email;

public class MimeKitEmailSender(ILogger<MimeKitEmailSender> logger,
  IOptions<MailserverConfiguration> mailserverOptions) : IEmailSender
{
  private readonly ILogger<MimeKitEmailSender> _logger = logger;
  private readonly MailserverConfiguration _mailserverConfiguration = mailserverOptions.Value!;

  public async Task SendEmailAsync(string to, string from, string subject, string body)
  {
    _logger.LogInformation("Sending email to {to} from {from} with subject {subject} using Mailtrap", to, from, subject);

    using var client = new MailKit.Net.Smtp.SmtpClient(); 
    
    await client.ConnectAsync(_mailserverConfiguration.Hostname, 
      _mailserverConfiguration.Port, MailKit.Security.SecureSocketOptions.StartTls);
    
    await client.AuthenticateAsync(_mailserverConfiguration.Username, _mailserverConfiguration.Password);
    
    var message = new MimeMessage();
    message.From.Add(new MailboxAddress("FoodDiary", from));
    message.To.Add(new MailboxAddress(to, to));
    message.Subject = subject;
    message.Body = new TextPart("plain") { Text = body };

    await client.SendAsync(message);
    await client.DisconnectAsync(true);
    
    _logger.LogInformation("Email sent successfully to {to}", to);
  }
}
