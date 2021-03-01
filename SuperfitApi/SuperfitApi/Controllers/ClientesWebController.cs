using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SuperfitApi.Models;

namespace SuperfitApi.Controllers
{
    public class ClientesWebController : Controller
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
        public AlertasModel alertasModel;
        //listas de modelos
        public List<DetallerutinaModel> listdetallerutinaMdl;
        #endregion       

        public ClientesWebController()
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
            string Id = Session["Id_Cliente"].ToString();
            int IdCliente= int.Parse(Id);

            clientesMdl = (from c in Db.Clientes
                           where c.Id_Cliente == IdCliente
                           select new ClientesModel()
                           {
                               Id_Cliente = c.Id_Cliente,
                               Nombres = c.Nombres,
                               Fotoperfil = c.Fotoperfil
                           }).FirstOrDefault();

            var list = Db.Mensualidad.Where(p => p.Id_Cliente == IdCliente).ToList();
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
                                          Id_estatus = es.Id_estatus,
                                          Descripcion = es.Descripcion
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
                        Nombres = clientesMdl.Nombres,
                        Fotoperfil = clientesMdl.Fotoperfil
                    };
                    return View(mensualidadMdl);
                }
                else
                {
                    mensualidadMdl = new MensualidadModel
                    {
                        Cliente = new ClientesModel(),                        
                    };
                    ViewBag.TipoRutina = "No asignado";
                    ViewBag.TipoEntrenamiento = "No asignado";
                    ViewBag.FechaI = "No asignado";
                    ViewBag.FechaF = "No asignado";
                    clientesMdl.Validar = true;
                    mensualidadMdl.Cliente = clientesMdl;
                    return View(mensualidadMdl);
                }
            }
            else
            {                
                mensualidadMdl = new MensualidadModel
                {
                    Cliente = new ClientesModel(),                    
                };
                ViewBag.TipoRutina = "No asignado";
                ViewBag.TipoEntrenamiento = "No asignado";
                ViewBag.FechaI = "No asignado";
                ViewBag.FechaF = "No asignado";
                clientesMdl.Validar = true;
                mensualidadMdl.Cliente = clientesMdl;
                return View(mensualidadMdl);
            }
            
        }

        public ActionResult Rutinas()
        {
            return View();
        }
        //Obtener los sets a realizar por dia
        [HttpGet]
        public ActionResult GetDetalleRutinaSets()
        {
            int IdMensualidad=0,  IdEstatusMes=0,  IdDIa=0;
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

            return View(listdetallerutinaMdl);
        }

        //Obtener ejercicios por set
        [HttpGet]
        public ActionResult GetDetalleRutinaEjercicios()
        {
            int IdMensualidad=0,  IdEstatusMes=0, IdDIa=0, TipoSet=0;
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
                                        Id_detallerutina = d.Id_detallerutina,
                                        Ejercicios = new EjerciciosModel
                                        {
                                            Id_ejercicio = e.Id_ejercicio,
                                            Clave_ejercicio = e.Clave_ejercicio,
                                            Ejercicio = e.Ejercicio,
                                            Descripcion = e.Descripcion,
                                            Posicion = e.Posicion,
                                            ubicacion_imagen = e.ubicacion_imagen
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

            return View(listdetallerutinaMdl);
        }

    }
}