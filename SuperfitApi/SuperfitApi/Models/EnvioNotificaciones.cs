using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace SuperfitApi.Models
{
    public class EnvioNotificaciones
    {
        public AlertasModel alertasMdl;
        public EnvioNotificaciones()
        {
            alertasMdl = new AlertasModel();
        }
        public AlertasModel EnviarCorreo(string User, string Plantilla, Dictionary<string, string> Datos, string succes, string asunto)
        {
            AlertasModel alertita = new AlertasModel();
            try
            {
                string ruta = Plantilla;
                string html = System.IO.File.ReadAllText(ruta);
                foreach (KeyValuePair<string, string> dato in Datos)
                {
                    html = html.Replace(dato.Key, dato.Value);
                }

                MailMessage MyMailMessage = new MailMessage();
                MyMailMessage.From = new MailAddress("soportebysuperfitvalerio@gmail.com");
                MyMailMessage.To.Add(User);
                MyMailMessage.Subject = asunto;
                AlternateView HTMLConImagenes;
                HTMLConImagenes = AlternateView.CreateAlternateViewFromString(html, null, "text/html");
                MyMailMessage.AlternateViews.Add(HTMLConImagenes);
                SmtpClient SMTPServer = new SmtpClient();
                SMTPServer.Port = 25;
                SMTPServer.Host = "smtp.gmail.com";
                SMTPServer.Credentials = new System.Net.NetworkCredential("soportebysuperfitvalerio@gmail.com", "Angelawhite7");
                SMTPServer.EnableSsl = true;
                try
                {
                    SMTPServer.Send(MyMailMessage);
                    alertasMdl.Mensaje = succes;
                    alertasMdl.Result = true;
                }
                catch (Exception ex)
                {
                    alertasMdl.Mensaje = ex.Message;
                    alertasMdl.Result = false;
                }
            }
            catch (Exception ex)
            {
                alertasMdl.Mensaje = ex.Message;
                alertasMdl.Result = false;
            }
            return alertita = alertasMdl;

        }

        public AlertasModel EnviarMensaje(string User, string Mensaje, string succes)
        {
            AlertasModel alertita = new AlertasModel();
           /* try
            {
                var accountSid = "ACd6ccd243ec7ba11411f5f8888ef37e53";
                var authToken = "e8a54699fed75cbcda2cd36d3059a529";
                TwilioClient.Init(accountSid, authToken);

                var messageOptions = new CreateMessageOptions(
                    new PhoneNumber("whatsapp:+52" + User));
                messageOptions.From = new PhoneNumber("whatsapp:+14155238886");
                messageOptions.Body = Mensaje;

                var message = MessageResource.Create(messageOptions);
                alertasMdl.Mensaje = succes;
                alertasMdl.Result = true;
            }
            catch (Exception ex)
            {
                alertasMdl.Mensaje = ex.Message;
                alertasMdl.Result = false;
            }*/
            return alertita = alertasMdl;
        }
        public string GenerarCodigo()
        {
            Random random = new Random();
            string alfabeto = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            int numero = random.Next(1, 1000);
            string codigo = string.Empty;
            for (int i = 1; i <= 5; i++)
            {
                int ind = random.Next(alfabeto.Length);
                codigo += alfabeto[ind];
            }
            codigo += numero;
            return codigo;
        }
    }
}