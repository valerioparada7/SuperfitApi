using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using SuperfitApi.Models;
using SuperfitApi.Models.Entity;
using SuperfitApi.Controllers;
using System.IO;
using System.Net.Mail;
using Twilio;
using Twilio.Types;
using Twilio.Rest.Api.V2010.Account;

namespace SuperfitApi.Controllers
{    
    public class LoginWebController : Controller
    {
        #region Variables 
        public SuperfitEntities  Db;
        public Clientes clientes;
        public Cuestionario cuestionario;
        public Mensualidad mensualidad;
        public Asesoria_antropometria asesoria_antropometria;
        public EnvioNotificaciones envio;
        //modelos
        public ClientesModel clientesMdl;
        public UsuariosModel usuariosMdl;
        public CuestionarioModel cuestionarioMdl;
        public MensualidadModel mensualidadMdl;
        public AntropometriaModel asesoria_antropometriaMdl;
        public AlertasModel alertasMdl;
        #endregion       
        public LoginWebController()
        {
            Db = new SuperfitEntities();
            clientes = new Clientes();
            cuestionario = new Cuestionario();
            mensualidad = new Mensualidad();
            asesoria_antropometria = new Asesoria_antropometria();
            envio = new EnvioNotificaciones();
            //modelos
            clientesMdl = new ClientesModel();
            usuariosMdl = new UsuariosModel();
            cuestionarioMdl = new CuestionarioModel();
            mensualidadMdl = new MensualidadModel();
            asesoria_antropometriaMdl = new AntropometriaModel();
            alertasMdl = new AlertasModel();
        }
        // GET: LoginWeb
        public ActionResult LoginWeb()
        {
            try
            {
                var sesion = Session["sesion"];
                if (sesion != null)
                {
                    return RedirectToAction("Perfil", "ClientesWeb");
                }
                else
                {                    
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.message = ex.Message;
                return View();
            }                       
        }
        [HttpPost]
        public ActionResult LoginWeb(string User, string Pass)
        {
            try
            {
                if (ValidacionUser(User) == true)
                {
                    clientesMdl = (from c in Db.Clientes
                                   where c.Correo_electronico == User
                                   && c.Contraseña == Pass
                                   select new ClientesModel()
                                   {
                                       Id_cliente = c.Id_cliente,
                                       Nombres = c.Nombres,
                                       Foto_perfil = c.Foto_perfil
                                   }).FirstOrDefault();
                }
                else
                {
                    if (ValidarCelular(User) == true)
                    {
                        decimal celular = 0;
                        celular = decimal.Parse(User);
                        clientesMdl = (from c in Db.Clientes
                                       where c.Telefono == celular
                                       && c.Contraseña == Pass
                                       select new ClientesModel()
                                       {
                                           Id_cliente = c.Id_cliente,
                                           Nombres = c.Nombres,
                                           Foto_perfil = c.Foto_perfil
                                       }).FirstOrDefault();
                    }
                    else
                    {
                        clientesMdl = null;
                    }
                }

                if (clientesMdl != null)
                {
                    Session["sesion"] = clientesMdl;
                    Session["Id_cliente"] = clientesMdl.Id_cliente;
                    return RedirectToAction("Perfil", "ClientesWeb");
                }
                else
                {
                    ViewBag.Mensaje = "Usuario y/o contraseña Incorrectos";
                    return View(mensualidadMdl);
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Shared", new { Error = ex.Message });
            }
        }
        
        //Administrador        
        public ActionResult LoginAdministradorWeb()
        {
            try
            {
                var sesion = Session["sesion"];
                if (sesion != null)
                {
                    return RedirectToAction("AdministracionWebHome", "AdministracionWebHome");
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.message = ex.Message;
                return View();
            }                       
        }
        public ActionResult Recuperarcuenta()
        {
            return View();
        }
        #region Envio de correos
        //Envio de recuperacion cuenta contraseña con codigo aleatorio
        [HttpPost]
        public JsonResult Recuperarcuenta(string User)
        {
            alertasMdl.Result = false;
            bool validacion = false;
            string celortel = string.Empty;
            string succes = string.Empty;
            string pass = string.Empty;
            clientes = new Clientes();
            try
            {
                if (ValidacionUser(User) == true)
                {
                    var clientesserch = Db.Clientes.Where(y => y.Correo_electronico == User).FirstOrDefault();
                    if (clientesserch != null)
                    {
                        pass = clientesserch.Contraseña;
                        celortel = "correo";
                        validacion = true;
                        clientes = clientesserch;
                    }
                    else
                    {
                        alertasMdl.Mensaje = "No se encontro ese usuario con ese correo";
                        alertasMdl.Result = false;
                    }
                }
                else
                {
                    if (ValidarCelular(User) == true)
                    {
                        decimal celular = 0;
                        celular = decimal.Parse(User);
                        var clientesserch = Db.Clientes.Where(y => y.Telefono == celular).FirstOrDefault();
                        if (clientesserch != null)
                        {
                            pass = clientesserch.Contraseña;
                            celortel = "cel";
                            validacion = true;
                            clientes = clientesserch;
                        }
                    }
                    else
                    {
                        alertasMdl.Mensaje = "No se encontro ese usuario con ese numero";
                        alertasMdl.Result = false;
                    }
                }

                if (validacion == true)
                {
                    string codigo = envio.GenerarCodigo();
                    clientes.Contraseña = codigo;
                    Db.SaveChanges();
                    if (celortel == "correo")
                    {
                        //
                        Dictionary<string, string> datoscorreo = new Dictionary<string, string>();
                        datoscorreo.Add("@CodigoRecuperacion", codigo);
                        string plantilla = Server.MapPath("~/Plantillas/CorreoRecuperacion.html");
                        succes = "Revisa tu bandeja de correo que proporcionaste para continuar";
                        string asunto = "Recuperar contraseña";
                        AlertasModel resultado = envio.EnviarCorreo(User, plantilla, datoscorreo, succes, asunto);
                        if (resultado.Result == false)
                        {
                            clientes.Contraseña = pass;
                            Db.SaveChanges();
                        }
                        alertasMdl = resultado;                    
                    }
                    else
                    {
                        succes= "Revisa tu whatsapp con el numero proporcionaste para continuar";
                        string Mensaje = string.Empty;                       
                        Mensaje = "Codigo de recuperacion\n";
                        Mensaje += "Introduce este código autogenerado como contraseña temporal,\ndespues que inicies sesion cambia tu contraseña\n";
                        Mensaje += codigo;
                        Mensaje += "\nLos códigos de caducan después de dos horas.\n";
                        Mensaje += "Ir a Superfit\n";
                        Mensaje += "https://www.bsite.net/valerioparada";
                        AlertasModel resultado = envio.EnviarMensaje(User, Mensaje, succes);
                        if (resultado.Result = false)
                        {
                            clientes.Contraseña = pass;
                            Db.SaveChanges();
                        }
                        alertasMdl = resultado;
                    }

                }
                else
                {
                    alertasMdl.Mensaje = alertasMdl.Mensaje;
                    alertasMdl.Result = false;
                }

            }
            catch (Exception ex)
            {
                alertasMdl.Mensaje = ex.Message;
                alertasMdl.Result = false;
            }
            return Json(alertasMdl, JsonRequestBehavior.AllowGet);
        }        
        #endregion

        [HttpPost]
        public ActionResult LoginAdministradorWeb(string User, string Pass)
        {
            try
            {

                usuariosMdl = (from c in Db.Usuarios 
                                where c.Clave_usuario  == User
                                && c.Contraseña == Pass
                                select new UsuariosModel()
                                {
                                    Id_Usuario = c.Id_usuario,
                                    Nombres = c.Nombres,
                                }).FirstOrDefault();                
                if (usuariosMdl != null)
                {
                    Session["sesion"] = clientesMdl;
                    Session["Id_cliente"] = clientesMdl.Id_cliente;
                    return RedirectToAction("AdministracionWebHome", "AdministracionWebHome");
                }
                else
                {
                    ViewBag.Mensaje = "Usuario y/o contraseña Incorrectos";
                    return View(mensualidadMdl);
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Shared", new { Error = ex.Message });
            }
        }

        public bool ValidacionUser(string email)
        {
            string expresion;
            expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (Regex.IsMatch(email, expresion))
            {
                if (Regex.Replace(email, expresion, string.Empty).Length == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool ValidarCelular(string strNumber)
        {
            Regex regex = new Regex(@"\A[0-9]{7,10}\z");
            Match match = regex.Match(strNumber);
            if (match.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        //Registrar Cliente
        public ActionResult RegistrarCliente()
        {            
            ViewBag.Tiporutina = (from t in Db.Tipo_rutina select new TiporutinaModel() { Id_tiporutina = t.Id_tipo_rutina, Descripcion = t.Descripcion }).ToList();
            ViewBag.TipoEntrenamiento = (from t in Db.Tipo_entrenamiento select new TipoentrenamientoModel() { Id_TipoEntrenamiento = t.Id_tipo_entrenamiento, Tipo_entrenamiento = t.Tipo_entrenamientos }).ToList();
            return View();
        }

        [HttpPost]
        public string RegistrarCliente(RegistroCliente Registro)
        {
            string result = "No se guardaron los datos intente de nuevo";
            string Clave = "000C-";
            string fotoperfil="/Imagenes/Clientes/" + Clave;
            string ubicacion = fotoperfil;
            try
            {                       
                if (Registro != null)
                {
                    var cliente = Db.Clientes.Where(p => p.Correo_electronico == Registro.Cliente.Correo_electronico
                                                            || p.Telefono == Registro.Cliente.Telefono).FirstOrDefault();
                    if (cliente == null)
                    {                  
                        clientes = new Clientes
                        {
                            Clave_cliente = Clave,
                            Nombres = Registro.Cliente.Nombres,
                            Apellido_paterno = Registro.Cliente.Apellido_paterno,
                            Apellido_materno = Registro.Cliente.Apellido_materno,
                            Edad = Registro.Cliente.Edad,
                            Telefono = Registro.Cliente.Telefono == null ? 0 : Registro.Cliente.Telefono,
                            Correo_electronico = Registro.Cliente.Correo_electronico == null ? "" : Registro.Cliente.Correo_electronico,
                            Apodo = Registro.Cliente.Apodo == null ? "" : Registro.Cliente.Apodo,
                            Contraseña = Registro.Cliente.Contraseña,
                            Foto_perfil = fotoperfil,
                            Sexo = Registro.Cliente.Sexo,
                            Estado = true
                        };
                        Db.Clientes.Add(clientes);
                        if (Db.SaveChanges() == 1)
                        {
                            int Identity = Db.Clientes.Select(y => y.Id_cliente).Max() + 1;
                            Clientes updatecliente = Db.Clientes.Where(y => y.Id_cliente == clientes.Id_cliente).FirstOrDefault();
                            updatecliente.Clave_cliente = updatecliente.Clave_cliente + "" + Identity.ToString();
                            updatecliente.Foto_perfil = "/Imagenes/Clientes/" + updatecliente.Clave_cliente;                           
                            if (Db.SaveChanges() == 1)
                            {
                                Clave = updatecliente.Clave_cliente;
                                fotoperfil = "/Imagenes/Clientes/" + Clave;
                                cuestionario = new Cuestionario
                                {
                                    Id_cliente = clientes.Id_cliente,
                                    Clave_cuestionario = "CUES-" + Clave,
                                    Padece_enfermedad = Registro.Cuestionario.Padece_enfermedad == null ? false : Registro.Cuestionario.Padece_enfermedad,
                                    Medicamento_prescrito_medico = Registro.Cuestionario.Medicamento_prescrito_medico == null ? "" : Registro.Cuestionario.Medicamento_prescrito_medico,
                                    Lesiones = Registro.Cuestionario.Lesiones == null ? false : Registro.Cuestionario.Lesiones,
                                    Alguna_recomendacion_lesiones = Registro.Cuestionario.Alguna_recomendacion_lesiones == null ? "" : Registro.Cuestionario.Alguna_recomendacion_lesiones,
                                    Fuma = Registro.Cuestionario.Fuma == null ? false : Registro.Cuestionario.Fuma,
                                    Veces_semana_fuma = Registro.Cuestionario.Veces_semana_fuma == null ? 0 : Registro.Cuestionario.Veces_semana_fuma,
                                    Alcohol = Registro.Cuestionario.Alcohol == null ? false : Registro.Cuestionario.Alcohol,
                                    Veces_semana_alcohol = Registro.Cuestionario.Veces_semana_alcohol == null ? 0 : Registro.Cuestionario.Veces_semana_alcohol,
                                    Actividad_fisica = Registro.Cuestionario.Actividad_fisica == null ? false : Registro.Cuestionario.Actividad_fisica,
                                    Tipo_ejercicios = Registro.Cuestionario.Tipo_ejercicios == null ? "" : Registro.Cuestionario.Tipo_ejercicios,
                                    Tiempo_dedicado = Registro.Cuestionario.Tiempo_dedicado == null ? "" : Registro.Cuestionario.Tiempo_dedicado,
                                    Horario_entreno = Registro.Cuestionario.Horario_entreno == null ? "" : Registro.Cuestionario.Horario_entreno,
                                    Metas_objetivos = Registro.Cuestionario.MetasObjetivos == null ? "" : Registro.Cuestionario.MetasObjetivos,
                                    Compromisos = Registro.Cuestionario.Compromisos == null ? "" : Registro.Cuestionario.Compromisos,
                                    Comentarios = Registro.Cuestionario.Comentarios == null ? "" : Registro.Cuestionario.Comentarios,
                                    Fecha_registro = DateTime.Now
                                };
                                Db.Cuestionario.Add(cuestionario);                        
                                if (Db.SaveChanges() == 1)
                                {
                                    Registro.Mensualidad.Fecha_inicio = DateTime.Now;
                                    Registro.Mensualidad.Fecha_fin = DateTime.Now.AddDays(30);
                                    int mes = DateTime.Now.Month;
                                    mensualidad = new Mensualidad
                                    {
                                        Id_cliente = clientes.Id_cliente,
                                        Id_tipo_rutina = Registro.Mensualidad.Tiporutina.Id_tiporutina,
                                        Id_mes = mes,
                                        Id_estatus = 1,
                                        Id_tipo_entrenamiento = Registro.Mensualidad.TipoEntrenamiento.Id_TipoEntrenamiento,
                                        Fecha_inicio = Registro.Mensualidad.Fecha_inicio,
                                        Fecha_fin = Registro.Mensualidad.Fecha_fin
                                    };
                                    Db.Mensualidad.Add(mensualidad);
                                    if (Db.SaveChanges()==1)
                                    {
                                        asesoria_antropometria = new Asesoria_antropometria
                                        {
                                            Peso = Registro.Medidas.Peso,
                                            Id_mensualidad = mensualidad.Id_mensualidad,
                                            Altura = Registro.Medidas.Altura,
                                            IMC = Registro.Medidas.IMC,
                                            Brazo_derecho_relajado = Registro.Medidas.Brazoderechorelajado,
                                            Brazo_derecho_fuerza = Registro.Medidas.Brazoderechofuerza,
                                            Brazo_izquierdo_relajado = Registro.Medidas.Brazoizquierdorelajado,
                                            Brazo_izquierdo_fuerza = Registro.Medidas.Brazoizquierdofuerza,
                                            Cintura = Registro.Medidas.Cintura,
                                            Cadera = Registro.Medidas.Cadera,
                                            Pierna_izquierda = Registro.Medidas.Piernaizquierda,
                                            Pierna_derecho = Registro.Medidas.Piernaderecho,
                                            Pantorrilla_derecha = Registro.Medidas.Pantorrilladerecha,
                                            Pantorrilla_izquierda = Registro.Medidas.Pantorrillaizquierda,
                                            Foto_frontal = fotoperfil,
                                            Foto_perfil = fotoperfil,
                                            Foto_posterior = fotoperfil,
                                            Fecha_registro = DateTime.Now
                                        };
                                        Db.Asesoria_antropometria.Add(asesoria_antropometria);
                                        if (Db.SaveChanges() == 1)
                                        {
                                            string ruta = Server.MapPath("~/Imagenes/Clientes");
                                            ruta = Path.Combine(ruta + @"\" + Clave);
                                            DirectoryInfo di = Directory.CreateDirectory(ruta);
                                            TempData["Ubicacion"] = Clave;
                                            TempData["IdCliente"] = clientes.Id_cliente;
                                            TempData["IdMedidas"] = asesoria_antropometria.Id;
                                            SolicitudRegistro(Registro);
                                            result = "True";
                                        }
                                        else
                                        {
                                            result = "Ocurrio un error al guardar tus medidas reintenta de nuevo y verifica tus datos";
                                            Db.Mensualidad.Remove(mensualidad);
                                            Db.SaveChanges();
                                            Db.Cuestionario.Remove(cuestionario);
                                            Db.SaveChanges();
                                            Db.Clientes.Remove(clientes);
                                            Db.SaveChanges();                                     
                                        }
                                    }
                                    else
                                    {
                                        result = "Ocurrio un error al guardar tu mensualidad reintenta de nuevo y verifica tus datos";
                                        Db.Cuestionario.Remove(cuestionario);
                                        Db.SaveChanges();
                                        Db.Clientes.Remove(clientes);
                                        Db.SaveChanges();
                                    }
                                }
                                else
                                {
                                    result = "Ocurrio un error al guardar tu cuestionario reintenta de nuevo y verifica tus datos";
                                    Db.Clientes.Remove(clientes);
                                    Db.SaveChanges();
                                }
                            }
                            else
                            {
                                result = "Ocurrio un error al guardar los datos intente de nuevo por favor";
                                Db.Clientes.Remove(clientes);
                                Db.SaveChanges();
                            }
                        }
                        else
                        {
                            result = "Ocurrio un error al guardar tus datos personales reintenta de nuevo y verifica tus datos";                        
                        }
                    }
                    else
                    {
                        result = "Ya hay un correo y/o celular registrado por favor intenta otro";                    
                    }
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        [HttpPost]
        public string UpdateImagenes(HttpPostedFileBase fotoperfil, HttpPostedFileBase fotofrontal, 
                                    HttpPostedFileBase fotolateral, HttpPostedFileBase fotoposterior)
        {
            string result = "1";
            try
            {
                string ubicacion = string.Empty, idcliente = string.Empty, medidas = string.Empty;
                int clienteid = 0, medidasid = 0;
                ubicacion = TempData["Ubicacion"].ToString();
                idcliente = TempData["IdCliente"].ToString();
                medidas = TempData["IdMedidas"].ToString();
                clienteid = int.Parse(idcliente);
                medidasid = int.Parse(medidas);
                clientes = Db.Clientes.Where(y => y.Id_cliente == clienteid).FirstOrDefault();
                asesoria_antropometria = Db.Asesoria_antropometria.Where(y => y.Id == medidasid).FirstOrDefault();
                if (fotoperfil!=null)
                {
                    clientes.Foto_perfil = clientes.Foto_perfil + "/" + fotoperfil.FileName.ToString();
                    Db.SaveChanges();
                    fotoperfil.SaveAs(Server.MapPath("~/Imagenes/Clientes/" + ubicacion + "/" + fotoperfil.FileName.ToString()));
                    result = "True";   
                }
                else
                {
                    clientes.Foto_perfil += "/";
                    Db.SaveChanges();
                }
                if(fotofrontal != null)
                {
                    asesoria_antropometria.Foto_frontal = asesoria_antropometria.Foto_frontal + "/" + fotofrontal.FileName.ToString();
                    Db.SaveChanges();
                    fotofrontal.SaveAs(Server.MapPath("~/Imagenes/Clientes/" + ubicacion + "/" + fotofrontal.FileName.ToString()));
                    result = "True";
                }
                if(fotolateral != null)
                {
                    asesoria_antropometria.Foto_perfil = asesoria_antropometria.Foto_perfil + "/" + fotolateral.FileName.ToString();
                    Db.SaveChanges();
                    fotolateral.SaveAs(Server.MapPath("~/Imagenes/Clientes/" + ubicacion + "/" + fotolateral.FileName.ToString()));
                    result = "True";
                }
                if(fotoposterior != null)
                {
                    asesoria_antropometria.Foto_posterior = asesoria_antropometria.Foto_posterior + "/" + fotoposterior.FileName.ToString();
                    Db.SaveChanges();
                    result = "True";
                    fotoposterior.SaveAs(Server.MapPath("~/Imagenes/Clientes/" + ubicacion + "/" + fotoposterior.FileName.ToString()));
                }
            }
            catch (Exception ex)
            {                
                result = ex.Message; 
            }
            return result;
        }
        //Envia correo
        public AlertasModel SolicitudRegistro(RegistroCliente registro)
        {
            try
            {
                string training = Db.Tipo_entrenamiento.Where(y => y.Id_tipo_entrenamiento == registro.Mensualidad.TipoEntrenamiento.Id_TipoEntrenamiento).Select(y => y.Tipo_entrenamientos).FirstOrDefault();
                string rutine = Db.Tipo_rutina.Where(y => y.Id_tipo_rutina == registro.Mensualidad.Tiporutina.Id_tiporutina).Select(y => y.Descripcion).FirstOrDefault();
                Dictionary<string, string> datoscorreo = new Dictionary<string, string>();
                datoscorreo.Add("@Names", registro.Cliente.Nombres);
                datoscorreo.Add("@Lastname", registro.Cliente.Apellido_paterno);
                datoscorreo.Add("@OtherLastname", registro.Cliente.Apellido_materno);
                datoscorreo.Add("@Rutine", rutine);
                datoscorreo.Add("@Training", training);
                datoscorreo.Add("@Rolesex", registro.Cliente.Sexo);
                datoscorreo.Add("@Age", registro.Cliente.Edad.ToString());
                datoscorreo.Add("@Numberphone", registro.Cliente.Telefono.ToString());
                datoscorreo.Add("@Email", registro.Cliente.Correo_electronico);
                string plantilla = Server.MapPath("~/Plantillas/SolicitudRegistro.html");
                string succes = "Se envio tu solicitud de registro a tu entrenador";
                string correomio = "paradavalerio@gmail.com";
                string asunto = "Solicitud de registro";
                AlertasModel resultado = envio.EnviarCorreo(correomio, plantilla, datoscorreo, succes, asunto);
                alertasMdl = resultado;
            }
            catch (Exception ex)
            {
                alertasMdl.Result = false;
                alertasMdl.Mensaje = ex.Message;
            }
            return alertasMdl;
        }
        //Cerrar sesion
        public ActionResult OutWeb()
        {
            try
            {
                Session["sesion"] = null;
                return RedirectToAction("LoginWeb", "LoginWeb");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Shared", new { Error = ex.Message });
            }            
        }

        public bool Validation()
        {
            try
            {
                var sesion = Session["sesion"];
                if (sesion == null)
                {
                    return true;
                }                                
            }
            catch (Exception ex)
            {
                return false;
            }
            return false;
        }
    }
 
}