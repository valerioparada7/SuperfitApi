using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SuperfitApi.Models.Entity;
using SuperfitApi.Models;
using System.Web.Mvc;
using System.IO;
using SuperfitApi.Autetication;
using System.Globalization;
using System.Net.Mail;

namespace SuperfitApi.Controllers.AdministracionWeb.CatalogosWeb
{
    [Validation]
    public class AdministracionWebHomeController : Controller
    {
        #region Variables
        public SuperfitEntities Db;
        public Clientes clientes;
        public Cuestionarios cuestionario;
        public Mensualidades mensualidad;
        public Asesoria_antropometria asesoria_antropometria;
        public Tipo_rutinas tipo_Rutina;
        public Tipo_entrenamientos tipo_Entrenamiento;
        public Ejercicios ejercicios;
        public Estatus estatus;
        public Rutinas rutina;
        public Detalle_rutinas detalle_Rutina;
        public List<Detalle_rutinas> listdetalle_Rutina;
        public EnvioNotificaciones envio;
        //modelos
        public AlertasModel alertasMdl;

        public ClientesModel clientesMdl;
        public CuestionarioModel cuestionarioMdl;
        public MensualidadModel mensualidadMdl;
        public AntropometriaModel asesoria_antropometriaMdl;
        public TiporutinaModel tiporutinaMdl;
        public TipoentrenamientoModel tipoentrenamientoMdl;
        public EjerciciosModel ejerciciosMdl;
        public EstatusModel estatusMdl;
        public AntropometriaModel antropometriaMdl;
        public RutinasModel rutinasMdl;
        public DetallerutinaModel detallerutinaMdl;


        //listas 
        public List<ClientesModel> listclientesMdl;
        public List<MensualidadModel> listmensualidadMdl;
        public List<TiporutinaModel> listtiporutinaMdl;
        public List<TipoentrenamientoModel> listtipoentrenamientoMdl;
        public List<EjerciciosModel> listejerciciosMdl;
        public List<EstatusModel> listestatusMdl;
        public List<AntropometriaModel> listantropometriaMdl;
        public List<RutinasModel> listrutinasMdl;
        public List<MesesModel> listmesesMdl;
        public List<DetallerutinaModel> listdetallerutinaMdl;

        #endregion
        public AdministracionWebHomeController()
        {
            Db = new SuperfitEntities();
            clientes = new Clientes();
            cuestionario = new Cuestionarios();
            mensualidad = new Mensualidades();
            asesoria_antropometria = new Asesoria_antropometria();
            tipo_Rutina = new Tipo_rutinas();
            tipo_Entrenamiento = new Tipo_entrenamientos();
            ejercicios = new Ejercicios();
            rutina = new Rutinas();
            detalle_Rutina = new Detalle_rutinas();
            listdetalle_Rutina = new List<Detalle_rutinas>();
            envio = new EnvioNotificaciones();
            //modelos
            clientesMdl = new ClientesModel();
            cuestionarioMdl = new CuestionarioModel();
            mensualidadMdl = new MensualidadModel();
            asesoria_antropometriaMdl = new AntropometriaModel();
            alertasMdl = new AlertasModel();
            tiporutinaMdl = new TiporutinaModel();
            tipoentrenamientoMdl = new TipoentrenamientoModel();
            ejerciciosMdl = new EjerciciosModel();
            estatusMdl = new EstatusModel();
            antropometriaMdl = new AntropometriaModel();
            detallerutinaMdl = new DetallerutinaModel();
            

            //listas
            listclientesMdl = new List<ClientesModel>();
            listmensualidadMdl = new List<MensualidadModel>();
            listtiporutinaMdl = new List<TiporutinaModel>();
            listtipoentrenamientoMdl = new List<TipoentrenamientoModel>();
            listejerciciosMdl = new List<EjerciciosModel>();
            listestatusMdl = new List<EstatusModel>();
            listantropometriaMdl = new List<AntropometriaModel>();
            listrutinasMdl = new List<RutinasModel>();
            listmesesMdl = new List<MesesModel>();
            listdetallerutinaMdl = new List<DetallerutinaModel>();
        }
        // GET: AdministracionWebHome
        public ActionResult AdministracionWebHome()
        {
            List<ClientesModel> pendientes = new List<ClientesModel>();
            List<ClientesModel> activos = new List<ClientesModel>();
            List<ClientesModel> inactivos = new List<ClientesModel>();
            List<ClientesModel> porfinalizar = new List<ClientesModel>();

            listclientesMdl = (from l in Db.Clientes
                               select new ClientesModel()
                               {
                                   Id_cliente = l.Id_cliente,
                                   Clave_cliente = l.Clave_cliente,
                                   Nombres = l.Nombres,
                                   Apellido_paterno = l.Apellido_paterno,
                                   Apellido_materno = l.Apellido_materno,
                                   Apodo = l.Apodo,
                                   Edad = l.Edad,
                                   Telefono = (decimal)l.Telefono,
                                   Correo_electronico = l.Correo_electronico,
                                   Estado = (bool)l.Estado,
                                   Contraseña = l.Contraseña,
                                   Foto_perfil = l.Foto_perfil,
                                   Sexo = l.Sexo
                               }).ToList();

            listmensualidadMdl = (from m in Db.Mensualidades
                                  join c in Db.Clientes
                                  on m.Id_cliente equals c.Id_cliente                                  
                                  join es in Db.Estatus
                                  on m.Id_estatus equals es.Id_estatus                                  
                                  select new MensualidadModel()
                                  {
                                      Id_mensualidad = m.Id_mensualidad,
                                      Fecha_inicio = (DateTime)m.Fecha_inicio,
                                      Fecha_fin = (DateTime)m.Fecha_fin,
                                      Estatus = new EstatusModel
                                      {
                                          Id_estatus = es.Id_estatus,
                                          Descripcion = es.Descripcion
                                      },
                                      Cliente = new ClientesModel
                                      {
                                          Id_cliente = c.Id_cliente,
                                          Clave_cliente = c.Clave_cliente,
                                          Nombres = c.Nombres,
                                          Apellido_paterno = c.Apellido_paterno,
                                          Apellido_materno = c.Apellido_materno,
                                          Apodo = c.Apodo,
                                          Edad = c.Edad,
                                          Telefono = (decimal)c.Telefono,
                                          Correo_electronico = c.Correo_electronico,
                                          Estado = (bool)c.Estado,
                                          Contraseña = c.Contraseña,
                                          Foto_perfil = c.Foto_perfil,
                                          Sexo = c.Sexo
                                      }
                                  }).ToList();

            if (listclientesMdl.Count > 0)
            {
                foreach(ClientesModel cliente in listclientesMdl)
                {             
                    if (listmensualidadMdl.Count > 0)
                    {
                        var list = listmensualidadMdl.Where(y => y.Cliente.Id_cliente == cliente.Id_cliente).ToList();
                        var mensualidad = list.OrderByDescending(p => p.Fecha_fin).FirstOrDefault();
                        if (mensualidad != null)
                        {
                            if (mensualidad.Estatus.Id_estatus == 1)
                            {
                                pendientes.Add(cliente);
                            }
                            else if (mensualidad.Estatus.Id_estatus == 2 || mensualidad.Estatus.Id_estatus == 4)
                            {
                                activos.Add(cliente);
                            }
                            else
                            {
                                inactivos.Add(cliente);
                            }
                        }
                    }
                    if (cliente != null)
                    {
                        var list = listmensualidadMdl.Where(y => y.Cliente.Id_cliente == cliente.Id_cliente).ToList();
                        if (list.Count == 0)
                        {
                            pendientes.Add(cliente);
                        }                        
                    }
                }

                if(listmensualidadMdl.Count > 0)
                {                    
                    var finalizar = listmensualidadMdl.Where(y => (y.Estatus.Id_estatus == 2 || y.Estatus.Id_estatus == 4) && DateTime.Now.AddDays(5) >= y.Fecha_fin).ToList();
                    if(finalizar.Count > 0)
                    {
                        foreach(MensualidadModel mes in finalizar)
                        {
                            if((mes.Fecha_fin < DateTime.Now) && (mes.Estatus.Id_estatus == 2 || mes.Estatus.Id_estatus == 4))
                            {
                                mensualidad = Db.Mensualidades.Where(y => y.Id_mensualidad == mes.Id_mensualidad).FirstOrDefault();
                                if (mensualidad != null)
                                {
                                    mensualidad.Id_estatus = 3;
                                    if (Db.SaveChanges() == 1)
                                    {
                                        mes.Cliente = null;
                                    }
                                }
                            }
                        }
                        porfinalizar = (from c in finalizar
                                        select c.Cliente).ToList();
                    }
                    
                }

                ViewBag.pendientes = pendientes;
                ViewBag.activos = activos;
                ViewBag.inactivos = inactivos;
                ViewBag.porfinalizar = porfinalizar;
            }
            
            return View();
        }

        #region Listas
        public void GetList()
        {
            ViewBag.Cliente = (from t in Db.Clientes select new ClientesModel() { Id_cliente = t.Id_cliente, Nombres = t.Nombres + " " + t.Apellido_paterno + " " + t.Apellido_materno }).ToList();
            ViewBag.Tiporutina = (from t in Db.Tipo_rutinas select new TiporutinaModel() { Id_tiporutina = t.Id_tipo_rutina, Descripcion = t.Descripcion }).ToList();
            ViewBag.Mes = (from t in Db.Meses select new MesesModel() { Id_mes = t.Id_mes, Mes = t.Mes }).ToList();
            ViewBag.Estatus = (from t in Db.Estatus select new EstatusModel() { Id_estatus = t.Id_estatus, Descripcion = t.Descripcion }).ToList();
            ViewBag.TipoEntrenamiento = (from t in Db.Tipo_entrenamientos select new TipoentrenamientoModel() { Id_TipoEntrenamiento = t.Id_tipo_entrenamiento, Tipo_entrenamiento = t.Tipo_entrenamiento }).ToList();
            ViewBag.Rutinas = (from t in Db.Rutinas select new RutinasModel() { Id_rutina = t.Id_rutina, Descripcion = t.Descripcion }).ToList();
            ViewBag.Dias = (from t in Db.Dias select new DiasModel() { Id_dia = t.Id_dia, Dia = t.Dia }).ToList();
            ViewBag.Ejercicios = (from t in Db.Ejercicios select new EjerciciosModel() { Id_ejercicio = t.Id_ejercicio, Ejercicio = t.Ejercicio }).ToList();
        }
        #endregion

        #region Clientes
        public ActionResult AdminClientesWeb()
        {
            listclientesMdl = (from l in Db.Clientes
                               select new ClientesModel()
                               {
                                   Id_cliente = l.Id_cliente,
                                   Clave_cliente = l.Clave_cliente,
                                   Nombres = l.Nombres,
                                   Apellido_paterno = l.Apellido_paterno,
                                   Apellido_materno = l.Apellido_materno,
                                   Apodo = l.Apodo,
                                   Edad = l.Edad,
                                   Telefono = (decimal)l.Telefono,
                                   Correo_electronico = l.Correo_electronico,
                                   Estado = (bool)l.Estado,
                                   Contraseña = l.Contraseña,
                                   Foto_perfil = l.Foto_perfil,
                                   Sexo = l.Sexo
                               }).ToList();
            GetList();
            return View(listclientesMdl);
        }

        [HttpPost]
        public bool AgregarCliente(ClientesModel clientesModel)
        {
            bool result = false;
            var cliente = Db.Clientes.Where(p => p.Correo_electronico == clientesModel.Correo_electronico
                                        || p.Telefono == clientesModel.Telefono).FirstOrDefault();

            if (cliente == null)
            {
                string Clave = "000C-";
                string fotoperfil = "/Imagenes/Clientes/" + Clave;
                string UpdateClave = string.Empty;
                clientes = new Clientes
                {
                    Clave_cliente = Clave,
                    Nombres = clientesModel.Nombres,
                    Apellido_paterno = clientesModel.Apellido_paterno,
                    Apellido_materno = clientesModel.Apellido_materno,
                    Edad = clientesModel.Edad,
                    Telefono = clientesModel.Telefono == null ? 0 : clientesModel.Telefono,
                    Correo_electronico = clientesModel.Correo_electronico == null ? string.Empty : clientesModel.Correo_electronico,
                    Apodo = clientesModel.Apodo == null ? "" : clientesModel.Apodo,
                    Contraseña = clientesModel.Contraseña,
                    Foto_perfil = fotoperfil,
                    Sexo = clientesModel.Sexo,
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
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                        
                }
                else
                {
                    result = false;
                }
            }
            else
            {
                result = false;
            }
            return result;
        }

        public JsonResult EditarCliente(int Id_cliente)
        {
            clientesMdl = (from l in Db.Clientes
                           where l.Id_cliente == Id_cliente
                           select new ClientesModel()
                           {
                               Id_cliente = l.Id_cliente,
                               Clave_cliente = l.Clave_cliente,
                               Nombres = l.Nombres,
                               Apellido_paterno = l.Apellido_paterno,
                               Apellido_materno = l.Apellido_materno,
                               Apodo = l.Apodo,
                               Edad = l.Edad,
                               Telefono = (decimal)l.Telefono,
                               Correo_electronico = l.Correo_electronico,
                               Estado = (bool)l.Estado,
                               Contraseña = l.Contraseña,
                               Foto_perfil = l.Foto_perfil,
                               Sexo = l.Sexo
                           }).FirstOrDefault();

            return Json(clientesMdl, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public bool EditarCliente(ClientesModel clientesModel)
        {
            clientes = Db.Clientes.Where(p => p.Id_cliente == clientesModel.Id_cliente).FirstOrDefault();
            clientes.Nombres = clientesModel.Nombres;
            clientes.Apellido_paterno = clientesModel.Apellido_paterno;
            clientes.Apellido_materno = clientesModel.Apellido_materno;
            clientes.Apodo = clientesModel.Apodo == null ? "" : clientesModel.Apodo;
            clientes.Edad = clientesModel.Edad;
            clientes.Telefono = clientesModel.Telefono == null ? 0 : clientesModel.Telefono;
            clientes.Correo_electronico = clientesModel.Correo_electronico == null ? "" : clientesModel.Correo_electronico;
            clientes.Contraseña = clientesModel.Contraseña;
            clientes.Sexo = clientesModel.Sexo;
            if (Db.SaveChanges() == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //Detalle Cliente
        public ActionResult ClienteDetalle(int Id_cliente)
        {
            Session["Id_Cliente"] = Id_cliente;
            GetList();
            ViewBag.idcliente = Id_cliente;
            return View();
        }

        [HttpPost]
        public bool UpImagenCliente(string Tipo, HttpPostedFileBase Imagen)
        {
            string Clave_ejercicio = TempData["Clave_ejercicio"].ToString();
            bool result = false;
            ejercicios = Db.Ejercicios.Where(y => y.Clave_ejercicio == Clave_ejercicio).FirstOrDefault();
            if (Imagen == null)
            {
                return true;
            }
            if (Tipo == "Definicion")
            {
                ejercicios.Ubicacion_imagen = "/Imagenes/Definicion/" + Imagen.FileName.ToString();
                if (Db.SaveChanges() == 1)
                {
                    Imagen.SaveAs(Server.MapPath("~/Imagenes/Definicion/" + Imagen.FileName.ToString()));
                    result = true;
                }

            }
            if (Tipo == "Volumen")
            {
                ejercicios.Ubicacion_imagen = "/Imagenes/Volumen/" + Imagen.FileName.ToString();
                if (Db.SaveChanges() == 1)
                {
                    Imagen.SaveAs(Server.MapPath("~/Imagenes/Volumen/" + Imagen.FileName.ToString()));
                    result = true;
                }
            }
            return result;
        }

        [HttpPost]
        public bool UpImagenActualizarCliente(string Tipo, HttpPostedFileBase Imagen)
        {
            string Clave_ejercicio = TempData["Clave_ejercicio"].ToString();
            bool result = false;
            ejercicios = Db.Ejercicios.Where(y => y.Clave_ejercicio == Clave_ejercicio).FirstOrDefault();
            if (Imagen == null)
            {
                return true;
            }
            var file = Path.Combine(HttpContext.Server.MapPath("~" + ejercicios.Ubicacion_imagen));
            System.IO.File.Delete(file);
            if (Tipo == "Definicion")
            {
                ejercicios.Ubicacion_imagen = "/Imagenes/Definicion/" + Imagen.FileName.ToString();
                if (Db.SaveChanges() == 1)
                {
                    Imagen.SaveAs(Server.MapPath("~/Imagenes/Definicion/" + Imagen.FileName.ToString()));
                    result = true;
                }

            }
            if (Tipo == "Volumen")
            {
                ejercicios.Ubicacion_imagen = "/Imagenes/Volumen/" + Imagen.FileName.ToString();
                if (Db.SaveChanges() == 1)
                {
                    Imagen.SaveAs(Server.MapPath("~/Imagenes/Volumen/" + Imagen.FileName.ToString()));
                    result = true;
                }
            }
            return result;
        }
        #endregion

        #region Usuarios

        #endregion

        #region Tiporutina
        public ActionResult AdminTiporutinaWeb()
        {
            listtiporutinaMdl = (from l in Db.Tipo_rutinas
                                 select new TiporutinaModel()
                                 {
                                     Id_tiporutina = l.Id_tipo_rutina,
                                     Descripcion = l.Descripcion,
                                     Tipo = l.Tipo,
                                 }).ToList();

            return View(listtiporutinaMdl);
        }

        [HttpPost]
        public bool AgregarTiporutina(TiporutinaModel tiporutinaModel)
        {
            bool result = false;
            tipo_Rutina = new Tipo_rutinas
            {
                Descripcion = tiporutinaModel.Descripcion,
                Tipo = tiporutinaModel.Tipo,
            };
            Db.Tipo_rutinas.Add(tipo_Rutina);
            if (Db.SaveChanges() == 1)
            {
                result = true;
            }

            return result;
        }

        public JsonResult EditarTiporutina(int Id_tipo_rutina)
        {
            tiporutinaMdl = (from t in Db.Tipo_rutinas
                             where t.Id_tipo_rutina == Id_tipo_rutina
                             select new TiporutinaModel()
                             {
                                 Id_tiporutina = t.Id_tipo_rutina,
                                 Descripcion = t.Descripcion,
                                 Tipo = t.Tipo
                             }).FirstOrDefault();

            return Json(tiporutinaMdl, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public bool EditarTiporutina(TiporutinaModel tiporutinaModel)
        {
            tipo_Rutina = Db.Tipo_rutinas.Where(t => t.Id_tipo_rutina == tiporutinaModel.Id_tiporutina).FirstOrDefault();

            tipo_Rutina.Descripcion = tiporutinaModel.Descripcion;
            tipo_Rutina.Tipo = tiporutinaModel.Tipo;

            if (Db.SaveChanges() == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Tipoentrenamiento        
        public ActionResult AdminTipoentrenamientoWeb()
        {
            listtipoentrenamientoMdl = (from l in Db.Tipo_entrenamientos
                                        select new TipoentrenamientoModel()
                                        {
                                            Id_TipoEntrenamiento = l.Id_tipo_entrenamiento,
                                            Clave_Entrenamiento = l.Clave_entrenamiento,
                                            Tipo_entrenamiento = l.Tipo_entrenamiento
                                        }).ToList();

            return View(listtipoentrenamientoMdl);
        }

        [HttpPost]
        public bool AgregarTipoentrenamiento(TipoentrenamientoModel tipoentrenamientoModel)
        {
            bool result = false;
            tipo_Entrenamiento = new Tipo_entrenamientos
            {
                Clave_entrenamiento = tipoentrenamientoModel.Clave_Entrenamiento,
                Tipo_entrenamiento = tipoentrenamientoModel.Tipo_entrenamiento,
            };
            Db.Tipo_entrenamientos.Add(tipo_Entrenamiento);
            if (Db.SaveChanges() == 1)
            {
                result = true;
            }

            return result;
        }

        public JsonResult EditarTipoentrenamiento(int Id_tipo_entrenamiento)
        {
            tipoentrenamientoMdl = (from l in Db.Tipo_entrenamientos
                                    where l.Id_tipo_entrenamiento == Id_tipo_entrenamiento
                                    select new TipoentrenamientoModel()
                                    {
                                        Id_TipoEntrenamiento = l.Id_tipo_entrenamiento,
                                        Clave_Entrenamiento = l.Clave_entrenamiento,
                                        Tipo_entrenamiento = l.Tipo_entrenamiento
                                    }).FirstOrDefault();

            return Json(tipoentrenamientoMdl, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public bool EditarTipoentrenamiento(TipoentrenamientoModel tipoentrenamientoModel)
        {
            tipo_Entrenamiento = Db.Tipo_entrenamientos.Where(t => t.Id_tipo_entrenamiento == tipoentrenamientoModel.Id_TipoEntrenamiento).FirstOrDefault();
            if (tipo_Entrenamiento == null)
            {
                return true;
            }
            tipo_Entrenamiento.Clave_entrenamiento = tipoentrenamientoModel.Clave_Entrenamiento;
            tipo_Entrenamiento.Tipo_entrenamiento = tipoentrenamientoModel.Tipo_entrenamiento;

            if (Db.SaveChanges() == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Rutinas
        public ActionResult AdminRutinasWeb()
        {
            listrutinasMdl = (from l in Db.Rutinas
                              select new RutinasModel()
                              {
                                  Id_rutina = l.Id_rutina,
                                  Clave_rutina = l.Clave_rutina,
                                  Descripcion = l.Descripcion,
                              }).ToList();

            return View(listrutinasMdl);
        }

        [HttpPost]
        public bool AgregarRutinas(RutinasModel rutinasModel)
        {
            bool result = false;
            rutina = new Rutinas
            {
                Clave_rutina = rutinasModel.Clave_rutina,
                Descripcion = rutinasModel.Descripcion
            };
            Db.Rutinas.Add(rutina);
            if (Db.SaveChanges() == 1)
            {
                result = true;
            }

            return result;
        }

        public JsonResult EditarRutinas(int Id_rutina)
        {
            rutinasMdl = (from l in Db.Rutinas
                          where l.Id_rutina == Id_rutina
                          select new RutinasModel()
                          {
                              Id_rutina = l.Id_rutina,
                              Clave_rutina = l.Clave_rutina,
                              Descripcion = l.Descripcion
                          }).FirstOrDefault();

            return Json(rutinasMdl, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public bool EditarRutinas(RutinasModel rutinasModel)
        {
            rutina = Db.Rutinas.Where(t => t.Id_rutina == rutinasModel.Id_rutina).FirstOrDefault();

            rutina.Clave_rutina = rutinasModel.Clave_rutina;
            rutina.Descripcion = rutinasModel.Descripcion;

            if (Db.SaveChanges() == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Ejercicios
        public ActionResult AdminEjerciciosWeb()
        {
            listejerciciosMdl = (from l in Db.Ejercicios
                                 join r in Db.Rutinas
                                 on l.Id_rutina equals r.Id_rutina
                                 select new EjerciciosModel()
                                 {
                                     Id_ejercicio = l.Id_ejercicio,
                                     Clave_ejercicio = l.Clave_ejercicio,
                                     Ejercicio = l.Ejercicio,
                                     Descripcion = l.Descripcion,
                                     Posicion = l.Posicion,
                                     Ubicacion_imagen = l.Ubicacion_imagen,
                                     Rutinas = new RutinasModel()
                                     {
                                         Id_rutina = r.Id_rutina,
                                         Clave_rutina = r.Clave_rutina,
                                         Descripcion =r.Descripcion
                                     }
                                     
                                 }).ToList();

            listrutinasMdl = (from r in Db.Rutinas
                              select new RutinasModel()
                              {
                                  Id_rutina = r.Id_rutina,
                                  Descripcion = r.Descripcion
                              }).ToList();
            ViewBag.ListEjercicios = listrutinasMdl;
            return View(listejerciciosMdl);
        }

        [HttpPost]
        public bool AgregarEjercicios(EjerciciosModel ejerciciosModel)
        {
            bool result = false;
            if (ejerciciosModel == null)
            {
                return true;
            }
            var exercise = Db.Ejercicios.Where(y => y.Clave_ejercicio == ejerciciosModel.Clave_ejercicio).FirstOrDefault();
            if (exercise != null)
            {
                return false;
            }
            ejercicios = new Ejercicios
            {
                Clave_ejercicio = ejerciciosModel.Clave_ejercicio,
                Ejercicio = ejerciciosModel.Ejercicio,
                Descripcion = ejerciciosModel.Descripcion,
                Posicion = ejerciciosModel.Posicion,
                Id_rutina = ejerciciosModel.Rutinas.Id_rutina,
                Ubicacion_imagen = ""
            };
            Db.Ejercicios.Add(ejercicios);
            if (Db.SaveChanges() == 1)
            {
                TempData["Clave_ejercicio"] = ejerciciosModel.Clave_ejercicio;
                result = true;
            }

            return result;
        }

        [HttpPost]
        public bool UpImagen(HttpPostedFileBase Imagen)
        {
            string Clave_ejercicio = TempData["Clave_ejercicio"].ToString();
            bool result = false;
            ejercicios = Db.Ejercicios.Where(y => y.Clave_ejercicio == Clave_ejercicio).FirstOrDefault();
            if (Imagen == null)
            {
                return true;
            }
            ejercicios.Ubicacion_imagen = "/Imagenes/Ejercicios/" + Imagen.FileName.ToString();
            if (Db.SaveChanges() == 1)
            {
                
                Imagen.SaveAs(Server.MapPath("~/Imagenes/Ejercicios/" + Imagen.FileName.ToString()));
                result = true;
            }
            return result;
        }

        public JsonResult EditarEjercicios(int Id_ejercicio)
        {
            ejerciciosMdl = (from l in Db.Ejercicios
                             join r in Db.Rutinas 
                             on l.Id_rutina equals r.Id_rutina
                             where l.Id_ejercicio == Id_ejercicio
                             select new EjerciciosModel()
                             {
                                 Id_ejercicio = l.Id_ejercicio,
                                 Clave_ejercicio = l.Clave_ejercicio,
                                 Ejercicio = l.Ejercicio,
                                 Descripcion = l.Descripcion,
                                 Posicion = l.Posicion,
                                 Ubicacion_imagen = l.Ubicacion_imagen,
                                 Rutinas = new RutinasModel
                                 {
                                     Id_rutina = r.Id_rutina,
                                     Descripcion = r.Descripcion 
                                 }
                             }).FirstOrDefault();            
            return Json(ejerciciosMdl, JsonRequestBehavior.AllowGet);
        }

        //ejercicios por rutina
        public JsonResult EjercicioRutina(int Id_rutina)
        {
            listejerciciosMdl = (from l in Db.Ejercicios
                             where l.Id_rutina == Id_rutina
                             select new EjerciciosModel()
                             {
                                 Id_ejercicio = l.Id_ejercicio,
                                 Clave_ejercicio = l.Clave_ejercicio,
                                 Ejercicio = l.Ejercicio,
                                 Descripcion = l.Descripcion,
                                 Posicion = l.Posicion,
                                 Ubicacion_imagen = l.Ubicacion_imagen                                 
                             }).ToList();

            return Json(listejerciciosMdl, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public bool EditarEjercicios(EjerciciosModel ejerciciosModel)
        {            
            if (ejerciciosModel == null)
            {
                TempData["Clave_ejercicio"] = ejerciciosModel.Clave_ejercicio;
                return true;
            }

            ejercicios = Db.Ejercicios.Where(t => t.Id_ejercicio == ejerciciosModel.Id_ejercicio).FirstOrDefault();
            var exercise = Db.Ejercicios.Where(t => t.Clave_ejercicio == ejerciciosModel.Clave_ejercicio).FirstOrDefault();
            if (exercise != null)
            {
                if (ejercicios.Id_ejercicio != exercise.Id_ejercicio)
                {
                    return false;
                }
                else
                {
                    ejercicios.Clave_ejercicio = ejerciciosModel.Clave_ejercicio;
                    ejercicios.Ejercicio = ejerciciosModel.Ejercicio;
                    ejercicios.Descripcion = ejerciciosModel.Descripcion;
                    ejercicios.Posicion = ejerciciosModel.Posicion;
                    ejercicios.Id_rutina  = ejerciciosModel.Rutinas.Id_rutina;
                    Db.SaveChanges();
                    TempData["Clave_ejercicio"] = ejerciciosModel.Clave_ejercicio;
                    return true;                    
                }
            }
            else
            {
                if (exercise == null)
                {
                    ejercicios.Clave_ejercicio = ejerciciosModel.Clave_ejercicio;
                    ejercicios.Ejercicio = ejerciciosModel.Ejercicio;
                    ejercicios.Descripcion = ejerciciosModel.Descripcion;
                    ejercicios.Posicion = ejerciciosModel.Posicion;
                    ejercicios.Id_rutina = ejerciciosModel.Rutinas.Id_rutina;
                    Db.SaveChanges();
                    TempData["Clave_ejercicio"] = ejerciciosModel.Clave_ejercicio;
                    return true;
                }
                else
                {
                    return false;
                }
            }                                  
        }

        [HttpPost]
        public bool UpImagenActualizar(HttpPostedFileBase Imagen)
        {
            string Clave_ejercicio = TempData["Clave_ejercicio"].ToString();
            bool result = false;
            ejercicios = Db.Ejercicios.Where(y => y.Clave_ejercicio == Clave_ejercicio).FirstOrDefault();
            if (Imagen == null)
            {
                return true;
            }
            var file = Path.Combine(HttpContext.Server.MapPath("~"+ejercicios.Ubicacion_imagen));
            System.IO.File.Delete(file);            
            ejercicios.Ubicacion_imagen = "/Imagenes/Ejercicios/" + Imagen.FileName.ToString();
            if (Db.SaveChanges() == 1)
            {
                Imagen.SaveAs(Server.MapPath("~/Imagenes/Ejercicios/" + Imagen.FileName.ToString()));
                result = true;
            }          
            return result;
        }
        #endregion

        #region Estatus
        public ActionResult AdminEstatusWeb()
        {
            listestatusMdl = (from l in Db.Estatus
                                    select new EstatusModel ()
                                    {
                                        Id_estatus = l.Id_estatus,
                                        Descripcion = l.Descripcion                                        
                                    }).ToList();

            return View(listestatusMdl);
        }

        [HttpPost]
        public bool AgregarEstatus(EstatusModel estatusModel)
        {
            bool result = false;
            estatus = new Estatus
            {                
                Descripcion = estatusModel.Descripcion                
            };
            Db.Estatus.Add(estatus);
            if (Db.SaveChanges() == 1)
            {
                result = true;
            }

            return result;
        }

        public JsonResult EditarEstatus(int Id_estatus)
        {
            estatusMdl = (from t in Db.Estatus
                                where t.Id_estatus == Id_estatus
                             select new EstatusModel()
                                {
                                    Id_estatus = t.Id_estatus,
                                    Descripcion = t.Descripcion
                                }).FirstOrDefault();

            return Json(estatusMdl, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public bool Editarestatus(EstatusModel estatusModel)
        {
            estatus = Db.Estatus.Where(t => t.Id_estatus == estatusModel.Id_estatus).FirstOrDefault();

            estatus.Descripcion = estatusModel.Descripcion;            
            if (Db.SaveChanges() == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Mensualidad
        public ActionResult AdminMensualidadWeb()
        {
            listmensualidadMdl = (from m in Db.Mensualidades
                                  join c in Db.Clientes
                                  on m.Id_cliente equals c.Id_cliente
                                  join t in Db.Tipo_rutinas
                                  on m.Id_tipo_rutina equals t.Id_tipo_rutina
                                  join mes in Db.Meses
                                  on m.Id_mes equals mes.Id_mes
                                  join es in Db.Estatus
                                  on m.Id_estatus equals es.Id_estatus
                                  join te in Db.Tipo_entrenamientos
                                  on m.Id_tipo_entrenamiento equals te.Id_tipo_entrenamiento                                 
                                  select new MensualidadModel()
                                  {
                                      Id_mensualidad = m.Id_mensualidad,
                                      Fecha_inicio = (DateTime)m.Fecha_inicio,
                                      Fecha_fin = (DateTime)m.Fecha_fin,
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
                                          Id_mes = mes.Id_mes,
                                          Clave_mes = mes.Clave_mes,
                                          Mes = mes.Mes
                                      },
                                      Estatus = new EstatusModel
                                      {
                                          Id_estatus = es.Id_estatus,
                                          Descripcion = es.Descripcion
                                      },
                                      Cliente = new ClientesModel
                                      {
                                          Id_cliente = c.Id_cliente,
                                          Clave_cliente = c.Clave_cliente,
                                          Nombres = c.Nombres,
                                          Apellido_paterno = c.Apellido_paterno,
                                          Apellido_materno = c.Apellido_materno,
                                          Apodo = c.Apodo,
                                          Edad = c.Edad,
                                          Telefono = (decimal)c.Telefono,
                                          Correo_electronico = c.Correo_electronico,
                                          Estado = (bool)c.Estado,
                                          Contraseña = c.Contraseña,
                                          Foto_perfil = c.Foto_perfil,
                                          Sexo = c.Sexo
                                      }
                                  }).ToList();
            GetList();
            return View(listmensualidadMdl);
        }

        public JsonResult VerMensualidad(int Id_cliente)
        {
            DateTimeFormatInfo fechastring = CultureInfo.GetCultureInfo("es-ES").DateTimeFormat;
            string Mes = string.Empty, Dia = string.Empty; 
            listmensualidadMdl = (from m in Db.Mensualidades
                                  join c in Db.Clientes
                                  on m.Id_cliente equals c.Id_cliente
                                  join t in Db.Tipo_rutinas
                                  on m.Id_tipo_rutina equals t.Id_tipo_rutina
                                  join mes in Db.Meses
                                  on m.Id_mes equals mes.Id_mes
                                  join es in Db.Estatus
                                  on m.Id_estatus equals es.Id_estatus
                                  join te in Db.Tipo_entrenamientos
                                  on m.Id_tipo_entrenamiento equals te.Id_tipo_entrenamiento
                                  where m.Id_cliente == Id_cliente
                                  select new MensualidadModel()
                                  {
                                      Id_mensualidad = m.Id_mensualidad,
                                      Fecha_inicio = (DateTime)m.Fecha_inicio,
                                      Fecha_fin = (DateTime)m.Fecha_fin,                                      
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
                                          Id_mes = mes.Id_mes,
                                          Clave_mes = mes.Clave_mes,
                                          Mes = mes.Mes
                                      },
                                      Estatus = new EstatusModel
                                      {
                                          Id_estatus = es.Id_estatus,
                                          Descripcion = es.Descripcion
                                      },
                                      Cliente = new ClientesModel
                                      {
                                          Id_cliente = c.Id_cliente,
                                          Clave_cliente = c.Clave_cliente,
                                          Nombres = c.Nombres,
                                          Apellido_paterno = c.Apellido_paterno,
                                          Apellido_materno = c.Apellido_materno,
                                          Apodo = c.Apodo,
                                          Edad = c.Edad,
                                          Telefono = (decimal)c.Telefono,
                                          Correo_electronico = c.Correo_electronico,
                                          Estado = (bool)c.Estado,
                                          Contraseña = c.Contraseña,
                                          Foto_perfil = c.Foto_perfil,
                                          Sexo = c.Sexo
                                      }
                                  }).ToList();

            if (listmensualidadMdl == null || listmensualidadMdl.Count==0)
            {
                listmensualidadMdl = new List<MensualidadModel>();
            }
            else
            {              
                for(int i=0;i< listmensualidadMdl.Count; i++)
                {                    
                    Mes = fechastring.GetMonthName(listmensualidadMdl[i].Fecha_inicio.Month);
                    Dia = fechastring.GetDayName(listmensualidadMdl[i].Fecha_inicio.DayOfWeek);
                    listmensualidadMdl[i].Fechainicio = Dia + " " + listmensualidadMdl[i].Fecha_inicio.Day.ToString() + " de " + Mes + " de " + listmensualidadMdl[i].Fecha_inicio.Year;
                    Mes = fechastring.GetMonthName(listmensualidadMdl[i].Fecha_fin.Month);
                    Dia = fechastring.GetDayName(listmensualidadMdl[i].Fecha_fin.DayOfWeek);
                    listmensualidadMdl[i].Fechafin = Dia + " " + listmensualidadMdl[i].Fecha_fin.Day.ToString() + " de " + Mes + " de " + listmensualidadMdl[i].Fecha_fin.Year;
                    int idmensu = listmensualidadMdl[i].Id_mensualidad;
                    var pago = Db.Pagos_mensualidades.Where(alitza => alitza.Id_mensualidad == idmensu).FirstOrDefault();
                    listmensualidadMdl[i].PagoMes = new PagosmensualModel();
                    if (pago != null)
                    {
                        DateTime fecha = (DateTime)pago.Fecha_pago;
                        Mes = fechastring.GetMonthName(fecha.Month);
                        Dia = fechastring.GetDayName(fecha.DayOfWeek);
                        listmensualidadMdl[i].PagoMes.Id_pago = pago.Id_pago;
                        listmensualidadMdl[i].PagoMes.Fechapago = Dia + " " + fecha.Day.ToString() + " de " + Mes + " de " + fecha.Year;
                        listmensualidadMdl[i].PagoMes.Ubicacion_imagen_pago = pago.Ubicacion_imagen_pago;
                    }
                    else
                    {
                        listmensualidadMdl[i].PagoMes.Id_pago = 0;
                    }
                }
            }

            return Json(listmensualidadMdl,JsonRequestBehavior.AllowGet);
        }

        public JsonResult VerMensualidadId(int Id_mensualidad)
        {
            mensualidadMdl = (from m in Db.Mensualidades
                                  join c in Db.Clientes
                                  on m.Id_cliente equals c.Id_cliente
                                  join t in Db.Tipo_rutinas
                                  on m.Id_tipo_rutina equals t.Id_tipo_rutina
                                  join mes in Db.Meses
                                  on m.Id_mes equals mes.Id_mes
                                  join es in Db.Estatus
                                  on m.Id_estatus equals es.Id_estatus
                                  join te in Db.Tipo_entrenamientos
                                  on m.Id_tipo_entrenamiento equals te.Id_tipo_entrenamiento
                                  where m.Id_mensualidad == Id_mensualidad
                                  select new MensualidadModel()
                                  {
                                      Id_mensualidad = m.Id_mensualidad,
                                      Fecha_inicio = (DateTime)m.Fecha_inicio,
                                      Fecha_fin = (DateTime)m.Fecha_fin,
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
                                          Id_mes = mes.Id_mes,
                                          Clave_mes = mes.Clave_mes,
                                          Mes = mes.Mes
                                      },
                                      Estatus = new EstatusModel
                                      {
                                          Id_estatus = es.Id_estatus,
                                          Descripcion = es.Descripcion
                                      },
                                      Cliente = new ClientesModel
                                      {
                                          Id_cliente = c.Id_cliente,
                                          Clave_cliente = c.Clave_cliente,
                                          Nombres = c.Nombres,
                                          Apellido_paterno = c.Apellido_paterno,
                                          Apellido_materno = c.Apellido_materno,
                                          Apodo = c.Apodo,
                                          Edad = c.Edad,
                                          Telefono = (decimal)c.Telefono,
                                          Correo_electronico = c.Correo_electronico,
                                          Estado = (bool)c.Estado,
                                          Contraseña = c.Contraseña,
                                          Foto_perfil = c.Foto_perfil,
                                          Sexo = c.Sexo
                                      }
                                  }).FirstOrDefault();

            if(mensualidadMdl == null)
            {
                mensualidadMdl = new MensualidadModel();
            }
            else
            {
                mensualidadMdl.Fechainicio = mensualidadMdl.Fecha_inicio.ToString("dd/MM/yyyy");
                mensualidadMdl.Fechafin = mensualidadMdl.Fecha_fin.ToString("dd/MM/yyyy");
            }

            return Json(mensualidadMdl, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public bool ActualizarEstatus(int Id_mensualidad,int estatus,int Id_tipo_entrenamiento,int Id_tipo_rutina)
        {
            bool result = false; 
            mensualidad = Db.Mensualidades.Where(y => y.Id_mensualidad == Id_mensualidad).FirstOrDefault();
            if (mensualidad != null)
            {
                if(mensualidad.Id_estatus != estatus)
                {
                    if (estatus == 2 || estatus == 4)
                    {
                        mensualidad.Fecha_inicio = DateTime.Now;
                        mensualidad.Fecha_fin = DateTime.Now.AddDays(30);
                        mensualidad.Id_mes = DateTime.Now.AddDays(30).Month;
                    }
                    mensualidad.Id_estatus = estatus;
                    if (Db.SaveChanges() == 1)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }  
                }
                if (mensualidad.Id_tipo_entrenamiento != Id_tipo_entrenamiento)
                {
                    mensualidad.Id_tipo_entrenamiento = Id_tipo_entrenamiento;
                    if (Db.SaveChanges() == 1)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
                if (mensualidad.Id_tipo_rutina != Id_tipo_rutina)
                {
                    mensualidad.Id_tipo_rutina = Id_tipo_rutina;
                    if (Db.SaveChanges() == 1)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
                else
                {
                    result= true;
                }
            }
            return result;
        }
        [HttpPost]
        public bool AgregarMensualidad(MensualidadModel mensualidadModel)
        {
            bool result = false;
            mensualidadModel.Fecha_inicio = DateTime.Now;
            mensualidadModel.Fecha_fin = DateTime.Now.AddDays(30);            
            mensualidad = new Mensualidades
            {                
                Id_cliente = mensualidadModel.Cliente.Id_cliente,
                Id_tipo_rutina = mensualidadModel.Tiporutina.Id_tiporutina,
                Id_mes= mensualidadModel.Mes.Id_mes,
                Id_estatus= mensualidadModel.Estatus.Id_estatus,
                Id_tipo_entrenamiento= mensualidadModel.TipoEntrenamiento.Id_TipoEntrenamiento,
                Fecha_inicio= mensualidadModel.Fecha_inicio,
                Fecha_fin= mensualidadModel.Fecha_fin
            };
            Db.Mensualidades.Add(mensualidad);
            if (Db.SaveChanges() == 1)
            {
                result = true;
            }

            return result;
        }

        public JsonResult EditarMensualidad(int Id_mensualidad)
        {
            mensualidadMdl = (from m in Db.Mensualidades
                              join c in Db.Clientes
                              on m.Id_cliente equals c.Id_cliente
                              join t in Db.Tipo_rutinas
                              on m.Id_tipo_rutina equals t.Id_tipo_rutina
                              join mes in Db.Meses
                              on m.Id_mes equals mes.Id_mes
                              join es in Db.Estatus
                              on m.Id_estatus equals es.Id_estatus
                              join te in Db.Tipo_entrenamientos
                              on m.Id_tipo_entrenamiento equals te.Id_tipo_entrenamiento
                              select new MensualidadModel()
                              {
                                  Id_mensualidad = m.Id_mensualidad,
                                  Fecha_inicio = (DateTime)m.Fecha_inicio,
                                  Fecha_fin = (DateTime)m.Fecha_fin,
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
                                      Id_mes = mes.Id_mes,
                                      Clave_mes = mes.Clave_mes,
                                      Mes = mes.Mes
                                  },
                                  Estatus = new EstatusModel
                                  {
                                      Id_estatus = es.Id_estatus,
                                      Descripcion = es.Descripcion
                                  },
                                  Cliente = new ClientesModel
                                  {
                                      Id_cliente = c.Id_cliente,
                                      Clave_cliente = c.Clave_cliente,
                                      Nombres = c.Nombres,
                                      Apellido_paterno = c.Apellido_paterno,
                                      Apellido_materno = c.Apellido_materno,
                                      Apodo = c.Apodo,
                                      Edad = c.Edad,
                                      Telefono = (decimal)c.Telefono,
                                      Correo_electronico = c.Correo_electronico,
                                      Estado = (bool)c.Estado,
                                      Contraseña = c.Contraseña,
                                      Foto_perfil = c.Foto_perfil,
                                      Sexo = c.Sexo
                                  }
                              }).FirstOrDefault();

            return Json(mensualidadMdl, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public bool EditarMensualidad(MensualidadModel mensualidadModel)
        {
            mensualidad = Db.Mensualidades.Where(t => t.Id_mensualidad == mensualidadModel.Id_mensualidad).FirstOrDefault();

            mensualidad.Id_cliente = mensualidadModel.Cliente.Id_cliente;
            mensualidad.Id_tipo_rutina = mensualidadModel.Tiporutina.Id_tiporutina;
            mensualidad.Id_mes = mensualidadModel.Mes.Id_mes;
            mensualidad.Id_estatus = mensualidadModel.Estatus.Id_estatus;
            mensualidad.Id_tipo_entrenamiento = mensualidadModel.TipoEntrenamiento.Id_TipoEntrenamiento;
            mensualidad.Fecha_inicio = mensualidadModel.Fecha_inicio;
            mensualidad.Fecha_fin = mensualidadModel.Fecha_fin;

            if (Db.SaveChanges() == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Asesoriaantropometria
        public ActionResult AdminAsesoriaantropometriaWeb()
        {
            listantropometriaMdl = (from a in Db.Asesoria_antropometria
                                 select new AntropometriaModel ()
                                 {
                                     Id=a.Id,                                  
                                    Peso=a.Peso,
                                    Altura=a.Altura,
                                    IMC=a.IMC,
                                    Brazoderechorelajado  =a.Brazo_derecho_relajado,
                                    Brazoderechofuerza=a.Brazo_derecho_fuerza,
                                    Brazoizquierdorelajado=a.Brazo_izquierdo_relajado,
                                    Brazoizquierdofuerza  =a.Brazo_izquierdo_fuerza,
                                    Cintura                 =a.Cintura,
                                    Cadera                  =a.Cadera,
                                    Piernaizquierda        =a.Pierna_izquierda,
                                    Piernaderecho          =a.Pierna_derecho,
                                    Pantorrilladerecha     =a.Pantorrilla_derecha,
                                    Pantorrillaizquierda   =a.Pantorrilla_izquierda,
                                    Fotofrontal            =a.Foto_frontal,
                                    Fotoperfil             =a.Foto_perfil,
                                    Fotoposterior          =a.Foto_posterior,
                                    Fecha_registro          =a.Fecha_registro
                                 }).ToList();

            return View(listantropometriaMdl);
        }        
        [HttpPost]
        public bool AgregarAsesoriaantropometria(AntropometriaModel antropometriaModel)
        {
            bool result = false;
            asesoria_antropometria = new Asesoria_antropometria
            {
                Peso = antropometriaModel.Peso,
                Id_mensualidad = antropometriaModel.Mensualidad.Id_mensualidad,
                Altura = antropometriaModel.Altura,
                IMC = antropometriaModel.IMC,
                Brazo_derecho_relajado = antropometriaModel.Brazoderechorelajado,
                Brazo_derecho_fuerza = antropometriaModel.Brazoderechofuerza,
                Brazo_izquierdo_relajado = antropometriaModel.Brazoizquierdorelajado,
                Brazo_izquierdo_fuerza = antropometriaModel.Brazoizquierdofuerza,
                Cintura = antropometriaModel.Cintura,
                Cadera = antropometriaModel.Cadera,
                Pierna_izquierda = antropometriaModel.Piernaizquierda,
                Pierna_derecho = antropometriaModel.Piernaderecho,
                Pantorrilla_derecha = antropometriaModel.Pantorrilladerecha,
                Pantorrilla_izquierda = antropometriaModel.Pantorrillaizquierda,
                Foto_frontal = antropometriaModel.Fotofrontal,
                Foto_perfil = antropometriaModel.Fotoperfil,
                Foto_posterior = antropometriaModel.Fotoposterior,
                Fecha_registro = DateTime.Now
            };
            Db.Asesoria_antropometria.Add(asesoria_antropometria);
            if (Db.SaveChanges() == 1)
            {
                result = true;
            }

            return result;
        }

        public JsonResult EditarAsesoriaantropometria(int Id)
        {
            asesoria_antropometriaMdl = (from a in Db.Asesoria_antropometria
                             where a.Id == Id
                             select new AntropometriaModel()
                             {
                                 Id = a.Id,
                                 Peso = a.Peso,
                                 Altura = a.Altura,
                                 IMC = a.IMC,
                                 Brazoderechorelajado = a.Brazo_derecho_relajado,
                                 Brazoderechofuerza = a.Brazo_derecho_fuerza,
                                 Brazoizquierdorelajado = a.Brazo_izquierdo_relajado,
                                 Brazoizquierdofuerza = a.Brazo_izquierdo_fuerza,
                                 Cintura = a.Cintura,
                                 Cadera = a.Cadera,
                                 Piernaizquierda = a.Pierna_izquierda,
                                 Piernaderecho = a.Pierna_derecho,
                                 Pantorrilladerecha = a.Pantorrilla_derecha,
                                 Pantorrillaizquierda = a.Pantorrilla_izquierda,
                                 Fotofrontal = a.Foto_frontal,
                                 Fotoperfil = a.Foto_perfil,
                                 Fotoposterior = a.Foto_posterior,
                                 Fecha_registro = a.Fecha_registro
                             }).FirstOrDefault();
            if (asesoria_antropometriaMdl == null)
            {
                asesoria_antropometriaMdl = new AntropometriaModel();
            }

            return Json(asesoria_antropometriaMdl, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public bool EditarAsesoriaantropometria(AntropometriaModel antropometriaModel)
        {
            asesoria_antropometria = Db.Asesoria_antropometria.Where(t => t.Id == antropometriaModel.Id).FirstOrDefault();
            if (asesoria_antropometria != null)
            {
                asesoria_antropometria.Peso = antropometriaModel.Peso;
                asesoria_antropometria.Id_mensualidad = antropometriaModel.Mensualidad.Id_mensualidad;
                asesoria_antropometria.Altura = antropometriaModel.Altura;
                asesoria_antropometria.IMC = antropometriaModel.IMC;
                asesoria_antropometria.Brazo_derecho_relajado = antropometriaModel.Brazoderechorelajado;
                asesoria_antropometria.Brazo_derecho_fuerza = antropometriaModel.Brazoderechofuerza;
                asesoria_antropometria.Brazo_izquierdo_relajado = antropometriaModel.Brazoizquierdorelajado;
                asesoria_antropometria.Brazo_izquierdo_fuerza = antropometriaModel.Brazoizquierdofuerza;
                asesoria_antropometria.Cintura = antropometriaModel.Cintura;
                asesoria_antropometria.Cadera = antropometriaModel.Cadera;
                asesoria_antropometria.Pierna_izquierda = antropometriaModel.Piernaizquierda;
                asesoria_antropometria.Pierna_derecho = antropometriaModel.Piernaderecho;
                asesoria_antropometria.Pantorrilla_derecha = antropometriaModel.Pantorrilladerecha;
                asesoria_antropometria.Pantorrilla_izquierda = antropometriaModel.Pantorrillaizquierda;
                asesoria_antropometria.Foto_frontal = antropometriaModel.Fotofrontal;
                asesoria_antropometria.Foto_perfil = antropometriaModel.Fotoperfil;
                asesoria_antropometria.Foto_posterior = antropometriaModel.Fotoposterior;
                if (Db.SaveChanges() == 1)
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
        #endregion

        #region Cuestionario 
        public JsonResult VerCuestionario(int Id_cliente)
        {

            cuestionarioMdl = (from c in Db.Cuestionarios
                               where c.Id_cliente == Id_cliente
                               select new CuestionarioModel()
                               {
                                   Id_cuestionario =c.Id_cuestionario,
                                    Cliente = new ClientesModel
                                    {
                                        Id_cliente =(int)c.Id_cliente
                                    },
                                    Clave_cuestionario= c.Clave_cuestionario,
                                    Padece_enfermedad= (bool)c.Padece_enfermedad,
                                    Medicamento_prescrito_medico= c.Medicamento_prescrito_medico   ,
                                    Lesiones= (bool)c.Lesiones,
                                    Alguna_recomendacion_lesiones= c.Alguna_recomendacion_lesiones  ,
                                    Fuma= (bool)c.Fuma,
                                    Veces_semana_fuma= (int)c.Veces_semana_fuma,
                                    Alcohol= (bool)c.Alcohol,
                                    Veces_semana_alcohol= (int)c.Veces_semana_alcohol,
                                    Actividad_fisica= (bool)c.Actividad_fisica,
                                    Tipo_ejercicios= c.Tipo_ejercicios,                                    
                                    Tiempo_dedicado = c.Tiempo_dedicado,
                                    Horario_entreno= c.Horario_entreno,
                                    MetasObjetivos= c.Metas_objetivos,
                                    Compromisos= c.Compromisos,
                                    Comentarios= c.Comentarios,
                                    Fecha_registro= (DateTime)c.Fecha_registro
                               }).FirstOrDefault();
            if (cuestionarioMdl == null)
            {
                cuestionarioMdl = new CuestionarioModel();
            }
            return Json(cuestionarioMdl, JsonRequestBehavior.AllowGet);
        }

        public bool AgregarCuestionario(CuestionarioModel cuestionarioModel)
        {
            bool result = false;
            var cliente = Db.Clientes.Where(y => y.Id_cliente == cuestionarioModel.Cliente.Id_cliente).FirstOrDefault();
            cuestionario = new Cuestionarios
            {            
                Id_cliente = cuestionarioModel.Cliente.Id_cliente,
                Clave_cuestionario ="CUES"+ cliente.Clave_cliente,
                Padece_enfermedad               = cuestionarioModel.Padece_enfermedad==null ? false: cuestionarioModel.Padece_enfermedad,
                Medicamento_prescrito_medico    = cuestionarioModel.Medicamento_prescrito_medico ==null ? "" : cuestionarioModel.Medicamento_prescrito_medico , 
                Lesiones                        = cuestionarioModel.Lesiones                     ==null ? false : cuestionarioModel.Lesiones                     , 
                Alguna_recomendacion_lesiones   = cuestionarioModel.Alguna_recomendacion_lesiones==null ? "" : cuestionarioModel.Alguna_recomendacion_lesiones , 
                Fuma                            = cuestionarioModel.Fuma                         ==null ? false : cuestionarioModel.Fuma                         , 
                Veces_semana_fuma               = cuestionarioModel.Veces_semana_fuma            ==null ? 0 : cuestionarioModel.Veces_semana_fuma            , 
                Alcohol                         = cuestionarioModel.Alcohol                      ==null ? false : cuestionarioModel.Alcohol                      , 
                Veces_semana_alcohol            = cuestionarioModel.Veces_semana_alcohol         ==null ? 0 : cuestionarioModel.Veces_semana_alcohol         , 
                Actividad_fisica                = cuestionarioModel.Actividad_fisica             ==null ? false : cuestionarioModel.Actividad_fisica             , 
                Tipo_ejercicios                 = cuestionarioModel.Tipo_ejercicios              ==null ? "": cuestionarioModel.Tipo_ejercicios              , 
                Tiempo_dedicado                 = cuestionarioModel.Tiempo_dedicado              ==null ?"": cuestionarioModel.Tiempo_dedicado              , 
                Horario_entreno                 = cuestionarioModel.Horario_entreno              ==null ?"" : cuestionarioModel.Horario_entreno              , 
                Metas_objetivos                  = cuestionarioModel.MetasObjetivos               ==null ?"" : cuestionarioModel.MetasObjetivos               , 
                Compromisos                     = cuestionarioModel.Compromisos                  ==null ?"" : cuestionarioModel.Compromisos                  , 
                Comentarios                     = cuestionarioModel.Comentarios                  ==null ?"" :  cuestionarioModel.Comentarios                  ,
                Fecha_registro                  =DateTime.Now
            };
            Db.Cuestionarios.Add(cuestionario);
            if (Db.SaveChanges() == 1)
            {
                result = true;
            }

            return result;
        }

        public bool ActualizarCuestionario(CuestionarioModel cuestionarioModel)
        {
            bool result = false;
            cuestionario = Db.Cuestionarios.Where(y => y.Id_cuestionario == cuestionarioModel.Id_cuestionario).FirstOrDefault();
            if (cuestionario != null)
            {
                cuestionario.Padece_enfermedad = cuestionarioModel.Padece_enfermedad == null ? false : cuestionarioModel.Padece_enfermedad;
                cuestionario.Medicamento_prescrito_medico = cuestionarioModel.Medicamento_prescrito_medico == null ? "" : cuestionarioModel.Medicamento_prescrito_medico;
                cuestionario.Lesiones = cuestionarioModel.Lesiones == null ? false : cuestionarioModel.Lesiones;
                cuestionario.Alguna_recomendacion_lesiones = cuestionarioModel.Alguna_recomendacion_lesiones == null ? "" : cuestionarioModel.Alguna_recomendacion_lesiones;
                cuestionario.Fuma = cuestionarioModel.Fuma == null ? false : cuestionarioModel.Fuma;
                cuestionario.Veces_semana_fuma = cuestionarioModel.Veces_semana_fuma == null ? 0 : cuestionarioModel.Veces_semana_fuma;
                cuestionario.Alcohol = cuestionarioModel.Alcohol == null ? false : cuestionarioModel.Alcohol;
                cuestionario.Veces_semana_alcohol = cuestionarioModel.Veces_semana_alcohol == null ? 0 : cuestionarioModel.Veces_semana_alcohol;
                cuestionario.Actividad_fisica = cuestionarioModel.Actividad_fisica == null ? false : cuestionarioModel.Actividad_fisica;
                cuestionario.Tipo_ejercicios = cuestionarioModel.Tipo_ejercicios == null ? "" : cuestionarioModel.Tipo_ejercicios;
                cuestionario.Tiempo_dedicado = cuestionarioModel.Tiempo_dedicado == null ? "" : cuestionarioModel.Tiempo_dedicado;
                cuestionario.Horario_entreno = cuestionarioModel.Horario_entreno == null ? "" : cuestionarioModel.Horario_entreno;
                cuestionario.Metas_objetivos = cuestionarioModel.MetasObjetivos == null ? "" : cuestionarioModel.MetasObjetivos;
                cuestionario.Compromisos = cuestionarioModel.Compromisos == null ? "" : cuestionarioModel.Compromisos;
                cuestionario.Comentarios = cuestionarioModel.Comentarios == null ? "" : cuestionarioModel.Comentarios;
                cuestionario.Fecha_registro = DateTime.Now;
                Db.SaveChanges();
                result =true;
            }

            return result;
        }
        #endregion

        #region DetalleRutina
        //Consultar
        [HttpGet]
        public JsonResult VerDetalleRutina(int Id_mensualidad)
        {
            listdetallerutinaMdl = (from d in Db.Detalle_rutinas
                                where d.Id_mensualidad == Id_mensualidad
                                select new DetallerutinaModel()
                                {
                                    Id_detallerutina = d.Id_detalle_rutina,
                                    Mensualidad = new MensualidadModel
                                    {
                                        Id_mensualidad= d.Mensualidades.Id_mensualidad }
                                    ,
                                    Rutinas = new RutinasModel
                                    {
                                        Id_rutina = d.Rutinas.Id_rutina,
                                        Descripcion = d.Rutinas.Descripcion
                                    },
                                    Ejercicios = new EjerciciosModel
                                    {
                                        Id_ejercicio = d.Ejercicios.Id_ejercicio,
                                        Descripcion = d.Ejercicios.Descripcion,
                                        Ejercicio = d.Ejercicios.Ejercicio,
                                        Ubicacion_imagen = d.Ejercicios.Ubicacion_imagen
                                    },
                                    Dias = new DiasModel
                                    {
                                        Id_dia = d.Dias.Id_dia,
                                        Dia = d.Dias.Dia
                                    },
                                    Repeticiones = d.Repeticiones,
                                    Series = d.Series,
                                    Tipo_set = d.Tipo_set,
                                    
                                }).ToList();
            if (listdetallerutinaMdl == null || listdetallerutinaMdl.Count==0)
            {
                listdetallerutinaMdl = new List<DetallerutinaModel>();
            }

            return Json(listdetallerutinaMdl, JsonRequestBehavior.AllowGet);
        }
        
        //Agregar asignar rutina.
        [HttpPost]
        public bool DetallRutina(List<DetallerutinaModel> detallerutinaModels,bool enviocorreo)
        {
            bool result = false;
            try
            {                
                int id = detallerutinaModels[0].Mensualidad.Id_mensualidad;
                var mes = Db.Mensualidades.Where(p => p.Id_mensualidad == id).FirstOrDefault();
                var cliente = Db.Clientes.Where(y => y.Id_cliente == mes.Id_cliente).FirstOrDefault();
                List<Detalle_rutinas> detalle = Db.Detalle_rutinas.Where(p => p.Id_mensualidad == id).ToList();
                if (detalle.Count > 0)
                {
                    Db.Detalle_rutinas.RemoveRange(detalle);
                    Db.SaveChanges();
                }

                listdetalle_Rutina = (from lista in detallerutinaModels
                                     select new Detalle_rutinas()
                                    {
                                        Id_mensualidad = lista.Mensualidad.Id_mensualidad,
                                        Id_rutina = lista.Rutinas.Id_rutina,
                                        Id_ejercicio = lista.Ejercicios.Id_ejercicio,
                                        Repeticiones = lista.Repeticiones,
                                        Series = lista.Series,
                                        Tipo_set = lista.Tipo_set,
                                        Id_dia = lista.Dias.Id_dia
                                    }).ToList();

                Db.Detalle_rutinas.AddRange(listdetalle_Rutina);
                Db.SaveChanges();
                result = true;
                if (cliente != null)
                {
                    if (enviocorreo == true)
                    {
                        AlertasModel resultado = Bienvenida(cliente.Correo_electronico, cliente.Nombres);
                        result = resultado.Result;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }
        public AlertasModel Bienvenida(string User, string Name)
        {
            try
            {
                Dictionary<string, string> datoscorreo = new Dictionary<string, string>();
                datoscorreo.Add("@Name", Name);
                string plantilla = Server.MapPath("~/Plantillas/BienvenidaSuperfit.html");
                string succes = "Se envio la rutina con exito y bienvenida";
                string asunto = "Rutina personalizada";
                AlertasModel resultado = envio.EnviarCorreo(User, plantilla, datoscorreo, succes, asunto);
                alertasMdl = resultado;
            }
            catch (Exception ex)
            {
                alertasMdl.Result = false;
                alertasMdl.Mensaje = ex.Message;
            }
            return alertasMdl;
        }
        #endregion                

        #region Entrenamiento en vivo
        public ActionResult Entrenamientoenvivo()
        {
            return View();
        }
        #endregion

        //cerrar sesion
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
    }
}