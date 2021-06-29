using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using WhatsAppApi;
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
            alertita.Mensaje = "Ocurrio un error al enviar el mensaje via Whatsapp";
            alertita.Result = false;
            try
            {
                WhatsApp whatsApp = new WhatsApp("+5214428802842", "S2pRzjF5WnVcHCwGFkjJuFI2oCM=", "sekhar",false,false);
                whatsApp.OnConnectSuccess += () =>
                {
                    whatsApp.OnLoginSuccess += (phone, data) =>
                    {
                        whatsApp.SendMessage(User, Mensaje);
                        alertita.Mensaje = succes;
                        alertita.Result = true;
                    };

                    whatsApp.OnLoginFailed += (data) =>
                    {
                        alertita.Mensaje = data;
                    };
                    whatsApp.Login();
                };

                whatsApp.OnConnectFailed += (ex) =>
                {
                    alertita.Mensaje = ex.Message;
                };

                whatsApp.Connect();
            }
            catch (Exception ex)
            {
                alertita.Mensaje = ex.Message;
                alertita.Result = false;
            }
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