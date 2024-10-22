using System.Net;
using System.Net.Mail;

namespace Blog.Services;

public class EmailServices
{
    public bool Send(
        string toName,
        string toEmail,
        string subject,
        string body,
        string fromName = "Equipe Login",
        string fromEmail = "ad0rn0code@gmail.com")
    {
        var client = new SmtpClient(Configuration.Smtp.Host, Configuration.Smtp.Port)
        {
            Credentials = new NetworkCredential(Configuration.Smtp.UserName, Configuration.Smtp.Password),
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network
        };

        var mail = new MailMessage();
        
        mail.From = new MailAddress(fromEmail, fromName);
        mail.To.Add(new MailAddress(toEmail, toName));
        mail.Subject = subject;
        mail.Body = body;
        mail.IsBodyHtml = true;

        try
        {
            client.Send(mail);
            return true;
        }
        catch
        {
            return false;
        }
    }
}