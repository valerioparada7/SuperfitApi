using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SuperfitApi.Models;

namespace SuperfitApi.Controllers
{
    public class ClientesController : ApiController
    {
//config.Formatters.Remove(config.Formatters.XmlFormatter); eso ta en webconfig de app
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
        public DetallerutinaModel detallerutinaMdl;
        //listas de modelos
        public List<DetallerutinaModel> listdetallerutinaMdl;
        #endregion
        public ClientesController()
        {
            Db = new SuperflyfitEntities();
            clientes = new Clientes();
            //modelos
            clientesMdl = new ClientesModel();
            cuestionarioMdl = new CuestionarioModel();
            mensualidadMdl = new MensualidadModel();
            asesoria_antropometriaMdl = new AntropometriaModel();            
            detallerutinaMdl = new DetallerutinaModel();
            //listas de modelos
            listdetallerutinaMdl = new List<DetallerutinaModel>();
        }

        //Obtener los datos para su perfil del cliente
        [HttpGet]
        public Clientes GetCliente(int IdCliente)
        {
            clientes = Db.Clientes.Where(p => p.Id_Cliente == IdCliente).FirstOrDefault();
            return clientes;
        }

        [HttpPut]
        public bool UpdateCliente(ClientesModel ClientesMdl)
        {
            int IdCliente = ClientesMdl.Id_Cliente;
            clientes = Db.Clientes.Where(p => p.Id_Cliente == IdCliente).FirstOrDefault();
            clientes.Nombres = ClientesMdl.Nombres;
            clientes.Apellido_Paterno = ClientesMdl.Apellido_Paterno;
            clientes.Apellido_Materno = ClientesMdl.Apellido_Materno;
            clientes.Edad = ClientesMdl.Edad;
            clientes.Telefono = ClientesMdl.Telefono;
            clientes.Correo_electronico = ClientesMdl.Correo_electronico;
            clientes.Apodo = ClientesMdl.Apodo;
            clientes.Contraseña = ClientesMdl.Contraseña;
            if (Db.SaveChanges() == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //RUTINAS PERSONALIZADAS IRAN JUNTO EL CLIENTE PORQUE TODO ES RESPECTO A SU ID

        //Obtener los sets a realizar por dia
        [HttpGet]
        public List<DetallerutinaModel> GetDetalleRutinaSets(int IdMensualidad,int IdEstatusMes,int IdDIa)
        {
            listdetallerutinaMdl = (from d in Db.Detalle_rutina
                                    join m in Db.Mensualidad
                                   on d.Id_mensualidad equals m.Id_mensualidad
                                    where m.Id_mensualidad== IdMensualidad && m.Id_estatus == IdEstatusMes && d.Id_dia == IdDIa
                                    select new DetallerutinaModel()
                                    {                                        
                                        Tipo_set = d.Tipo_set                                        
                                    }).Distinct().ToList();

            foreach(DetallerutinaModel detalle in listdetallerutinaMdl)
            {
                if (detalle.Tipo_set == 1)
                {
                    detalle.TipoSet = "Primer set";
                }
                else if (detalle.Tipo_set == 2)
                {
                    detalle.TipoSet = "Segundo set";
                }
                else if (detalle.Tipo_set == 3)
                {
                    detalle.TipoSet = "Tercer set";
                }
                else if (detalle.Tipo_set == 4)
                {
                    detalle.TipoSet = "Cuarto set";
                }
                else
                {
                    detalle.TipoSet = "Ultimo set";
                }
            }
            return listdetallerutinaMdl;
        }

        //Obtener ejercicios por set
        [HttpGet]
        public List<DetallerutinaModel> GetDetalleRutinaEjercicios(int IdMensualidad, int IdEstatusMes, int IdDIa,int TipoSet)
        {
            listdetallerutinaMdl = (from d in Db.Detalle_rutina
                                    join e in Db.Ejercicios
                                    on d.Id_ejercicio equals e.Id_ejercicio
                                    join m in Db.Mensualidad
                                    on d.Id_mensualidad equals m.Id_mensualidad
                                    where  m.Id_estatus == IdEstatusMes && d.Id_dia == IdDIa && d.Tipo_set== TipoSet
                                    select new DetallerutinaModel()
                                    {      
                                       Id_detallerutina = d.Id_detallerutina,   
                                       Ejercicios = new EjerciciosModel
                                       {
                                           Id_ejercicio=e.Id_ejercicio,
                                           Clave_ejercicio = e.Clave_ejercicio,
                                           Ejercicio= e.Ejercicio,
                                           Descripcion = e.Descripcion,
                                           Posicion = e.Posicion,
                                           ubicacion_imagen=e.ubicacion_imagen
                                       }
                                    }).ToList();
            return listdetallerutinaMdl;
        }
    }
}