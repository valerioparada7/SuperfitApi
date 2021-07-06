using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using SuperfitApi.Models.Entity;
using SuperfitApi.Models;
using System.Web;
using System.Web.Hosting;
using System.Drawing;
using System.Net.Mail;

namespace SuperfitApi.Controllers
{
    public class LoginController : ApiController
    {        
        #region Variables 
        public SuperfitEntities Db;
        public Clientes clientes;
        public Cuestionarios cuestionario;
        public Mensualidades mensualidad;
        public Asesoria_antropometria asesoria_antropometria;
        public EnvioNotificaciones envio;
        //modelos
        public ClientesModel clientesMdl;
        public CuestionarioModel cuestionarioMdl;
        public MensualidadModel mensualidadMdl;
        public AntropometriaModel asesoria_antropometriaMdl;
        public AlertasModel alertasMdl;
        public LoginWebController logincontrol;
        //Variables globales
        public string Ubicacion = string.Empty;
        public string IdCliente=string.Empty;
        public string IdMedidas=string.Empty;
        #endregion       
        public LoginController()
        {
            Db = new SuperfitEntities();
            clientes = new Clientes();
            cuestionario = new Cuestionarios();
            mensualidad = new Mensualidades();
            asesoria_antropometria = new Asesoria_antropometria();
            envio = new EnvioNotificaciones();
            //modelos
            clientesMdl = new ClientesModel();
            cuestionarioMdl = new CuestionarioModel();
            mensualidadMdl = new MensualidadModel();
            asesoria_antropometriaMdl = new AntropometriaModel();
            alertasMdl = new AlertasModel();
        }
        //Recuperar cuenta
        [HttpPost]
        [Route("api/Login/Recuperarcuenta")]
        public AlertasModel Recuperarcuenta(string User)
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
                            celortel = "correo";
                            validacion = true;
                            clientes = clientesserch;
                            User = clientesserch.Correo_electronico;
                        }
                        else
                        {
                            alertasMdl.Mensaje = "No se encontro ese usuario con ese numero";
                            alertasMdl.Result = false;
                        }
                    }
                    else
                    {
                        alertasMdl.Mensaje = "No se encontro ese usuario con";
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
                        string plantilla = HostingEnvironment.MapPath("~/Plantillas/CorreoRecuperacion.html");
                        succes = "Revisa tu correo que proporcionaste para continuar";
                        string asunto = "Recuperacion de cuenta";
                        AlertasModel resultado = envio.EnviarCorreo(User, plantilla, datoscorreo, succes,asunto);
                        if (resultado.Result == false)
                        {
                            clientes.Contraseña = pass;
                            Db.SaveChanges();
                        }
                        alertasMdl = resultado;
                    }
                    else
                    {
                        succes = "Revisa tu whatsapp con el numero proporcionaste para continuar";
                        string Mensaje = string.Empty;
                        Mensaje = "Codigo de recuperacion\n";
                        Mensaje += "Introduce este código autogenerado como contraseña temporal,\ndespues que inicies sesion cambia tu contraseña\n";
                        Mensaje += codigo;
                        Mensaje += "\nLos códigos de caducan después de dos horas.\n";
                        Mensaje += "Ir a Superfit\n";
                        Mensaje += "https://www.bsite.net/valerioparada";
                        AlertasModel resultado = envio.EnviarMensaje(User, Mensaje, succes);
                        if (resultado.Result == false)
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
            return alertasMdl;
        }        
        //Loguearse
        [HttpGet]
        [Route("api/Login/Login")]
        public MensualidadModel Login(string User,string Pass)
        {            
            if (ValidacionUser(User)==true)
            {
                clientesMdl = (from c in Db.Clientes
                               where c.Correo_electronico == User
                               && c.Contraseña == Pass
                               select new ClientesModel()
                               {
                                   Id_cliente = c.Id_cliente,
                                   Nombres = c.Nombres,
                                   Foto_perfil=c.Foto_perfil
                               }).FirstOrDefault();
                if (clientesMdl == null)
                {
                    clientesMdl = new ClientesModel();
                }
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


                    if (clientesMdl == null)
                    {
                        clientesMdl = new ClientesModel();                        
                    }
                }
            }

            
            if (clientesMdl.Id_cliente != 0)
            {
                var list = Db.Mensualidades.Where(p => p.Id_cliente  == clientesMdl.Id_cliente).ToList();
                var mensualidad = list.OrderByDescending(p => p.Fecha_fin).FirstOrDefault();
                if (mensualidad != null)
                {
                    int id = mensualidad.Id_mensualidad;
                    mensualidadMdl = (from m in Db.Mensualidades  
                                      join t in Db.Tipo_rutinas
                                      on m.Id_tipo_rutina equals t.Id_tipo_rutina
                                      join te in Db.Tipo_entrenamientos
                                      on m.Id_tipo_entrenamiento equals te.Id_tipo_entrenamiento
                                      join meses in Db.Meses 
                                      on m.Id_mes equals meses.Id_mes
                                      join es in Db.Estatus
                                      on m.Id_estatus equals es.Id_estatus                                      
                                      where m.Id_mensualidad == id
                                      select new MensualidadModel()
                                      {
                                          Id_mensualidad = m.Id_mensualidad,                                          
                                          Tiporutina = new TiporutinaModel
                                          {
                                              Id_tiporutina = t.Id_tipo_rutina,
                                              Tipo = t.Tipo
                                          },
                                          TipoEntrenamiento = new TipoentrenamientoModel
                                          {
                                              Id_TipoEntrenamiento = (int)te.Id_tipo_entrenamiento,
                                              Clave_Entrenamiento = te.Clave_entrenamiento,
                                              Tipo_entrenamiento = te.Tipo_entrenamiento 
                                          },
                                          Mes = new MesesModel
                                          {
                                              Id_mes = meses.Id_mes,
                                              Clave_mes = meses.Clave_mes,
                                              Mes = meses.Mes 
                                          },
                                          Estatus = new EstatusModel
                                          {
                                              Id_estatus=es.Id_estatus,
                                              Descripcion =es.Descripcion
                                          },
                                          Fecha_inicio = (DateTime)m.Fecha_inicio,
                                          Fecha_fin = (DateTime)m.Fecha_fin,
                                      }).FirstOrDefault();
                    
                    if (mensualidadMdl != null)
                    {                                                
                        mensualidadMdl.Cliente = new ClientesModel() 
                        {
                            Id_cliente = clientesMdl.Id_cliente,
                            Validar = true,
                            Nombres = clientesMdl.Nombres,
                            Foto_perfil = clientesMdl.Foto_perfil
                        };                                                                                                             
                        return mensualidadMdl;
                    }
                    else
                    {
                        mensualidadMdl = new MensualidadModel
                        {
                            Cliente = new ClientesModel()
                        };
                        clientesMdl.Id_cliente = clientesMdl.Id_cliente;
                        clientesMdl.Validar = true;
                        clientesMdl.Nombres = clientesMdl.Nombres;
                        clientesMdl.Foto_perfil = clientesMdl.Foto_perfil;
                        mensualidadMdl.Cliente = clientesMdl;
                        return mensualidadMdl;
                    }
                }
                else
                {
                    mensualidadMdl = new MensualidadModel
                    {
                        Cliente = new ClientesModel()
                    };
                    clientesMdl.Id_cliente = clientesMdl.Id_cliente;
                    clientesMdl.Validar = true;
                    clientesMdl.Nombres = clientesMdl.Nombres;
                    clientesMdl.Foto_perfil = clientesMdl.Foto_perfil;
                    mensualidadMdl.Cliente = clientesMdl;
                    return mensualidadMdl;
                }

            }
            else
            {
                mensualidadMdl = new MensualidadModel();
                mensualidadMdl.Cliente = new ClientesModel();                               
                mensualidadMdl.Cliente.Validar = false;
                mensualidadMdl.Cliente.Nombres = "Usuario y/o contraseña Incorrectos";                
                return mensualidadMdl;
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
        //Crear cuenta registrando el cliente
        [HttpPost]
        [Route("api/Login/RegistrarCliente")]
        public AlertasModel RegistrarCliente(ClientesModel clientesModel)
        {            
            var cliente = Db.Clientes.Where(p => p.Correo_electronico == clientesModel.Correo_electronico
                                        || p.Telefono == clientesModel.Telefono).FirstOrDefault();

            if (cliente == null) 
            {
                string Clave = "";
                Clave = clientesModel.Nombres.Substring(0, 3) + clientesModel.Apellido_paterno.Substring(0, 3) +
                        clientesModel.Apellido_materno.Substring(0, 3);  
                string Name = clientesModel.Nombres + clientesModel.Apellido_paterno +
                        clientesModel.Apellido_materno;
                Name = Name.Replace(" ", "_");
                string Foto_perfil = "Imagenes/Clientes/" + Name;
                clientes = new Clientes
                {
                    Clave_cliente = Clave,
                    Nombres = clientesModel.Nombres,
                    Apellido_paterno = clientesModel.Apellido_paterno,
                    Apellido_materno = clientesModel.Apellido_materno,
                    Edad = clientesModel.Edad,
                    Telefono = clientesModel.Telefono,
                    Correo_electronico = clientesModel.Correo_electronico,
                    Apodo = clientesModel.Apodo,
                    Contraseña = clientesModel.Contraseña,
                    Foto_perfil = Foto_perfil,
                    Sexo= clientesModel.Sexo,
                    Estado = true
                };
                Db.Clientes.Add(clientes);
                if (Db.SaveChanges() == 1)
                {
                    DirectoryInfo di = Directory.CreateDirectory(Foto_perfil);
                   // clientesModel.Imagen.SaveAs("");
                    alertasMdl.Id = clientes.Id_cliente;
                    alertasMdl.Result = true;
                    alertasMdl.Mensaje = "Se realizo correctamente el registro";
                    return alertasMdl;
                }
                else
                {
                    alertasMdl.Result = false;
                    alertasMdl.Mensaje = "Ocurrio un error con el registro intente de nuevo";
                    return alertasMdl;
                }
            }
            else
            {
                alertasMdl.Result = false;
                alertasMdl.Mensaje = "Ya hay un Usuario registrado con el mismo correo y/o telefono";
                return alertasMdl;
            }
        }        
        //registro ,responder su cuestionario
        [HttpPost]
        [Route("api/Login/RegistroCuestionario")]
        public AlertasModel RegistroCuestionario(CuestionarioModel cuestionarioModel)
        {
            var clave = Db.Clientes.Where(p => p.Id_cliente == cuestionarioModel.Cliente.Id_cliente).FirstOrDefault();
            cuestionario = new Cuestionarios
            {
                Id_cliente = cuestionarioModel.Cliente.Id_cliente,
                Clave_cuestionario = "Cues" + clave.Clave_cliente,
                Padece_enfermedad = cuestionarioModel.Padece_enfermedad,
                Medicamento_prescrito_medico = cuestionarioModel.Medicamento_prescrito_medico,
                Lesiones = cuestionarioModel.Lesiones,
                Alguna_recomendacion_lesiones = cuestionarioModel.Alguna_recomendacion_lesiones,
                Fuma = cuestionarioModel.Fuma,
                Veces_semana_fuma = cuestionarioModel.Veces_semana_fuma,
                Alcohol = cuestionarioModel.Alcohol,
                Veces_semana_alcohol = cuestionarioModel.Veces_semana_alcohol,
                Actividad_fisica = cuestionarioModel.Actividad_fisica,
                Tipo_ejercicios = cuestionarioModel.Tipo_ejercicios,
                Tiempo_dedicado = cuestionarioModel.Tiempo_dedicado,
                Horario_entreno = cuestionarioModel.Horario_entreno,
                Metas_objetivos = cuestionarioModel.MetasObjetivos,
                Compromisos = cuestionarioModel.Compromisos,
                Comentarios = cuestionarioModel.Comentarios,
                Fecha_registro = DateTime.Now
            };
            Db.Cuestionarios.Add(cuestionario);
            if (Db.SaveChanges() == 1)
            {
                alertasMdl.Id = cuestionarioModel.Cliente.Id_cliente;
                alertasMdl.Result = true;
                alertasMdl.Mensaje = "Se realizo correctamente el registro";
                return alertasMdl;
            }
            else
            {
                alertasMdl.Result = false;
                alertasMdl.Mensaje = "Ocurrio un error con el registro intente de nuevo";
                return alertasMdl;
            }
        }
        //registro de como quiere su mensualidad
        [HttpPost]
        [Route("api/Login/RegistrarMensualidad")]
        public AlertasModel RegistrarMensualidad(MensualidadModel mensualidadModel)
        {
            mensualidad = new Mensualidades()
            {
                Id_cliente = mensualidadModel.Cliente.Id_cliente,
                Id_tipo_rutina = mensualidadModel.Tiporutina.Id_tiporutina,
                Id_mes = DateTime.Now.Month,
                Id_estatus = 1,
                Id_tipo_entrenamiento = mensualidadModel.TipoEntrenamiento.Id_TipoEntrenamiento,
                Fecha_inicio = DateTime.Now,
                Fecha_fin = DateTime.Now.AddMonths(1)
            };
            Db.Mensualidades.Add(mensualidad);
            if (Db.SaveChanges() == 1)
            {
                alertasMdl.Id = mensualidad.Id_mensualidad;
                alertasMdl.Result = true;
                alertasMdl.Mensaje = "Se realizo correctamente el registro";
                return alertasMdl;
            }
            else
            {
                alertasMdl.Result = false;
                alertasMdl.Mensaje = "Ocurrio un error con el registro intente de nuevo";
                return alertasMdl;
            }
        }
        //registro de sus medidas
        [HttpPost]
        [Route("api/Login/RegistrarAntropometria")]
        public AlertasModel RegistrarAntropometria(AntropometriaModel antropometriaModel)
        {
            asesoria_antropometria = new Asesoria_antropometria()
            {
                    Id_mensualidad = antropometriaModel.Mensualidad.Id_mensualidad,
                    Peso=antropometriaModel.Peso,
                    Altura=antropometriaModel.Altura,
                    IMC=antropometriaModel.IMC,
                    Brazo_derecho_relajado=antropometriaModel.Brazoderechorelajado,
                    Brazo_derecho_fuerza=antropometriaModel.Brazoderechofuerza,
                    Brazo_izquierdo_relajado=antropometriaModel.Brazoizquierdorelajado,
                    Brazo_izquierdo_fuerza=antropometriaModel.Brazoizquierdofuerza,
                    Cintura=antropometriaModel.Cintura,
                    Cadera=antropometriaModel.Cadera,
                    Pierna_izquierda=antropometriaModel.Piernaizquierda,
                    Pierna_derecho=antropometriaModel.Piernaderecho,
                    Pantorrilla_derecha=antropometriaModel.Pantorrilladerecha,
                    Pantorrilla_izquierda=antropometriaModel.Pantorrillaizquierda,
                    Fecha_registro=DateTime.Now
            };
            Db.Asesoria_antropometria.Add(asesoria_antropometria);
            if (Db.SaveChanges() == 1)
            {
                alertasMdl.Result = true;
                alertasMdl.Mensaje = "Se realizo correctamente el registro";
                return alertasMdl;
            }
            else
            {
                alertasMdl.Result = false;
                alertasMdl.Mensaje = "Ocurrio un error con el registro intente de nuevo";
                return alertasMdl;
            }
        }
        
        //Registro cliente completo
        [HttpPost]
        [Route("api/Login/RegistroCompleto")]
        public AlertasModel RegistroCompleto(RegistroCliente Registro)
        {
            string result = "No se guardaron los datos intente de nuevo";
            string Clave = "000C-";
            string fotoperfil = "/Imagenes/Clientes/" + Clave;
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
                                cuestionario = new Cuestionarios
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
                                Db.Cuestionarios.Add(cuestionario);
                                if (Db.SaveChanges() == 1)
                                {
                                    Registro.Mensualidad.Fecha_inicio = DateTime.Now;
                                    Registro.Mensualidad.Fecha_fin = DateTime.Now.AddDays(30);
                                    int mes = DateTime.Now.Month;
                                    mensualidad = new Mensualidades
                                    {
                                        Id_cliente = clientes.Id_cliente,
                                        Id_tipo_rutina = Registro.Mensualidad.Tiporutina.Id_tiporutina,
                                        Id_mes = mes,
                                        Id_estatus = 1,
                                        Id_tipo_entrenamiento = Registro.Mensualidad.TipoEntrenamiento.Id_TipoEntrenamiento,
                                        Fecha_inicio = Registro.Mensualidad.Fecha_inicio,
                                        Fecha_fin = Registro.Mensualidad.Fecha_fin
                                    };
                                    Db.Mensualidades.Add(mensualidad);
                                    if (Db.SaveChanges() == 1)
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
                                            string ruta = HostingEnvironment.MapPath("~/Imagenes/Clientes");
                                            ruta = Path.Combine(ruta + @"\" + Clave);
                                            DirectoryInfo di = Directory.CreateDirectory(ruta);
                                            Ubicacion = Clave;
                                            IdCliente = clientes.Id_cliente.ToString();
                                            IdMedidas = asesoria_antropometria.Id.ToString();
                                            Imagenes imagenes = new Imagenes
                                            {
                                                ImagenFrontal = Registro.Imagenes.ImagenFrontal,
                                                ImagenPerfilCuenta = Registro.Imagenes.ImagenPerfilCuenta,
                                                ImagenPerfil = Registro.Imagenes.ImagenPerfil,
                                                ImagenPosterior = Registro.Imagenes.ImagenPosterior,
                                            };
                                            SolicitudRegistro(Registro);
                                            AlertasModel alert = new AlertasModel();
                                            alert = UpdateImagenes(imagenes,Ubicacion, IdCliente, IdMedidas);
                                            if (alert.Mensaje == "True")
                                            {                                                
                                                result = "Se registro correcto";
                                            }
                                            else
                                            {
                                                result = "Se registro correcto falto imagens";
                                            }
                                            alertasMdl.Result = true;
                                        }
                                        else
                                        {                                            
                                            result = "Ocurrio un error al guardar tus medidas reintenta de nuevo y verifica tus datos";
                                            Db.Mensualidades.Remove(mensualidad);
                                            Db.SaveChanges();
                                            Db.Cuestionarios.Remove(cuestionario);
                                            Db.SaveChanges();
                                            Db.Clientes.Remove(clientes);
                                            Db.SaveChanges();
                                            alertasMdl.Result = false;
                                        }
                                    }
                                    else
                                    {                                        
                                        result = "Ocurrio un error al guardar tu mensualidad reintenta de nuevo y verifica tus datos";
                                        Db.Cuestionarios.Remove(cuestionario);
                                        Db.SaveChanges();
                                        Db.Clientes.Remove(clientes);
                                        Db.SaveChanges();
                                        alertasMdl.Result = false;
                                    }
                                }
                                else
                                {                                    
                                    result = "Ocurrio un error al guardar tu cuestionario reintenta de nuevo y verifica tus datos";
                                    Db.Clientes.Remove(clientes);
                                    Db.SaveChanges();
                                    alertasMdl.Result = false;
                                }
                            }
                            else
                            {                                
                                result = "Ocurrio un error al guardar los datos intente de nuevo por favor";
                                Db.Clientes.Remove(clientes);
                                Db.SaveChanges();
                                alertasMdl.Result = false;
                            }
                        }
                        else
                        {                            
                            result = "Ocurrio un error al guardar tus datos personales reintenta de nuevo y verifica tus datos";
                            alertasMdl.Result = false;
                        }
                    }
                    else
                    {                                              
                        result = "Ya hay un correo y/o celular registrado por favor intenta otro";
                        alertasMdl.Result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                alertasMdl.Result = false;
                alertasMdl.Mensaje = ex.Message;
                result = ex.Message;
            }
            alertasMdl.Mensaje = result;
            return alertasMdl;
        }

        //Subimos las imagenes
        public AlertasModel UpdateImagenes(Imagenes imagenes, string ubicacion ,string idcliente,string medidas)
        {            
            alertasMdl.Result = true;
            alertasMdl.Mensaje = "1";
            try
            {
                int clienteid = 0, medidasid = 0;
                clienteid = int.Parse(idcliente);
                medidasid = int.Parse(medidas);
                clientes = Db.Clientes.Where(y => y.Id_cliente == clienteid).FirstOrDefault();
                asesoria_antropometria = Db.Asesoria_antropometria.Where(y => y.Id == medidasid).FirstOrDefault();
                if (!string.IsNullOrEmpty(imagenes.ImagenPerfilCuenta))
                {
                    byte[] Foto_perfil = Convert.FromBase64String(imagenes.ImagenPerfilCuenta);
                    clientes.Foto_perfil = clientes.Foto_perfil + "/" + "Foto_cuenta.jpg";
                    Db.SaveChanges();
                    string rutaimagenPerfilCuenta = HostingEnvironment.MapPath("~/Imagenes/Clientes/" + ubicacion + "/" + "Foto_cuenta.jpg");
                    using (var ms = new MemoryStream(Foto_perfil, 0, Foto_perfil.Length))
                    {
                        Image image = Image.FromStream(ms, true);
                        image.Save(rutaimagenPerfilCuenta);
                    }
                    alertasMdl.Result = true;
                    alertasMdl.Mensaje = "True";
                }
                else
                {
                    clientes.Foto_perfil = clientes.Foto_perfil + "/";
                    Db.SaveChanges();
                }
                if (!string.IsNullOrEmpty(imagenes.ImagenFrontal))
                {
                    byte[] Foto_frontal = Convert.FromBase64String(imagenes.ImagenFrontal);
                    asesoria_antropometria.Foto_frontal = asesoria_antropometria.Foto_frontal + "/" + "Foto_frontal.jpg";
                    Db.SaveChanges();
                    string rutaFoto_frontal= HostingEnvironment.MapPath("~/Imagenes/Clientes/" + ubicacion + "/" + "Foto_frontal.jpg");
                    using (var ms = new MemoryStream(Foto_frontal, 0, Foto_frontal.Length))
                    {
                        Image image = Image.FromStream(ms, true);
                        image.Save(rutaFoto_frontal);
                    }
                    alertasMdl.Result = true;
                    alertasMdl.Mensaje = "True";
                }
                if (!string.IsNullOrEmpty(imagenes.ImagenPerfil))
                {
                    byte[] Foto_perfil = Convert.FromBase64String(imagenes.ImagenPerfil);
                    asesoria_antropometria.Foto_perfil = asesoria_antropometria.Foto_perfil + "/" + "Foto_lateral.jpg";
                    Db.SaveChanges();
                    string rutafotolateral = HostingEnvironment.MapPath("~/Imagenes/Clientes/" + ubicacion + "/" + "Foto_lateral.jpg");
                    using (var ms = new MemoryStream(Foto_perfil, 0, Foto_perfil.Length))
                    {
                        Image image = Image.FromStream(ms, true);
                        image.Save(rutafotolateral);
                    }
                    alertasMdl.Result = true;
                    alertasMdl.Mensaje = "True";
                }
                if (!string.IsNullOrEmpty(imagenes.ImagenPosterior))
                {
                    byte[] Foto_posterior = Convert.FromBase64String(imagenes.ImagenPosterior);
                    asesoria_antropometria.Foto_posterior = asesoria_antropometria.Foto_posterior + "/" + "Foto_posterior.jpg"; 
                    Db.SaveChanges();
                    string fotoposterior=HostingEnvironment.MapPath("~/Imagenes/Clientes/" + ubicacion + "/" + "Foto_posterior.jpg");
                    using (var ms = new MemoryStream(Foto_posterior, 0, Foto_posterior.Length))                    
                    {
                        Image image = Image.FromStream(ms, true);
                        image.Save(fotoposterior);
                    }
                    alertasMdl.Result = true;
                    alertasMdl.Mensaje = "True";
                    
                }
            }
            catch (Exception ex)
            {
                alertasMdl.Mensaje = ex.Message;
            }
            return alertasMdl;
        }

        //Enviar correo
        public AlertasModel SolicitudRegistro(RegistroCliente registro)
        {
            try
            {
                string training = Db.Tipo_entrenamientos.Where(y => y.Id_tipo_entrenamiento == registro.Mensualidad.TipoEntrenamiento.Id_TipoEntrenamiento).Select(y => y.Tipo_entrenamiento).FirstOrDefault();
                string rutine = Db.Tipo_rutinas.Where(y => y.Id_tipo_rutina == registro.Mensualidad.Tiporutina.Id_tiporutina).Select(y => y.Descripcion).FirstOrDefault();
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
                string plantilla = HostingEnvironment.MapPath("~/Plantillas/SolicitudRegistro.html");
                string succes = "Se envio tu solicitud de registro a tu entrenador";
                string correomio = "paradavalerio@gmail.com";
                string asunto = "Solicitud de rutina";
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
    }
   
}