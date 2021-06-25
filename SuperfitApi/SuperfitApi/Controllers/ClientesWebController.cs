using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SuperfitApi.Models.Entity;
using SuperfitApi.Models;
using SuperfitApi.Autetication;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Net.Mail;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

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
        public Pagos_mensualidades pagos_Mensualidades;
        public EnvioNotificaciones envio;
        //modelos
        public ClientesModel clientesMdl;
        public CuestionarioModel cuestionarioMdl;
        public MensualidadModel mensualidadMdl;
        public AntropometriaModel asesoria_antropometriaMdl;
        public AlertasModel alertasMdl;
        public PagosmensualModel PagosmensualMdl;
        //listas de modelos
        public List<DetallerutinaModel> listdetallerutinaMdl;
        public List<MensualidadModel> listmensualidadMdl;
        public List<AntropometriaModel> listantropometriaMdl;
        #endregion       

        public ClientesWebController()
        {
            Db = new SuperfitEntities ();
            clientes = new Clientes();
            cuestionario = new Cuestionario();
            mensualidad = new Mensualidad();
            asesoria_antropometria = new Asesoria_antropometria();
            pagos_Mensualidades = new Pagos_mensualidades();
            envio = new EnvioNotificaciones();
            //modelos
            clientesMdl = new ClientesModel();
            cuestionarioMdl = new CuestionarioModel();
            mensualidadMdl = new MensualidadModel();
            asesoria_antropometriaMdl = new AntropometriaModel();
            alertasMdl = new AlertasModel();
            PagosmensualMdl = new PagosmensualModel();
            //listas de modelos
            listdetallerutinaMdl = new List<DetallerutinaModel>();
            listmensualidadMdl = new List<MensualidadModel>();
            listantropometriaMdl = new List<AntropometriaModel>();
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
                DateTimeFormatInfo fechastring = CultureInfo.GetCultureInfo("es-ES").DateTimeFormat;                
                string Mes = string.Empty, Dia = string.Empty,finalizar=string.Empty;
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
                    Mes = fechastring.GetMonthName(mensualidadMdl.Fecha_fin.Month);
                    Dia = fechastring.GetDayName(mensualidadMdl.Fecha_fin.DayOfWeek);
                    mensualidadMdl.Fechafin = Dia +" "+ mensualidadMdl.Fecha_fin.Day.ToString() +" de " + Mes + " de "+ mensualidadMdl.Fecha_fin.Year;
                    Mes = fechastring.GetMonthName(mensualidadMdl.Fecha_inicio.Month);
                    Dia = fechastring.GetDayName(mensualidadMdl.Fecha_inicio.DayOfWeek);
                    mensualidadMdl.Fechainicio = Dia + " " + mensualidadMdl.Fecha_inicio.Day.ToString() + " de " + Mes + " de " + mensualidadMdl.Fecha_inicio.Year;
                    if((mensualidadMdl.Estatus.Id_estatus == 2 || mensualidadMdl.Estatus.Id_estatus == 4) && DateTime.Now.AddDays(5) >= mensualidadMdl.Fecha_fin)
                    {
                        finalizar = "Tu mes esta por acabar";
                    }
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
                ViewBag.Finalizar = finalizar;
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

        public ActionResult Mensualidades()
        {
            DateTimeFormatInfo fechastring = CultureInfo.GetCultureInfo("es-ES").DateTimeFormat;
            string Mes = string.Empty, Dia = string.Empty;
            string Id = Session["Id_Cliente"].ToString();
            int IdCliente = int.Parse(Id);
            listmensualidadMdl = (from m in Db.Mensualidad
                                  join c in Db.Clientes
                                  on m.Id_cliente equals c.Id_cliente
                                  join t in Db.Tipo_rutina
                                  on m.Id_tipo_rutina equals t.Id_tipo_rutina
                                  join mes in Db.Meses
                                  on m.Id_mes equals mes.Id_mes
                                  join es in Db.Estatus
                                  on m.Id_estatus equals es.Id_estatus
                                  join te in Db.Tipo_entrenamiento
                                  on m.Id_tipo_entrenamiento equals te.Id_tipo_entrenamiento
                                  where m.Id_cliente == IdCliente
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

            if (listmensualidadMdl == null || listmensualidadMdl.Count == 0)
            {
                listmensualidadMdl = new List<MensualidadModel>();
            }
            else
            {
                for (int i = 0; i < listmensualidadMdl.Count; i++)
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
                        listmensualidadMdl[i].PagoMes.Id_pago = pago.Id_pago;
                        listmensualidadMdl[i].PagoMes.Ubicacion_imagen_pago = pago.Ubicacion_imagen_pago;
                    }
                    else
                    {
                        listmensualidadMdl[i].PagoMes.Id_pago = 0;
                    }
                    

                }


            }
            return View(listmensualidadMdl);
        }

        public bool PagoMes(HttpPostedFileBase imagen,int Idmes,double monto,string descripcion) 
        {
            string Id = Session["Id_Cliente"].ToString();
            int IdCliente = int.Parse(Id);
            string ruta = string.Empty;
            var cliente = Db.Clientes.Where(y => y.Id_cliente == IdCliente).FirstOrDefault();
            var meses = Db.Mensualidad.Where(y => y.Id_mensualidad == Idmes).FirstOrDefault();
            if (meses.Id_estatus == 1)
            {
                pagos_Mensualidades = new Pagos_mensualidades
                {
                    Id_mensualidad = Idmes,
                    Monto = (decimal)monto,
                    Descripcion = descripcion,
                    Fecha_pago = DateTime.Now,
                    Ubicacion_imagen_pago = "/Imagenes/Clientes/" + cliente.Clave_cliente + "/" + "Mes" + Idmes + "" + imagen.FileName.ToString()
                };
                ruta = pagos_Mensualidades.Ubicacion_imagen_pago;
                Db.Pagos_mensualidades.Add(pagos_Mensualidades);
                if (Db.SaveChanges() == 1)
                {
                    try
                    {
                        DateTimeFormatInfo fechastring = CultureInfo.GetCultureInfo("es-ES").DateTimeFormat;
                        string Mes = string.Empty, Dia = string.Empty;
                        Mes = fechastring.GetMonthName(DateTime.Now.Month);
                        Dia = fechastring.GetDayName(DateTime.Now.DayOfWeek);
                        string Fecha= Dia + " " + DateTime.Now.Day.ToString() + " de " + Mes + " de " + DateTime.Now.Year;
                        imagen.SaveAs(Server.MapPath("~" + ruta));                        
                        Dictionary<string, string> datoscorreo = new Dictionary<string, string>();
                        string nombre = cliente.Nombres +" "+ cliente.Apellido_paterno + " " + cliente.Apellido_materno;                                               
                        datoscorreo.Add("@Names", nombre);
                        datoscorreo.Add("@Email", cliente.Correo_electronico);
                        datoscorreo.Add("@Phone", cliente.Telefono.ToString());
                        datoscorreo.Add("@Money", monto.ToString());
                        datoscorreo.Add("@Date", Fecha);
                        datoscorreo.Add("@Comments", descripcion);
                        datoscorreo.Add("@Urlbicacion", ruta);
                        string plantilla = Server.MapPath("~/Plantillas/Comprobantepago.html");
                        string succes = "Se envio tu solicitud de pago para la rutina a tu entrenador";
                        string user = "paradavalerio@gmail.com";
                        string asunto = "Comprobante de pago";
                        envio.EnviarCorreo(user, plantilla, datoscorreo, succes, asunto);
                        return true;
                    }
                    catch (Exception ex)
                    {                        
                        return false;
                    }                    
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

        public bool UpdateFoto(HttpPostedFileBase imagen)
        {
            string Id = Session["Id_Cliente"].ToString();
            Id = Id == null ? "0" : Id;
            int IdCliente = int.Parse(Id);
            string exception = string.Empty;
            string ruta = string.Empty;

            if (IdCliente != 0) 
            {
                clientes = Db.Clientes.Where(y => y.Id_cliente == IdCliente).FirstOrDefault();
                var file = HttpContext.Server.MapPath("~" + clientes.Foto_perfil);
                try
                {
                    System.IO.File.Delete(file);
                }
                catch (Exception ex)
                {
                    exception = ex.Message;
                }
                ruta = "/Imagenes/Clientes/" + clientes.Clave_cliente + "/" + imagen.FileName.ToString();
                clientes.Foto_perfil = ruta;
                if (Db.SaveChanges() == 1)
                {
                    try
                    {
                        imagen.SaveAs(Server.MapPath("~" + ruta));
                        return true;
                    }
                    catch (Exception ex)
                    {
                        exception = ex.Message;
                        return false;
                    }
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

        //Obtener los sets a realizar por dia
        [HttpGet]
        public JsonResult GetDetalleRutinaSets(int IdMensualidad = 0, int IdEstatusMes = 0, int IdDIa = 0)
        {
            var mes = Db.Mensualidad.Where(y => y.Id_mensualidad == IdMensualidad).FirstOrDefault();
            IdEstatusMes = mes.Id_estatus;
            if (IdEstatusMes == 2 || IdEstatusMes == 4)
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
            }
            return Json(listdetallerutinaMdl,JsonRequestBehavior.AllowGet);
        }

        //Obtener ejercicios por set
        [HttpGet]
        public JsonResult GetDetalleRutinaEjercicios(int IdMensualidad = 0, int IdEstatusMes = 0, int IdDIa = 0, int TipoSet = 0)
        {
            var mes = Db.Mensualidad.Where(y => y.Id_mensualidad == IdMensualidad).FirstOrDefault();
            IdEstatusMes = mes.Id_estatus;
            if (IdEstatusMes == 2 || IdEstatusMes == 4)
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
            }

            return Json(listdetallerutinaMdl, JsonRequestBehavior.AllowGet);
        }

        //Obtener mis medidas
        [HttpGet]
        public JsonResult ListAsesoriaantropometria(int Id_mensualidad)
        {
            DateTimeFormatInfo fechastring = CultureInfo.GetCultureInfo("es-ES").DateTimeFormat;
            string Mes = string.Empty, Dia = string.Empty;
            listantropometriaMdl = (from a in Db.Asesoria_antropometria
                                    where a.Id_mensualidad == Id_mensualidad
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
                                    }).ToList();

            foreach(var item in listantropometriaMdl)
            {
                Mes = fechastring.GetMonthName(item.Fecha_registro.Month);
                Dia = fechastring.GetDayName(item.Fecha_registro.DayOfWeek);
                item.Fecharegistro = Dia + " " + item.Fecha_registro.Day.ToString() + " de " + Mes + " de " + item.Fecha_registro.Year; 
            }

            return Json(listantropometriaMdl, JsonRequestBehavior.AllowGet);
        }
        
        //Hacer un nuevo registro de medidas
        public string RegistrarMedidas(AntropometriaModel antropometriaModel)
        {
            string result = "False";
            string fotoperfil = "/Imagenes/Clientes/";
            try
            {
                string Id = Session["Id_Cliente"].ToString();
                Id = Id == null ? "0" : Id;
                int IdCliente = int.Parse(Id);
                if (IdCliente != 0)
                {
                    clientes = Db.Clientes.Where(y => y.Id_cliente == IdCliente).FirstOrDefault();
                    fotoperfil += "/" + clientes.Clave_cliente;
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
                        Foto_frontal = fotoperfil,
                        Foto_perfil = fotoperfil,
                        Foto_posterior = fotoperfil,
                        Fecha_registro = DateTime.Now
                    };
                    Db.Asesoria_antropometria.Add(asesoria_antropometria);
                    if (Db.SaveChanges() == 1)
                    {
                        TempData["idmedidas"] = asesoria_antropometria.Id;
                        TempData["Name"] = fotoperfil;
                        result = "True";
                    }
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        public string RegistrarFotos(HttpPostedFileBase frontal,HttpPostedFileBase perfil,HttpPostedFileBase posterior)
        {
            string result = "False";
            try
            {
                string medida = TempData["idmedidas"].ToString();
                string name = TempData["Name"].ToString();
                int Idmedida = int.Parse(medida);
                asesoria_antropometria = Db.Asesoria_antropometria.Where(y => y.Id == Idmedida).FirstOrDefault();
                if (frontal != null)
                {
                    asesoria_antropometria.Foto_frontal += "/" + frontal.FileName.ToString();
                    Db.SaveChanges();
                    frontal.SaveAs(Server.MapPath("~/"+name +"/" + frontal.FileName.ToString()));
                    result = "True";
                }
                if (perfil != null)
                {
                    asesoria_antropometria.Foto_perfil += "/" + perfil.FileName.ToString();
                    Db.SaveChanges();
                    perfil.SaveAs(Server.MapPath("~/" + name + "/" + perfil.FileName.ToString()));
                    result = "True";
                }
                if (posterior != null)
                {
                    asesoria_antropometria.Foto_posterior += "/" + posterior.FileName.ToString();
                    Db.SaveChanges();
                    posterior.SaveAs(Server.MapPath("~/" + name + "/" + posterior.FileName.ToString()));
                    result = "True";
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }            
            return result;
        }
    }
}