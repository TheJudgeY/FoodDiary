namespace FoodDiary.Infrastructure.Email;

public class MailserverConfiguration()
{
  public string Hostname { get; set; } = "sandbox.smtp.mailtrap.io";
  public int Port { get; set; } = 2525;
  public string Username { get; set; } = "980ebd65aafac0";
  public string Password { get; set; } = "3640aff510338a";
  public bool EnableSsl { get; set; } = true;
}
