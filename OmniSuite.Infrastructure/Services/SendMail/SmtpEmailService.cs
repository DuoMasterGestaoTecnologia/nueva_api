using System.Net.Mail;
using System.Net;
public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string bodyHtml);
    Task SendWelcomeEmailAsync(string to, string nome);

    Task SendResetPasswordEmailAsync(string to, string nome, string resetLink);
}

public class SmtpEmailService : IEmailService
{
    private readonly string _smtpServer = "smtp.hostinger.com";
    private readonly int _smtpPort = 587;
    private readonly string _fromEmail = "noreply@flowpag.com";
    private readonly string _fromPassword = "@1Flowpag";
    private readonly string _fromName = "Flowpag";

    public async Task SendEmailAsync(string to, string subject, string bodyHtml)
    {
        using var smtp = new SmtpClient(_smtpServer, _smtpPort)
        {
            Credentials = new NetworkCredential(_fromEmail, _fromPassword),
            EnableSsl = true
        };

        using var message = new MailMessage
        {
            From = new MailAddress(_fromEmail, _fromName),
            Subject = subject,
            Body = bodyHtml,
            IsBodyHtml = true
        };

        message.To.Add(to);

        await smtp.SendMailAsync(message);
    }

    public async Task SendWelcomeEmailAsync(string to, string nome)
    {
        var templatePath = Path.Combine(AppContext.BaseDirectory, "Templates", "welcome.html");
        var html = await File.ReadAllTextAsync(templatePath);

        html = html.Replace("{{nome}}", nome);

        await SendEmailAsync(to, "Bem-vindo à Flowpag 🚀", html);
    }

    public async Task SendResetPasswordEmailAsync(string to, string nome, string resetLink)
    {
        var templatePath = Path.Combine(AppContext.BaseDirectory, "Templates", "reset-password.html");

        if (!File.Exists(templatePath))
            throw new FileNotFoundException("Template de redefinição de senha não encontrado.", templatePath);

        var html = await File.ReadAllTextAsync(templatePath);

        html = html.Replace("{{nome}}", nome)
                   .Replace("{{link}}", resetLink)
                   .Replace("{{ano}}", DateTime.UtcNow.Year.ToString());

        await SendEmailAsync(to, "[Redefinição de senha] Flowpag", html);
    }
}
