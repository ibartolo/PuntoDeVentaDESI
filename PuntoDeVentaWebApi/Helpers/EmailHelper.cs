using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace PuntoDeVentaWebApi.Helpers
{
    public static class EmailHelper
    {
        /// <summary>Envía un correo HTML usando la configuración SMTP de appSettings.</summary>
        public static void EnvioEmail(IEnumerable<string> para, string asunto, string mensaje, bool ssl = false, string attachment = "")
        {
            var smtpClient = ConfigurationManager.AppSettings["smtpClient"];
            var userEmail = ConfigurationManager.AppSettings["userEmail"];
            var passEmail = ConfigurationManager.AppSettings["passEmail"];
            int port;
            if (!int.TryParse(ConfigurationManager.AppSettings["port"], out port))
            {
                port = 587;
            }

            using (var mail = new MailMessage())
            {
                mail.From = new MailAddress(userEmail);
                if (para != null)
                {
                    foreach (var destinatario in para)
                    {
                        if (!string.IsNullOrWhiteSpace(destinatario))
                        {
                            mail.To.Add(destinatario);
                        }
                    }
                }

                mail.Subject = asunto;
                mail.Body = mensaje;
                mail.IsBodyHtml = true;

                if (!string.IsNullOrWhiteSpace(attachment))
                {
                    mail.Attachments.Add(new Attachment(attachment));
                }

                using (var client = new SmtpClient(smtpClient, port))
                {
                    client.EnableSsl = ssl;
                    client.Credentials = new NetworkCredential(userEmail, passEmail);
                    client.Send(mail);
                }
            }
        }
    }
}
