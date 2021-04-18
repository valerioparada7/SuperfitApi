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

namespace SuperfitApi.Controllers
{
    public class LoginController : ApiController
    {
        #region Variables 
        public SuperfitEntities Db;
        public Clientes clientes;
        public Cuestionario cuestionario;
        public Mensualidad mensualidad;
        public Asesoria_antropometria asesoria_antropometria;
        //modelos
        public ClientesModel clientesMdl;
        public CuestionarioModel cuestionarioMdl;
        public MensualidadModel mensualidadMdl;
        public AntropometriaModel asesoria_antropometriaMdl;
        public AlertasModel alertasModel;
        #endregion       
        public LoginController()
        {
            Db = new SuperfitEntities();
            clientes = new Clientes();
            cuestionario = new Cuestionario();
            mensualidad = new Mensualidad();
            asesoria_antropometria = new Asesoria_antropometria();
            //modelos
            clientesMdl = new ClientesModel();
            cuestionarioMdl = new CuestionarioModel();
            mensualidadMdl = new MensualidadModel();
            asesoria_antropometriaMdl = new AntropometriaModel();
            alertasModel = new AlertasModel();
        }        
        //Loguearse
        [HttpGet]
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
            }
            
            
            if (clientesMdl.Id_cliente != 0)
            {
                var list = Db.Mensualidad.Where(p => p.Id_cliente  == clientesMdl.Id_cliente).ToList();
                var mensualidad = list.OrderByDescending(p => p.Fecha_fin).FirstOrDefault();
                if (mensualidad != null)
                {
                    int id = mensualidad.Id_mensualidad;
                    mensualidadMdl = (from m in Db.Mensualidad   
                                      join t in Db.Tipo_rutina
                                      on m.Id_tipo_rutina equals t.Id_tipo_rutina
                                      join te in Db.Tipo_entrenamiento
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
                                              Tipo_entrenamiento = te.Tipo_entrenamientos 
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
                    alertasModel.Id = clientes.Id_cliente;
                    alertasModel.Result = true;
                    alertasModel.Mensaje = "Se realizo correctamente el registro";
                    return alertasModel;
                }
                else
                {
                    alertasModel.Result = false;
                    alertasModel.Mensaje = "Ocurrio un error con el registro intente de nuevo";
                    return alertasModel;
                }
            }
            else
            {
                alertasModel.Result = false;
                alertasModel.Mensaje = "Ya hay un Usuario registrado con el mismo correo y/o telefono";
                return alertasModel;
            }
        }        
        //registro ,responder su cuestionario
        [HttpPost]
        [Route("api/Login/RegistroCuestionario")]
        public AlertasModel RegistroCuestionario(CuestionarioModel cuestionarioModel)
        {
            var clave = Db.Clientes.Where(p => p.Id_cliente == cuestionarioModel.Cliente.Id_cliente).FirstOrDefault();
            cuestionario = new Cuestionario
            {
                Id_cliente = cuestionarioModel.Cliente.Id_cliente,
                Clave_cuestionario = "Cues" + clave.Clave_cliente,
                Padece_enfermedad = cuestionarioModel.Padece_enfermedad,
                Medicamento_prescrito_medico = cuestionarioModel.Medicamento_prescrito_medico,
                lesiones = cuestionarioModel.lesiones,
                Alguna_recomendacion_lesiones = cuestionarioModel.Alguna_recomendacion_lesiones,
                Fuma = cuestionarioModel.Fuma,
                Veces_semana_fuma = cuestionarioModel.Veces_semana_fuma,
                Alcohol = cuestionarioModel.Alcohol,
                Veces_semana_alcohol = cuestionarioModel.Veces_semana_alcohol,
                Actividad_fisica = cuestionarioModel.Actividad_fisica,
                Tipo_ejercicios = cuestionarioModel.Tipo_ejercicios,
                Tiempo_dedicado = cuestionarioModel.Tiempo_dedicado,
                Horario_entreno = cuestionarioModel.Horario_entreno,
                MetasObjetivos = cuestionarioModel.MetasObjetivos,
                Compromisos = cuestionarioModel.Compromisos,
                Comentarios = cuestionarioModel.Comentarios,
                Fecha_registro = DateTime.Now
            };
            Db.Cuestionario.Add(cuestionario);
            if (Db.SaveChanges() == 1)
            {
                alertasModel.Id = cuestionarioModel.Cliente.Id_cliente;
                alertasModel.Result = true;
                alertasModel.Mensaje = "Se realizo correctamente el registro";
                return alertasModel;
            }
            else
            {
                alertasModel.Result = false;
                alertasModel.Mensaje = "Ocurrio un error con el registro intente de nuevo";
                return alertasModel;
            }
        }
        //registro de como quiere su mensualidad
        [HttpPost]
        [Route("api/Login/RegistrarMensualidad")]
        public AlertasModel RegistrarMensualidad(MensualidadModel mensualidadModel)
        {
            mensualidad = new Mensualidad()
            {
                Id_cliente = mensualidadModel.Cliente.Id_cliente,
                Id_tipo_rutina = mensualidadModel.Tiporutina.Id_tiporutina,
                Id_mes = DateTime.Now.Month,
                Id_estatus = 1,
                Id_tipo_entrenamiento = mensualidadModel.TipoEntrenamiento.Id_TipoEntrenamiento,
                Fecha_inicio = DateTime.Now,
                Fecha_fin = DateTime.Now.AddMonths(1)
            };
            Db.Mensualidad.Add(mensualidad);
            if (Db.SaveChanges() == 1)
            {
                alertasModel.Id = mensualidad.Id_mensualidad;
                alertasModel.Result = true;
                alertasModel.Mensaje = "Se realizo correctamente el registro";
                return alertasModel;
            }
            else
            {
                alertasModel.Result = false;
                alertasModel.Mensaje = "Ocurrio un error con el registro intente de nuevo";
                return alertasModel;
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
                alertasModel.Result = true;
                alertasModel.Mensaje = "Se realizo correctamente el registro";
                return alertasModel;
            }
            else
            {
                alertasModel.Result = false;
                alertasModel.Mensaje = "Ocurrio un error con el registro intente de nuevo";
                return alertasModel;
            }
        }
    }
}