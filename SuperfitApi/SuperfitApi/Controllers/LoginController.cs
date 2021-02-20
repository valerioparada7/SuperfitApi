﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SuperfitApi.Models;

namespace SuperfitApi.Controllers
{
    public class LoginController : ApiController
    {
        #region Variables 
        public SuperflyfitEntities Db;
        public Clientes clientes;
        public Cuestionario cuestionario;
        public Mensualidad mensualidad;
        public Asesoria_Antropometria asesoria_antropometria;
        //modelos
        public ClientesModel clientesMdl;
        public CuestionarioModel cuestionarioMdl;
        public MensualidadModel mensualidadMdl;
        public AntropometriaModel asesoria_antropometriaMdl;
        #endregion

        public LoginController()
        {
            Db = new SuperflyfitEntities();
            clientes = new Clientes();
            cuestionario = new Cuestionario();
            mensualidad = new Mensualidad();
            asesoria_antropometria = new Asesoria_Antropometria();
            //modelos
            clientesMdl = new ClientesModel();
            cuestionarioMdl = new CuestionarioModel();
            mensualidadMdl = new MensualidadModel();
            asesoria_antropometriaMdl = new AntropometriaModel();
        }        
        //Loguearse
        [HttpGet]
        public MensualidadModel Login(string User,string Pass)
        {            
            clientesMdl = (from c in Db.Clientes
                           where c.Contraseña == Pass && c.Clave_Cliente == User
                           select new ClientesModel()
                           {
                               Id_Cliente = c.Id_Cliente,
                               Nombres = c.Nombres,
                           }).FirstOrDefault();

            if (clientesMdl != null)
            {
                var list = Db.Mensualidad.Where(p => p.Id_Cliente == clientesMdl.Id_Cliente).ToList();
                var mensualidad = list.OrderByDescending(p => p.Fecha_fin).FirstOrDefault();
                if (mensualidad != null)
                {
                    int id = mensualidad.Id_mensualidad;
                    mensualidadMdl = (from m in Db.Mensualidad   
                                      join t in Db.Tiporutina
                                      on m.Id_tiporutina equals t.Id_tiporutina
                                      join te in Db.TipoEntrenamiento
                                      on m.Id_TipoEntrenamiento equals te.Id_TipoEntrenamiento
                                      join mes in Db.Meses
                                      on m.Id_mes equals mes.Id_mes
                                      join es in Db.Estatus
                                      on m.Id_estatus equals es.Id_estatus                                      
                                      where m.Id_mensualidad == id
                                      select new MensualidadModel()
                                      {
                                          Id_mensualidad = m.Id_mensualidad,                                          
                                          Tiporutina = new TiporutinaModel
                                          {
                                              Id_tiporutina = t.Id_tiporutina,
                                              Tipo = t.Tipo
                                          },
                                          TipoEntrenamiento = new TipoentrenamientoModel
                                          {
                                              Id_TipoEntrenamiento = (int)te.Id_TipoEntrenamiento,
                                              Clave_Entrenamiento = te.Clave_Entrenamiento,
                                              Tipo_entrenamiento = te.Tipo_entrenamiento
                                          },
                                          Mes = new MesesModel
                                          {
                                              Id_mes = mes.Id_mes,
                                              Clave_mes = mes.Clave_mes,
                                              Mes = mes.Mes
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
                            Id_Cliente = clientesMdl.Id_Cliente,
                            Validar = true,
                            Nombres = clientesMdl.Nombres                            
                        };                                                                                                             
                        return mensualidadMdl;
                    }
                    else
                    {
                        mensualidadMdl = new MensualidadModel
                        {
                            Cliente = new ClientesModel()
                        };
                        clientesMdl.Id_Cliente = clientesMdl.Id_Cliente;
                        clientesMdl.Validar = true;
                        clientesMdl.Nombres = clientesMdl.Nombres;
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
                    clientesMdl.Id_Cliente = clientesMdl.Id_Cliente;
                    clientesMdl.Validar = true;
                    clientesMdl.Nombres = clientesMdl.Nombres;
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
        //Crear cuenta registrando el cliente
        [HttpPost]
        [Route("api/Login/RegistrarCliente")]
        public Boolean RegistrarCliente(ClientesModel clientesModel)
        {
            string Clave = "";
            Clave = clientesModel.Nombres.Substring(0, 3) + clientesModel.Apellido_Paterno.Substring(0, 3) +
                    clientesModel.Apellido_Materno.Substring(0, 3);
            clientes = new Clientes
            {
                Clave_Cliente = Clave,
                Nombres = clientesModel.Nombres,
                Apellido_Paterno = clientesModel.Apellido_Paterno,
                Apellido_Materno = clientesModel.Apellido_Materno,
                Edad = clientesModel.Edad,
                Telefono = clientesModel.Telefono,
                Correo_electronico = clientesModel.Correo_electronico,
                Apodo = clientesModel.Apodo,
                Contraseña = clientesModel.Contraseña,
                Estado = true
            };
            Db.Clientes.Add(clientes);
            if (Db.SaveChanges() == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }        
        //registro ,responder su cuestionario
        [HttpPost]
        [Route("api/Login/RegistroCuestionario")]
        public Boolean RegistroCuestionario(CuestionarioModel cuestionarioModel)
        {
            cuestionario = new Cuestionario
            {
                Id_Cliente = cuestionarioModel.Cliente.Id_Cliente,
                Clave_cuestionario = cuestionarioModel.Clave_cuestionario,
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
                return true; 
            }
            else
            {
                return false;
            }
        }
        //registro de como quiere su mensualidad
        [HttpPost]
        [Route("api/Login/RegistrarMensualidad")]
        public Boolean RegistrarMensualidad(MensualidadModel mensualidadModel)
        {
            mensualidad = new Mensualidad()
            {
                Id_Cliente = mensualidadModel.Cliente.Id_Cliente,
                Id_tiporutina = mensualidadModel.Tiporutina.Id_tiporutina,
                Id_mes = mensualidadModel.Mes.Id_mes,
                Id_estatus = mensualidadModel.Estatus.Id_estatus,
                Id_TipoEntrenamiento = mensualidadModel.TipoEntrenamiento.Id_TipoEntrenamiento,
                Fecha_inicio = DateTime.Now,
                Fecha_fin = DateTime.Now.AddMonths(1)
            };
            Db.Mensualidad.Add(mensualidad);
            if (Db.SaveChanges() == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //registro de sus medidas
        [HttpPost]
        [Route("api/Login/RegistrarAntropometria")]
        public Boolean RegistrarAntropometria(AntropometriaModel antropometriaModel)
        {
            asesoria_antropometria = new Asesoria_Antropometria()
            {
                    Id_mensualidad = antropometriaModel.Mensualidad.Id_mensualidad,
                    Peso=antropometriaModel.Peso,
                    Altura=antropometriaModel.Altura,
                    IMC=antropometriaModel.IMC,
                    Brazoderechorelajado=antropometriaModel.Brazoderechorelajado,
                    Brazoderechofuerza=antropometriaModel.Brazoderechofuerza,
                    Brazoizquierdorelajado=antropometriaModel.Brazoizquierdorelajado,
                    Brazoizquierdofuerza=antropometriaModel.Brazoizquierdofuerza,
                    Cintura=antropometriaModel.Cintura,
                    Cadera=antropometriaModel.Cadera,
                    Piernaizquierda=antropometriaModel.Piernaizquierda,
                    Piernaderecho=antropometriaModel.Piernaderecho,
                    Pantorrilladerecha=antropometriaModel.Pantorrilladerecha,
                    Pantorrillaizquierda=antropometriaModel.Pantorrillaizquierda,
                    Fecha_registro=antropometriaModel.Fecha_registro,
            };
            Db.Asesoria_Antropometria.Add(asesoria_antropometria);
            if (Db.SaveChanges() == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}