using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using SuperfitApi.Models;
using SuperfitApi.Models.Entity;

namespace SuperfitApi.Controllers
{
    public class ClientesController : ApiController
    {   
        //config.Formatters.Remove(config.Formatters.XmlFormatter); eso ta en webconfig de app
        #region Variables 
        public SuperfitEntities Db;
        public Clientes clientes;
        public Cuestionarios cuestionario;
        public Mensualidades mensualidad;
        public Asesoria_antropometria asesoria_antropometria;
        public Pagos_mensualidades pagos_Mensualidades;
        public EnvioNotificaciones envio;
        //modelos
        public ClientesModel clientesMdl;
        public CuestionarioModel cuestionarioMdl;
        public MensualidadModel mensualidadMdl;
        public AntropometriaModel asesoria_antropometriaMdl;
        public DetallerutinaModel detallerutinaMdl;
        public AlertasModel alertasMdl;
        public PagosmensualModel PagosmensualMdl;
        //listas de modelos
        public List<DetallerutinaModel> listdetallerutinaMdl;
        public List<MensualidadModel> listmensualidadMdl;
        public List<AntropometriaModel> listAntropometriaMdl;
        #endregion
        public ClientesController()
        {
            Db = new SuperfitEntities();
            clientes = new Clientes();
            pagos_Mensualidades = new Pagos_mensualidades();
            envio = new EnvioNotificaciones();
            //modelos
            clientesMdl = new ClientesModel();
            cuestionarioMdl = new CuestionarioModel();
            mensualidadMdl = new MensualidadModel();
            asesoria_antropometriaMdl = new AntropometriaModel();            
            detallerutinaMdl = new DetallerutinaModel();
            alertasMdl = new AlertasModel();
            PagosmensualMdl = new PagosmensualModel();
            //listas de modelos
            listdetallerutinaMdl = new List<DetallerutinaModel>();
            listmensualidadMdl = new List<MensualidadModel>();
            listAntropometriaMdl = new List<AntropometriaModel>();
        }
        //Obtener los datos para su perfil del cliente
        [HttpGet]
        [Route("api/Clientes/GetCliente")]
        public ClientesModel GetCliente(int Id_cliente)
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

            if (clientesMdl == null)
            {
                clientesMdl = new ClientesModel();
            }

            return clientesMdl;
        }

        [HttpPut]
        [Route("api/Clientes/UpdateCliente")]
        public bool UpdateCliente(ClientesModel ClientesMdl)
        {
            int IdCliente = ClientesMdl.Id_cliente ;
            clientes = Db.Clientes.Where(p => p.Id_cliente == IdCliente).FirstOrDefault();
            if (clientes != null)
            {
                clientes.Nombres = ClientesMdl.Nombres;
                clientes.Apellido_paterno = ClientesMdl.Apellido_paterno;
                clientes.Apellido_materno = ClientesMdl.Apellido_materno;
                clientes.Edad = ClientesMdl.Edad;
                clientes.Telefono = ClientesMdl.Telefono;
                clientes.Correo_electronico = ClientesMdl.Correo_electronico;
                clientes.Apodo = ClientesMdl.Apodo;
                clientes.Contraseña = ClientesMdl.Contraseña;
                clientes.Sexo = ClientesMdl.Sexo;
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
        
        [HttpPost]
        [Route("api/Clientes/UpdateClienteFoto")]
        public bool UpdateClienteFoto(Imagenes imagenes,int Id_cliente)
        {
            string exception = string.Empty;
            string ruta = string.Empty;
            if (Id_cliente != 0)
            {
                clientes = Db.Clientes.Where(y => y.Id_cliente == Id_cliente).FirstOrDefault();
                var file = HostingEnvironment.MapPath("~" + clientes.Foto_perfil);
                try
                {
                    System.IO.File.Delete(file);
                }
                catch (Exception ex)
                {
                    exception = ex.Message;
                }       
                ruta = "/Imagenes/Clientes/" + clientes.Clave_cliente + "/" + "Foto_cuenta" + clientes.Clave_cliente + ".jpg";
                if (clientes.Foto_perfil != ruta)
                {
                    clientes.Foto_perfil = ruta;
                    Db.SaveChanges();
                }
                try
                {
                    string newruta = HostingEnvironment.MapPath("~/Imagenes/Clientes/" + clientes.Clave_cliente + "/" + "Foto_cuenta" + clientes.Clave_cliente + ".jpg");
                    byte[] Foto_perfil = Convert.FromBase64String(imagenes.ImagenPerfilCuenta);
                    using (var ms = new MemoryStream(Foto_perfil, 0, Foto_perfil.Length))
                    {
                        Image image = Image.FromStream(ms, true);
                        image.Save(newruta);
                    }
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

        //Obtener los sets a realizar por dia
        [HttpGet]
        [Route("api/Clientes/GetDetalleRutinaSets")]
        public List<DetallerutinaModel> GetDetalleRutinaSets(int IdMensualidad,int IdEstatusMes,int IdDIa)
        {
            var mes = Db.Mensualidades.Where(y => y.Id_mensualidad == IdMensualidad).FirstOrDefault();
            IdEstatusMes = mes.Id_estatus;
            if (IdEstatusMes == 2 || IdEstatusMes == 4)
            {
                listdetallerutinaMdl = (from d in Db.Detalle_rutinas
                                        join m in Db.Mensualidades
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
            return listdetallerutinaMdl;
        }

        //Obtener ejercicios por set
        [HttpGet]
        [Route("api/Clientes/GetDetalleRutinaEjercicios")]
        public List<DetallerutinaModel> GetDetalleRutinaEjercicios(int IdMensualidad, int IdEstatusMes, int IdDIa,int TipoSet)
        {
            var mes = Db.Mensualidades.Where(y => y.Id_mensualidad == IdMensualidad).FirstOrDefault();
            IdEstatusMes = mes.Id_estatus;
            if (IdEstatusMes == 2 || IdEstatusMes == 4)
            {
                listdetallerutinaMdl = (from d in Db.Detalle_rutinas
                                        join e in Db.Ejercicios
                                        on d.Id_ejercicio equals e.Id_ejercicio
                                        join m in Db.Mensualidades
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
            return listdetallerutinaMdl;
        }

        //Obtener mi mensualidad ultima que esta corriendo
        [HttpGet]
        [Route("api/Clientes/GetMensualidad")]
        public MensualidadModel GetMensualidad(int IdCliente)
        {
            try
            {
                DateTimeFormatInfo fechastring = CultureInfo.GetCultureInfo("es-ES").DateTimeFormat;
                string Mes = string.Empty, Dia = string.Empty, finalizar = string.Empty;
                clientesMdl = (from c in Db.Clientes
                               where c.Id_cliente == IdCliente
                               select new ClientesModel()
                               {
                                   Id_cliente = c.Id_cliente,
                                   Nombres = c.Nombres,
                                   Foto_perfil = c.Foto_perfil,
                                   Validar=true                                   
                               }).FirstOrDefault();

                var list = Db.Mensualidades.Where(p => p.Id_cliente == IdCliente).ToList();
                var mensualidad = list.OrderByDescending(p => p.Fecha_fin).FirstOrDefault();
                if (mensualidad != null)
                {
                    int id = mensualidad.Id_mensualidad;
                    mensualidadMdl = (from m in Db.Mensualidades
                                      join t in Db.Tipo_rutinas
                                      on m.Id_tipo_rutina equals t.Id_tipo_rutina
                                      join te in Db.Tipo_entrenamientos
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

                    mensualidadMdl.Cliente = new ClientesModel();
                    mensualidadMdl.Cliente = clientesMdl;
                    Mes = fechastring.GetMonthName(mensualidadMdl.Fecha_fin.Month);
                    Dia = fechastring.GetDayName(mensualidadMdl.Fecha_fin.DayOfWeek);
                    mensualidadMdl.Fechafin = Dia + " " + mensualidadMdl.Fecha_fin.Day.ToString() + " de " + Mes + " de " + mensualidadMdl.Fecha_fin.Year;
                    Mes = fechastring.GetMonthName(mensualidadMdl.Fecha_inicio.Month);
                    Dia = fechastring.GetDayName(mensualidadMdl.Fecha_inicio.DayOfWeek);
                    mensualidadMdl.Fechainicio = Dia + " " + mensualidadMdl.Fecha_inicio.Day.ToString() + " de " + Mes + " de " + mensualidadMdl.Fecha_inicio.Year;
                    if ((mensualidadMdl.Estatus.Id_estatus == 2 || mensualidadMdl.Estatus.Id_estatus == 4) && DateTime.Now.AddDays(5) >= mensualidadMdl.Fecha_fin)
                    {
                        finalizar = "Tu mes esta por acabar";
                        mensualidadMdl.Estatus.Descripcion = finalizar;
                    }
                    int idmensu = mensualidadMdl.Id_mensualidad;
                    var pago = Db.Pagos_mensualidades.Where(alitza => alitza.Id_mensualidad == idmensu).FirstOrDefault();
                    mensualidadMdl.PagoMes = new PagosmensualModel();
                    if (pago != null)
                    {
                        DateTime fecha = (DateTime)pago.Fecha_pago;
                        Mes = fechastring.GetMonthName(fecha.Month);
                        Dia = fechastring.GetDayName(fecha.DayOfWeek);
                        mensualidadMdl.PagoMes.Id_pago = pago.Id_pago;
                        mensualidadMdl.PagoMes.Fechapago = Dia + " " + fecha.Day.ToString() + " de " + Mes + " de " + fecha.Year;
                        mensualidadMdl.PagoMes.Ubicacion_imagen_pago = pago.Ubicacion_imagen_pago;
                    }
                    else
                    {
                        mensualidadMdl.PagoMes.Id_pago = 0;
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
                    mensualidadMdl.Cliente.Validar = true;
                    mensualidadMdl.Cliente = clientesMdl;
                    mensualidadMdl.Tiporutina.Tipo = "No asignado";
                    mensualidadMdl.TipoEntrenamiento.Tipo_entrenamiento = "No asignado";
                    mensualidadMdl.Fechafin = "Sin fecha asignada";
                    mensualidadMdl.Fechainicio = "Sin fecha asignada";                    
                }
            }
            catch (Exception ex)
            {
                mensualidadMdl.Fechainicio = ex.Message;
            }
            return mensualidadMdl;
        }

        //Obtener mis mensualidadades
        [HttpGet]
        [Route("api/Clientes/GetMensualidades")]
        public List<MensualidadModel> GetMensualidades(int IdCliente)
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

            return listmensualidadMdl;
        }

        //Pagar Mensualidad
        [HttpPost]
        [Route("api/Clientes/PagoMes")]
        public AlertasModel PagoMes(Imagenes imagenes, int IdCliente, int Idmes, double monto, string descripcion)
        {
            alertasMdl.Result = false;
            alertasMdl.Mensaje = "Ocurrio un error al enviar sus datos";
            string ruta = string.Empty;
            try
            {
                
                var cliente = Db.Clientes.Where(y => y.Id_cliente == IdCliente).FirstOrDefault();
                var meses = Db.Mensualidades.Where(y => y.Id_mensualidad == Idmes).FirstOrDefault();
                if (meses.Id_estatus == 1)
                {
                    pagos_Mensualidades = new Pagos_mensualidades
                    {
                        Id_mensualidad = Idmes,
                        Monto = (decimal)monto,
                        Descripcion = descripcion,
                        Fecha_pago = DateTime.Now,
                        Ubicacion_imagen_pago = "/Imagenes/Clientes/" + cliente.Clave_cliente + "/" + "Mes" + Idmes + "" + "Pago.jpg"
                    };
                    ruta = pagos_Mensualidades.Ubicacion_imagen_pago;
                    Db.Pagos_mensualidades.Add(pagos_Mensualidades);
                    if (Db.SaveChanges() == 1)
                    {
                        try
                        {
                            byte[] Foto_frontal = Convert.FromBase64String(imagenes.ImagenPerfil);
                            using (var ms = new MemoryStream(Foto_frontal, 0, Foto_frontal.Length))
                            {
                                ruta = HostingEnvironment.MapPath("~/Imagenes/Clientes/" + cliente.Clave_cliente + "/" + "Mes" + Idmes + "" + "Pago.jpg");
                                Image image = Image.FromStream(ms, true);
                                image.Save(ruta);
                                try
                                {
                                    DateTimeFormatInfo fechastring = CultureInfo.GetCultureInfo("es-ES").DateTimeFormat;
                                    string Mes = string.Empty, Dia = string.Empty;
                                    Mes = fechastring.GetMonthName(DateTime.Now.Month);
                                    Dia = fechastring.GetDayName(DateTime.Now.DayOfWeek);
                                    string Fecha = Dia + " " + DateTime.Now.Day.ToString() + " de " + Mes + " de " + DateTime.Now.Year;                                    
                                    Dictionary<string, string> datoscorreo = new Dictionary<string, string>();
                                    string nombre = cliente.Nombres + " " + cliente.Apellido_paterno + " " + cliente.Apellido_materno;
                                    datoscorreo.Add("@Names", nombre);
                                    datoscorreo.Add("@Email", cliente.Correo_electronico);
                                    datoscorreo.Add("@Phone", cliente.Telefono.ToString());
                                    datoscorreo.Add("@Money", monto.ToString());
                                    datoscorreo.Add("@Date", Fecha);
                                    datoscorreo.Add("@Comments", descripcion);
                                    string imagenhtml = "<img src=\"https://www.bsite.net/valerioparada\"" + ruta + " height = \"200\" width = \"200\" />";
                                    datoscorreo.Add("@Urlbicacion", imagenhtml);
                                    string plantilla = HostingEnvironment.MapPath("~/Plantillas/Comprobantepago.html");
                                    string succes = "Se envio tu solicitud de pago para la rutina a tu entrenador";
                                    string user = "paradavalerio@gmail.com";
                                    string asunto = "Comprobante de pago";
                                    alertasMdl = envio.EnviarCorreo(user, plantilla, datoscorreo, succes, asunto);
                                    return alertasMdl;
                                }
                                catch (Exception ex)
                                {
                                    alertasMdl.Mensaje = ex.Message;
                                    alertasMdl.Result = false;
                                    return alertasMdl;
                                }
                            }                            
                        }
                        catch (Exception ex)
                        {
                            Db.Pagos_mensualidades.Remove(pagos_Mensualidades);
                            Db.SaveChanges();
                            alertasMdl.Mensaje = ex.Message;
                            alertasMdl.Result = false;
                            return alertasMdl;
                        }
                    }
                    else
                    {
                        return alertasMdl;
                    }
                }
                else
                {
                    alertasMdl.Result = true;
                    alertasMdl.Mensaje = "El estatus no esta en pendiente verifique con su instructror el pago";
                    return alertasMdl;
                }
            }
            catch (Exception ex)
            {
                alertasMdl.Mensaje = ex.Message;
            }
            return alertasMdl;
        }

        //Obtener Mi cuestionario
        [HttpGet]
        [Route("api/Clientes/GetCuestionario")]
        public CuestionarioModel GetCuestionario(int IdCliente)
        {
            cuestionarioMdl = (from c in Db.Cuestionarios
                               where c.Id_cuestionario == IdCliente
                               select new CuestionarioModel()
                               {
                                    Id_cuestionario=c.Id_cuestionario,
                                    Clave_cuestionario=c.Clave_cuestionario,
                                    Padece_enfermedad =(bool)c.Padece_enfermedad,     
                                    Medicamento_prescrito_medico  =c.Medicamento_prescrito_medico ,     
                                    Lesiones= (bool)c.Lesiones,     
                                    Alguna_recomendacion_lesiones =c.Alguna_recomendacion_lesiones,     
                                    Fuma= (bool)c.Fuma,     
                                    Veces_semana_fuma= (int)c.Veces_semana_fuma,     
                                    Alcohol= (bool)c.Alcohol,     
                                    Veces_semana_alcohol= (int)c.Veces_semana_alcohol,     
                                    Actividad_fisica= (bool)c.Actividad_fisica,     
                                    Tipo_ejercicios =c.Tipo_ejercicios,     
                                    Tiempo_dedicado =c.Tiempo_dedicado,     
                                    Horario_entreno =c.Horario_entreno,     
                                    MetasObjetivos  =c.Metas_objetivos ,     
                                    Compromisos  =c.Compromisos,     
                                    Comentarios  =c.Comentarios,     
                                    Fecha_registro= (DateTime)c.Fecha_registro,
                                    Cliente = new ClientesModel
                                    {
                                        Id_cliente = IdCliente
                                    }

                               }).FirstOrDefault();

            return cuestionarioMdl;
        }

        //Obtener Mi cuestionario
        [HttpPost]
        [Route("api/Clientes/UpdateCuestionario")]
        public AlertasModel UpdateCuestionario(CuestionarioModel cuestionarioModel)
        {
            try
            {
                alertasMdl.Result = false;
                alertasMdl.Mensaje = "Ocurrio un problema al actualizar los datos";
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
                    cuestionario.Comentarios = cuestionarioModel.Comentarios == null ? " "+ cuestionario.Comentarios : cuestionarioModel.Comentarios;
                    cuestionario.Fecha_registro = DateTime.Now;
                    Db.SaveChanges();
                    alertasMdl.Result = true;
                }
                else
                {
                    alertasMdl.Mensaje = "No hay cuestionario asigando";
                }
            }
            catch (Exception ex)
            {
                alertasMdl.Mensaje = ex.Message;
            }
            return alertasMdl;
        }

        //Obtener las medidas por una mensualidad
        [HttpGet]
        [Route("api/Clientes/GetAsesoriaantropometria")]
        public List<AntropometriaModel> GetAsesoriaantropometria(int Id_mensualidad)
        {
            DateTimeFormatInfo fechastring = CultureInfo.GetCultureInfo("es-ES").DateTimeFormat;
            string Mes = string.Empty, Dia = string.Empty;
            listAntropometriaMdl = (from a in Db.Asesoria_antropometria
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

            if (listAntropometriaMdl.Count == 0)
            {
                listAntropometriaMdl = new List<AntropometriaModel>();
            }

            foreach (var item in listAntropometriaMdl)
            {
                Mes = fechastring.GetMonthName(item.Fecha_registro.Month);
                Dia = fechastring.GetDayName(item.Fecha_registro.DayOfWeek);
                item.Fecharegistro = Dia + " " + item.Fecha_registro.Day.ToString() + " de " + Mes + " de " + item.Fecha_registro.Year;
            }

            return listAntropometriaMdl;
        }

        //Registar una nueva medida a una mensualidad
        [HttpPost]
        [Route("api/Clientes/RegistrarMedidas")]
        public AlertasModel RegistrarMedidas(AntropometriaModel antropometriaModel,int Id_Cliente)
        {
            string result = "false";
            alertasMdl.Result = false;
            string fotoperfil = "/Imagenes/Clientes/";
            try
            {
                if (Id_Cliente != 0)
                {
                    clientes = Db.Clientes.Where(y => y.Id_cliente == Id_Cliente).FirstOrDefault();
                    fotoperfil += clientes.Clave_cliente;
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
                        Imagenes imagenes = new Imagenes
                        {
                            ImagenFrontal = antropometriaModel.Foto_frontal64,
                            ImagenPerfil =antropometriaModel.Foto_perfil64,
                            ImagenPosterior = antropometriaModel.Foto_posterior64
                        };
                        alertasMdl = UpdateImagenes(imagenes, clientes.Clave_cliente, asesoria_antropometria.Id, antropometriaModel.Mensualidad.Id_mensualidad);
                        if (alertasMdl.Mensaje == "True")
                        {
                            result = "Se registro correcto";
                        }
                        else
                        {
                            result = "Se registro correcto falto imagens";
                        }
                        alertasMdl.Result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            alertasMdl.Mensaje = result;
            return alertasMdl;
        }

        //Subimos las imagenes 
        public AlertasModel UpdateImagenes(Imagenes imagenes, string clavecliente,int Idmedidas,int idmensualidad)
        {
            alertasMdl.Result = true;
            alertasMdl.Mensaje = "1";
            int consecutivo = 0;
            string nomeclatura = string.Empty;
            try
            {
                asesoria_antropometria = Db.Asesoria_antropometria.Where(y => y.Id == Idmedidas).FirstOrDefault();
                consecutivo= Db.Asesoria_antropometria.Max(y => y.Id)+1;
                nomeclatura = clavecliente + consecutivo;
                if (!string.IsNullOrEmpty(imagenes.ImagenFrontal))
                {
                    byte[] Foto_frontal = Convert.FromBase64String(imagenes.ImagenFrontal);
                    asesoria_antropometria.Foto_frontal = asesoria_antropometria.Foto_frontal + "/" + "Foto_frontal"+ nomeclatura + ".jpg";
                    Db.SaveChanges();
                    string rutaFoto_frontal = HostingEnvironment.MapPath("~/Imagenes/Clientes/" + clavecliente + "/" + "Foto_frontal" + nomeclatura + ".jpg");
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
                    asesoria_antropometria.Foto_perfil = asesoria_antropometria.Foto_perfil + "/" + "Foto_lateral" + nomeclatura + ".jpg";
                    Db.SaveChanges();
                    string rutafotolateral = HostingEnvironment.MapPath("~/Imagenes/Clientes/" + clavecliente + "/" + "Foto_lateral" + nomeclatura + ".jpg");
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
                    asesoria_antropometria.Foto_posterior = asesoria_antropometria.Foto_posterior + "/" + "Foto_posterior" + nomeclatura + ".jpg";
                    Db.SaveChanges();
                    string fotoposterior = HostingEnvironment.MapPath("~/Imagenes/Clientes/" + clavecliente + "/" + "Foto_posterior" + nomeclatura + ".jpg");
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
    }
}