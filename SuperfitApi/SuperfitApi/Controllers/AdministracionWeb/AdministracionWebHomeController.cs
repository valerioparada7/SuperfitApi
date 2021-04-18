using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SuperfitApi.Models.Entity;
using SuperfitApi.Models;
using System.Web.Mvc;
using System.IO;

namespace SuperfitApi.Controllers.AdministracionWeb.CatalogosWeb
{
    public class AdministracionWebHomeController : Controller
    {
        #region Variables
        public SuperfitEntities Db;
        public Clientes clientes;
        public Cuestionario cuestionario;
        public Mensualidad mensualidad;
        public Asesoria_antropometria asesoria_antropometria;
        public Tipo_rutina tipo_Rutina;
        public Tipo_entrenamiento tipo_Entrenamiento;
        public Ejercicios ejercicios;
        public Estatus estatus;

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
        

        //listas 
        public List<ClientesModel> listclientesMdl;
        public List<MensualidadModel> listmensualidadMdl;
        public List<TiporutinaModel> listtiporutinaMdl;        
        public List<TipoentrenamientoModel> listtipoentrenamientoMdl;
        public List<EjerciciosModel> listejerciciosMdl;
        public List<EstatusModel> listestatusMdl;
        public List<AntropometriaModel> listantropometriaMdl;

        #endregion
        public AdministracionWebHomeController()
        {
            Db = new SuperfitEntities();
            clientes = new Clientes();
            cuestionario = new Cuestionario();
            mensualidad = new Mensualidad();
            asesoria_antropometria = new Asesoria_antropometria();
            tipo_Rutina = new Tipo_rutina();
            tipo_Entrenamiento = new Tipo_entrenamiento();
            ejercicios = new Ejercicios();

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

            //listas
            listclientesMdl = new List<ClientesModel>();
            listmensualidadMdl = new List<MensualidadModel>();
            listtiporutinaMdl = new List<TiporutinaModel>();
            listtipoentrenamientoMdl = new List<TipoentrenamientoModel>();
            listejerciciosMdl = new List<EjerciciosModel>();
            listestatusMdl = new List<EstatusModel>();
            listantropometriaMdl = new List<AntropometriaModel>();

        }
        // GET: AdministracionWebHome
        public ActionResult AdministracionWebHome()
        {
            return View();
        }

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
                                       Estado = l.Estado,
                                       Contraseña = l.Contraseña,
                                       Foto_perfil = l.Foto_perfil,
                                       Sexo = l.Sexo
                                   }).ToList();

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
                string Clave = "";
                Clave = clientesModel.Nombres.Substring(0, 3) + clientesModel.Apellido_paterno.Substring(0, 3) +
                        clientesModel.Apellido_materno.Substring(0, 3);
                string Name = clientesModel.Nombres +" "+ clientesModel.Apellido_paterno +" "+
                        clientesModel.Apellido_materno;
                Name = Name.Replace(" ", "_");
                string fotoperfil = "Imagenes/Clientes/" + Name;
                clientes = new Clientes
                {
                    Clave_cliente = Clave,
                    Nombres = clientesModel.Nombres,
                    Apellido_paterno = clientesModel.Apellido_paterno,
                    Apellido_materno = clientesModel.Apellido_materno,
                    Edad = clientesModel.Edad,
                    Telefono = clientesModel.Telefono == null  ? 0 : clientesModel.Telefono,
                    Correo_electronico = clientesModel.Correo_electronico == null ? "" : clientesModel.Correo_electronico,
                    Apodo = clientesModel.Apodo == null ? "" : clientesModel.Apodo,
                    Contraseña = clientesModel.Contraseña,
                    Foto_perfil = fotoperfil,
                    Sexo = clientesModel.Sexo,
                    Estado = true
                };
                Db.Clientes.Add(clientes);
                if (Db.SaveChanges() == 1)
                {
                    result=  true;
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
                               Estado = l.Estado,
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
            clientes.Nombres            = clientesModel.Nombres;
            clientes.Apellido_paterno   = clientesModel.Apellido_paterno;
            clientes.Apellido_materno   = clientesModel.Apellido_materno;
            clientes.Apodo              = clientesModel.Apodo == null ? "" : clientesModel.Apodo;
            clientes.Edad               = clientesModel.Edad;
            clientes.Telefono = clientesModel.Telefono == null ? 0 : clientesModel.Telefono;
            clientes.Correo_electronico = clientesModel.Correo_electronico == null ? "" : clientesModel.Correo_electronico;            
            clientes.Contraseña         = clientesModel.Contraseña;            
            clientes.Sexo               = clientesModel.Sexo;            
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

        #region Usuarios

        #endregion

        #region Tiporutina
        public ActionResult AdminTiporutinaWeb()
        {
            listtiporutinaMdl = (from l in Db.Tipo_rutina
                                select new TiporutinaModel()
                                {
                                    Id_tiporutina = l.Id_tipo_rutina ,
                                    Descripcion = l.Descripcion,
                                    Tipo = l.Tipo,
                                }).ToList();

            return View(listtiporutinaMdl);
        }
        
        [HttpPost]
        public bool AgregarTiporutina(TiporutinaModel tiporutinaModel)
        {
            bool result = false;
            tipo_Rutina = new Tipo_rutina
            {
                Descripcion = tiporutinaModel.Descripcion,
                Tipo = tiporutinaModel.Tipo,                
            };
            Db.Tipo_rutina.Add(tipo_Rutina);
            if (Db.SaveChanges() == 1)
            {
                result = true;
            }

            return result;
        }

        public JsonResult EditarTiporutina(int Id_tipo_rutina)
        {
            tiporutinaMdl = (from t in Db.Tipo_rutina
                           where t.Id_tipo_rutina == Id_tipo_rutina
                             select new TiporutinaModel()
                           {
                               Id_tiporutina =t.Id_tipo_rutina,
                               Descripcion = t.Descripcion,
                               Tipo = t.Tipo
                            }).FirstOrDefault();

            return Json(tiporutinaMdl, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public bool EditarTiporutina(TiporutinaModel tiporutinaModel)
        {
            tipo_Rutina = Db.Tipo_rutina.Where(t => t.Id_tipo_rutina == tiporutinaModel.Id_tiporutina).FirstOrDefault();

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
        /*
        public ActionResult AdminTipoentrenamientoWeb()
        {
            listtipoentrenamientoMdl = (from l in Db.Tipo_entrenamiento
                                 select new TipoentrenamientoModel ()
                                 {
                                     Id_TipoEntrenamiento = l.Id_tipo_entrenamiento,
                                     Clave_Entrenamiento = l.Clave_entrenamiento,
                                     Tipo_entrenamiento = l.Tipo_entrenamientos,
                                 }).ToList();

            return View(listtipoentrenamientoMdl);
        }

        [HttpPost]
        public bool AgregarTipoentrenamiento(TipoentrenamientoModel tipoentrenamientoModel)
        {
            bool result = false;
            tipo_Entrenamiento = new Tipo_entrenamiento
            {                
                Clave_entrenamiento = tipoentrenamientoModel.Clave_Entrenamiento,
                Tipo_entrenamientos = tipoentrenamientoModel.Tipo_entrenamiento,
            };
            Db.Tipo_entrenamiento.Add(tipo_Entrenamiento);
            if (Db.SaveChanges() == 1)
            {
                result = true;
            }

            return result;
        }

        public JsonResult EditarTipoentrenamiento(int Id_tipo_entrenamiento)
        {
            tiporutinaMdl = (from t in Db.Tipo_rutina
                             where t.Id_tipo_rutina == Id_tipo_entrenamiento
                             select new TiporutinaModel()
                             {
                                 Id_tiporutina = t.Id_tipo_rutina,
                                 Descripcion = t.Descripcion,
                                 Tipo = t.Tipo
                             }).FirstOrDefault();

            return Json(tiporutinaMdl, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public bool EditarTipoentrenamiento(TipoentrenamientoModel tipoentrenamientoModel)
        {
            tipo_Entrenamiento = Db.Tipo_entrenamiento.Where(t => t.Id_tipo_entrenamiento == tipoentrenamientoModel.Id_TipoEntrenamiento).FirstOrDefault();

            tipo_Entrenamiento.Clave_entrenamiento = tipoentrenamientoModel.Clave_Entrenamiento;
            tipo_Entrenamiento.Tipo_entrenamientos = tipoentrenamientoModel.Tipo_entrenamiento;

            if (Db.SaveChanges() == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }*/
        #endregion

        #region Rutinas
/*
 * public ActionResult AdminTipoentrenamientoWeb()
{
    listtipoentrenamientoMdl = (from l in Db.Tipo_entrenamiento
                         select new TipoentrenamientoModel ()
                         {
                             Id_TipoEntrenamiento = l.Id_tipo_entrenamiento,
                             Clave_Entrenamiento = l.Clave_entrenamiento,
                             Tipo_entrenamiento = l.Tipo_entrenamientos,
                         }).ToList();

    return View(listtipoentrenamientoMdl);
}

[HttpPost]
public bool AgregarTipoentrenamiento(TipoentrenamientoModel tipoentrenamientoModel)
{
    bool result = false;
    tipo_Entrenamiento = new Tipo_entrenamiento
    {                
        Clave_entrenamiento = tipoentrenamientoModel.Clave_Entrenamiento,
        Tipo_entrenamientos = tipoentrenamientoModel.Tipo_entrenamiento,
    };
    Db.Tipo_entrenamiento.Add(tipo_Entrenamiento);
    if (Db.SaveChanges() == 1)
    {
        result = true;
    }

    return result;
}

public JsonResult EditarTipoentrenamiento(int Id_tipo_entrenamiento)
{
    tiporutinaMdl = (from t in Db.Tipo_rutina
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
public bool EditarTipoentrenamiento(TipoentrenamientoModel tipoentrenamientoModel)
{
    tipo_Entrenamiento = Db.Tipo_entrenamiento.Where(t => t.Id_tipo_entrenamiento == tipoentrenamientoModel.Id_TipoEntrenamiento).FirstOrDefault();

    tipo_Entrenamiento.Clave_entrenamiento = tipoentrenamientoModel.Clave_Entrenamiento;
    tipo_Entrenamiento.Tipo_entrenamientos = tipoentrenamientoModel.Tipo_entrenamiento;

    if (Db.SaveChanges() == 1)
    {
        return true;
    }
    else
    {
        return false;
    }
}
 */
#endregion

        #region Ejercicios
        /*public ActionResult AdminTipoentrenamientoWeb()
        {
            listtipoentrenamientoMdl = (from l in Db.Tipo_entrenamiento
                                 select new TipoentrenamientoModel ()
                                 {
                                     Id_TipoEntrenamiento = l.Id_tipo_entrenamiento,
                                     Clave_Entrenamiento = l.Clave_entrenamiento,
                                     Tipo_entrenamiento = l.Tipo_entrenamientos,
                                 }).ToList();

            return View(listtipoentrenamientoMdl);
        }

        [HttpPost]
        public bool AgregarTipoentrenamiento(TipoentrenamientoModel tipoentrenamientoModel)
        {
            bool result = false;
            tipo_Entrenamiento = new Tipo_entrenamiento
            {                
                Clave_entrenamiento = tipoentrenamientoModel.Clave_Entrenamiento,
                Tipo_entrenamientos = tipoentrenamientoModel.Tipo_entrenamiento,
            };
            Db.Tipo_entrenamiento.Add(tipo_Entrenamiento);
            if (Db.SaveChanges() == 1)
            {
                result = true;
            }

            return result;
        }

        public JsonResult EditarTipoentrenamiento(int Id_tipo_entrenamiento)
        {
            tiporutinaMdl = (from t in Db.Tipo_rutina
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
        public bool EditarTipoentrenamiento(TipoentrenamientoModel tipoentrenamientoModel)
        {
            tipo_Entrenamiento = Db.Tipo_entrenamiento.Where(t => t.Id_tipo_entrenamiento == tipoentrenamientoModel.Id_TipoEntrenamiento).FirstOrDefault();

            tipo_Entrenamiento.Clave_entrenamiento = tipoentrenamientoModel.Clave_Entrenamiento;
            tipo_Entrenamiento.Tipo_entrenamientos = tipoentrenamientoModel.Tipo_entrenamiento;

            if (Db.SaveChanges() == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }*/
        #endregion

        #region Estatus
        /*public ActionResult AdminTipoentrenamientoWeb()
                {
                    listtipoentrenamientoMdl = (from l in Db.Tipo_entrenamiento
                                         select new TipoentrenamientoModel ()
                                         {
                                             Id_TipoEntrenamiento = l.Id_tipo_entrenamiento,
                                             Clave_Entrenamiento = l.Clave_entrenamiento,
                                             Tipo_entrenamiento = l.Tipo_entrenamientos,
                                         }).ToList();

                    return View(listtipoentrenamientoMdl);
                }

                [HttpPost]
                public bool AgregarTipoentrenamiento(TipoentrenamientoModel tipoentrenamientoModel)
                {
                    bool result = false;
                    tipo_Entrenamiento = new Tipo_entrenamiento
                    {                
                        Clave_entrenamiento = tipoentrenamientoModel.Clave_Entrenamiento,
                        Tipo_entrenamientos = tipoentrenamientoModel.Tipo_entrenamiento,
                    };
                    Db.Tipo_entrenamiento.Add(tipo_Entrenamiento);
                    if (Db.SaveChanges() == 1)
                    {
                        result = true;
                    }

                    return result;
                }

                public JsonResult EditarTipoentrenamiento(int Id_tipo_entrenamiento)
                {
                    tiporutinaMdl = (from t in Db.Tipo_rutina
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
                public bool EditarTipoentrenamiento(TipoentrenamientoModel tipoentrenamientoModel)
                {
                    tipo_Entrenamiento = Db.Tipo_entrenamiento.Where(t => t.Id_tipo_entrenamiento == tipoentrenamientoModel.Id_TipoEntrenamiento).FirstOrDefault();

                    tipo_Entrenamiento.Clave_entrenamiento = tipoentrenamientoModel.Clave_Entrenamiento;
                    tipo_Entrenamiento.Tipo_entrenamientos = tipoentrenamientoModel.Tipo_entrenamiento;

                    if (Db.SaveChanges() == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }*/
        #endregion

        #region Mensualidad
        /*public ActionResult AdminTipoentrenamientoWeb()
                {
                    listtipoentrenamientoMdl = (from l in Db.Tipo_entrenamiento
                                         select new TipoentrenamientoModel ()
                                         {
                                             Id_TipoEntrenamiento = l.Id_tipo_entrenamiento,
                                             Clave_Entrenamiento = l.Clave_entrenamiento,
                                             Tipo_entrenamiento = l.Tipo_entrenamientos,
                                         }).ToList();

                    return View(listtipoentrenamientoMdl);
                }

                [HttpPost]
                public bool AgregarTipoentrenamiento(TipoentrenamientoModel tipoentrenamientoModel)
                {
                    bool result = false;
                    tipo_Entrenamiento = new Tipo_entrenamiento
                    {                
                        Clave_entrenamiento = tipoentrenamientoModel.Clave_Entrenamiento,
                        Tipo_entrenamientos = tipoentrenamientoModel.Tipo_entrenamiento,
                    };
                    Db.Tipo_entrenamiento.Add(tipo_Entrenamiento);
                    if (Db.SaveChanges() == 1)
                    {
                        result = true;
                    }

                    return result;
                }

                public JsonResult EditarTipoentrenamiento(int Id_tipo_entrenamiento)
                {
                    tiporutinaMdl = (from t in Db.Tipo_rutina
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
                public bool EditarTipoentrenamiento(TipoentrenamientoModel tipoentrenamientoModel)
                {
                    tipo_Entrenamiento = Db.Tipo_entrenamiento.Where(t => t.Id_tipo_entrenamiento == tipoentrenamientoModel.Id_TipoEntrenamiento).FirstOrDefault();

                    tipo_Entrenamiento.Clave_entrenamiento = tipoentrenamientoModel.Clave_Entrenamiento;
                    tipo_Entrenamiento.Tipo_entrenamientos = tipoentrenamientoModel.Tipo_entrenamiento;

                    if (Db.SaveChanges() == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }*/
        #endregion

        #region Asesoriaantropometria
        /*public ActionResult AdminTipoentrenamientoWeb()
        {
            listtipoentrenamientoMdl = (from l in Db.Tipo_entrenamiento
                                 select new TipoentrenamientoModel ()
                                 {
                                     Id_TipoEntrenamiento = l.Id_tipo_entrenamiento,
                                     Clave_Entrenamiento = l.Clave_entrenamiento,
                                     Tipo_entrenamiento = l.Tipo_entrenamientos,
                                 }).ToList();

            return View(listtipoentrenamientoMdl);
        }

        [HttpPost]
        public bool AgregarTipoentrenamiento(TipoentrenamientoModel tipoentrenamientoModel)
        {
            bool result = false;
            tipo_Entrenamiento = new Tipo_entrenamiento
            {                
                Clave_entrenamiento = tipoentrenamientoModel.Clave_Entrenamiento,
                Tipo_entrenamientos = tipoentrenamientoModel.Tipo_entrenamiento,
            };
            Db.Tipo_entrenamiento.Add(tipo_Entrenamiento);
            if (Db.SaveChanges() == 1)
            {
                result = true;
            }

            return result;
        }

        public JsonResult EditarTipoentrenamiento(int Id_tipo_entrenamiento)
        {
            tiporutinaMdl = (from t in Db.Tipo_rutina
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
        public bool EditarTipoentrenamiento(TipoentrenamientoModel tipoentrenamientoModel)
        {
            tipo_Entrenamiento = Db.Tipo_entrenamiento.Where(t => t.Id_tipo_entrenamiento == tipoentrenamientoModel.Id_TipoEntrenamiento).FirstOrDefault();

            tipo_Entrenamiento.Clave_entrenamiento = tipoentrenamientoModel.Clave_Entrenamiento;
            tipo_Entrenamiento.Tipo_entrenamientos = tipoentrenamientoModel.Tipo_entrenamiento;

            if (Db.SaveChanges() == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }*/
        #endregion

    }
}