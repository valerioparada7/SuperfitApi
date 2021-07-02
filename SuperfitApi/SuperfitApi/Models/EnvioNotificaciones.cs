using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
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
            alertita.Mensaje = "Ocurrio un error al enviar el mensaje via Whatsapp";
            alertita.Result = false;
            try
            {
                //WebClient client = new WebClient();
                //client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                //client.QueryString.Add("username", "bernardoaltamirano@grupomh.mx");
                //client.QueryString.Add("password", "Imperia_7220041007721");
                //client.QueryString.Add("msisdn", "+52" + User);
                //client.QueryString.Add("message", Mensaje);
                //string baseurl = "http://api.labsmobile.com/get/send.php";
                //Stream data = client.OpenRead(baseurl);
                //StreamReader reader = new StreamReader(data);
                //string s = reader.ReadToEnd();
                //data.Close();
                //reader.Close();
                //alertita.Mensaje = succes;
                //alertita.Result = true;
                //string yourId = "<Your unique X-APIId>";
                //string yourMobile = User;
                //string yourMessage = Mensaje;
                //string url = "https://NiceApi.net/API";
                //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                //request.Method = "POST";
                //request.ContentType = "application/x-www-form-urlencoded";
                //request.Headers.Add("X-APIId", yourId);
                //request.Headers.Add("X-APIMobile", yourMobile);
                //using (StreamWriter streamOut = new StreamWriter(request.GetRequestStream()))
                //{
                //    streamOut.Write(yourMessage);
                //}
                //using (StreamReader streamIn = new StreamReader(request.GetResponse().GetResponseStream()))
                //{
                //    alertita.Mensaje = streamIn.ReadToEnd();
                //    alertita.Result = true;
                //}

                //API KEYY
                //78f3e9a399c4469794c35530525bd127
                //string data = @"{
                //                ""api_key"": 78f3e9a399c4469794c35530525bd127,
                //                ""messages"": [
                //              {
                //                  ""from"": ""4428802842"",
                //                  ""to"": ""User"",
                //                  ""text"": ""Mensaje""
                //                 }
                //             ]
                //            }";
                //data = data.Replace("User", User);
                //data = data.Replace("Mensaje", Mensaje);
                //WebHeaderCollection headers = new WebHeaderCollection();
                //headers.Add("accept", "application/json");
                //headers.Add("content-type", "application/json");
                //WebClient myClient = new WebClient
                //{
                //    Headers = headers
                //};
                //string result;
                //try
                //{
                //    result = myClient.UploadString("https://api.gateway360.com/api/3.0/sms/send", "POST", data);
                //}
                //catch (WebException ex)
                //{
                //    WebResponse response = ex.Response;
                //    Stream dataStream = response.GetResponseStream();
                //    StreamReader reader = new StreamReader(dataStream);
                //    result = reader.ReadToEnd();
                //    alertita.Mensaje = ex.Message;
                //    alertita.Result = false;
                //}

                //var client = new RestClient("https://api.gateway360.com/api/3.0/sms/send");

                //string data = @"{                
                //    ""api_key"":""78f3e9a399c4469794c35530525bd127"",
                //    ""report_url"":""http://yourserver.com/callback/script"",
                //    ""concat"":""1"",
                //    ""messages"":[
                //                {
                //                ""from"":""+524428802842"",
                //            ""to"":""User"",
                //            ""text"":""Mensaje""                            
                //                }
                //            ]
                //        }";
                //data = data.Replace("User", "+52"+User);
                //data = data.Replace("Mensaje", Mensaje);
                //var request = new RestRequest(Method.POST);
                //request.AddHeader("accept", "application/json");
                //request.AddHeader("content-type", "application/json");
                //request.AddParameter("application/json", data, ParameterType.RequestBody);
                //IRestResponse response = client.Execute(request);
                
                alertita.Mensaje = succes;
                alertita.Result = true;
            }
            catch (Exception ex)
            {
                alertita.Mensaje = ex.Message;
                alertita.Result = false;
            }
            return alertita;
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