using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SuperfitApi.Models.Entity;
using SuperfitApi.Models;
using SuperfitApi.Autetication;

namespace SuperfitApi.Controllers
{
    [Validation]
    public class ClientesWebController : Controller
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
        //listas de modelos
        public List<DetallerutinaModel> listdetallerutinaMdl;
        #endregion       

        public ClientesWebController()
        {
            Db = new SuperfitEntities ();
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
            //listas de modelos
            listdetallerutinaMdl = new List<DetallerutinaModel>();
        }
        // GET: ClientesWeb
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Perfil()
        {
            try
            {            
                string Id = Session["Id_Cliente"].ToString();
                int IdCliente = int.Parse(Id);

                clientesMdl = (from c in Db.Clientes
                               where c.Id_cliente == IdCliente
                               select new ClientesModel()
                               {
                                   Id_cliente = c.Id_cliente,
                                   Nombres = c.Nombres,
                                   Foto_perfil = c.Foto_perfil
                               }).FirstOrDefault();

                var list = Db.Mensualidad.Where(p => p.Id_cliente == IdCliente).ToList();
                var mensualidad = list.OrderByDescending(p => p.Fecha_fin).FirstOrDefault();
                if (mensualidad != null)
                {
                    int id = mensualidad.Id_mensualidad;
                    mensualidadMdl = (from m in Db.Mensualidad
                                      join t in Db.Tipo_rutina
                                      on m.Id_tipo_rutina equals t.Id_tipo_rutina
                                      join te in Db.Tipo_entrenamiento
                                      on m.Id_tipo_entrenamiento equals te.Id_tipo_entrenamiento
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
                                              Id_mes = mes.Id_mes,
                                              Clave_mes = mes.Clave_mes,
                                              Mes = mes.Mes
                                          },
                                          Estatus = new EstatusModel
                                          {
                                              Id_estatus = es.Id_estatus,
                                              Descripcion = es.Descripcion
                                          },
                                          Fecha_inicio = (DateTime)m.Fecha_inicio,
                                          Fecha_fin = (DateTime)m.Fecha_fin,
                                      }).FirstOrDefault();

                    Session["Mensualdiad"] = mensualidadMdl.Id_mensualidad;
                    Session["EstatusMensualdiad"] = mensualidadMdl.Estatus.Id_estatus;
                    mensualidadMdl.Cliente = new ClientesModel();
                    mensualidadMdl.Cliente = clientesMdl;
                    mensualidadMdl.Fechafin = mensualidadMdl.Fecha_fin.ToString("dd/MM/yyyy");
                    mensualidadMdl.Fechainicio = mensualidadMdl.Fecha_inicio.ToString("dd/MM/yyyy");
                }
                else
                {
                    mensualidadMdl = new MensualidadModel()
                    {
                        Cliente = new ClientesModel(),
                        Tiporutina = new TiporutinaModel(),
                        TipoEntrenamiento = new TipoentrenamientoModel()
                    };
                    Session["Mensualdiad"] = 0;
                    mensualidadMdl.Cliente = clientesMdl;
                    mensualidadMdl.Tiporutina.Tipo = "No asignado";
                    mensualidadMdl.TipoEntrenamiento.Tipo_entrenamiento = "No asignado";
                    mensualidadMdl.Fechafin = "Sin fecha asignada";
                    mensualidadMdl.Fechainicio = "Sin fecha asignada";
                }
                string fto = mensualidadMdl.Cliente.Foto_perfil;
                ViewBag.Mes = mensualidadMdl;
                return View();
            }
            catch (Exception ex)
            {
                return View(ex.Message);
            }
        }        

        public ActionResult Rutinas()
        {
            return View();
        }
        //Obtener los sets a realizar por dia
        [HttpGet]
        public JsonResult GetDetalleRutinaSets(int IdMensualidad = 0, int IdEstatusMes = 0,int IdDIa=0)
        {            
            listdetallerutinaMdl = (from d in Db.Detalle_rutina
                                    join m in Db.Mensualidad
                                   on d.Id_mensualidad equals m.Id_mensualidad
                                    where m.Id_mensualidad == IdMensualidad && m.Id_estatus == IdEstatusMes && d.Id_dia == IdDIa
                                    select new DetallerutinaModel()
                                    {
                                        Tipo_set = d.Tipo_set
                                    }).Distinct().ToList();
            foreach (DetallerutinaModel detalle in listdetallerutinaMdl)
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
            return Json(listdetallerutinaMdl,JsonRequestBehavior.AllowGet);
        }

        //Obtener ejercicios por set
        [HttpGet]
        public JsonResult GetDetalleRutinaEjercicios(int IdMensualidad = 0, int IdEstatusMes = 0, int IdDIa = 0, int TipoSet = 0)
        {
            
            listdetallerutinaMdl = (from d in Db.Detalle_rutina
                                    join e in Db.Ejercicios
                                    on d.Id_ejercicio equals e.Id_ejercicio
                                    join m in Db.Mensualidad
                                    on d.Id_mensualidad equals m.Id_mensualidad
                                    join r in Db.Rutinas
                                    on d.Id_rutina equals r.Id_rutina
                                    where m.Id_mensualidad == IdMensualidad && m.Id_estatus == IdEstatusMes && d.Id_dia == IdDIa && d.Tipo_set == TipoSet
                                    select new DetallerutinaModel()
                                    {
                                        Id_detallerutina = d.Id_detalle_rutina,
                                        Ejercicios = new EjerciciosModel
                                        {
                                            Id_ejercicio = e.Id_ejercicio,
                                            Clave_ejercicio = e.Clave_ejercicio,
                                            Ejercicio = e.Ejercicio,
                                            Descripcion = e.Descripcion,
                                            Posicion = e.Posicion,
                                            Ubicacion_imagen = e.Ubicacion_imagen
                                        },
                                        Rutinas = new RutinasModel
                                        {
                                            Id_rutina = r.Id_rutina,
                                            Clave_rutina = r.Clave_rutina,
                                            Descripcion = r.Descripcion
                                        },
                                        Repeticiones = d.Repeticiones,
                                        Series = d.Series
                                    }).ToList();

            return Json(listdetallerutinaMdl, JsonRequestBehavior.AllowGet);
        }

    }
}