using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SuperfitApi.Models;


namespace SuperfitApi.Controllers.Catalogos
{
    public class CatalogoController : ApiController
    {
        public CatalogoController()
        {
            Db = new SuperflyfitEntities();
            listdias = new List<DiasModel>();
            listmeses = new List<MesesModel>();
            listejercicios = new List<EjerciciosModel>();
            listestatus = new List<EstatusModel>();
            listrutinas = new List<RutinasModel>();
            listtipoentrena = new List<TipoentrenamientoModel>();

            dias = new DiasModel();
            meses = new MesesModel();
            ejercicios = new EjerciciosModel();
            estatus = new EstatusModel();
            rutinas = new RutinasModel();
            tipoentrena = new TipoentrenamientoModel();
        }
        #region Variables
        public SuperflyfitEntities Db;

        public DiasModel dias;
        public MesesModel meses;
        public EjerciciosModel ejercicios;
        public EstatusModel estatus;
        public RutinasModel rutinas;
        public TiporutinaModel tiporutinas;
        public TipoentrenamientoModel tipoentrena;

        public List<DiasModel> listdias;
        public List<MesesModel> listmeses;
        public List<EjerciciosModel> listejercicios;
        public List<EstatusModel> listestatus;
        public List<RutinasModel> listrutinas;
        public List<TiporutinaModel> listtiporutinas;
        public List<TipoentrenamientoModel> listtipoentrena;

        #endregion

        //Obtener dias
        [HttpGet]
        [Route("api/Catalogo/GetDays")]
        public List<DiasModel> GetDays()
        {
            listdias = (from d in Db.Dias
                        select new DiasModel()
                        {
                            Id_dia=d.Id_dia,
                            Clave_dia =d.Clave_dia,
                            Dia=d.Dia
                        }).ToList();
            return listdias;
        }
        [HttpGet]        
        public DiasModel GetDays(int IdDay)
        {
           dias = (from d in Db.Dias
                        where d.Id_dia==IdDay
                        select new DiasModel()
                        {
                            Id_dia = d.Id_dia,
                            Clave_dia = d.Clave_dia,
                            Dia = d.Dia
                        }).FirstOrDefault();
            return dias;
        }

        //Obetener meses
        [HttpGet]
        [Route("api/Catalogo/GetMonths")]
        public List<MesesModel> GetMonths()
        {
            listmeses = (from m in Db.Meses                    
                    select new MesesModel()
                    {
                        Id_mes=m.Id_mes,
                        Clave_mes=m.Clave_mes,
                        Mes=m.Mes
                    }).ToList();
            return listmeses;
        }
        [HttpGet]
        public MesesModel GetMonths(int IdMonth)
        {
            meses = (from m in Db.Meses
                     where m.Id_mes==IdMonth
                         select new MesesModel()
                         {
                             Id_mes = m.Id_mes,
                             Clave_mes = m.Clave_mes,
                             Mes = m.Mes
                         }).FirstOrDefault();
            return meses;
        }

        //Obtener Ejercicios
        [HttpGet]
        [Route("api/Catalogo/GetExercicies")]
        public List<EjerciciosModel> GetExercicies()
        {
            listejercicios = (from e in Db.Ejercicios                     
                     select new EjerciciosModel()
                     {   
                         Id_ejercicio=e.Id_ejercicio,
                         Clave_ejercicio=e.Clave_ejercicio,
                         Ejercicio =e.Ejercicio,
                         Descripcion =e.Descripcion,
                         Posicion=e.Posicion,
                         ubicacion_imagen=e.ubicacion_imagen
                     }).ToList();
            return listejercicios;
        }

        [HttpGet]
        public EjerciciosModel GetExercicies(int IdExercicie)
        {
            ejercicios = (from e in Db.Ejercicios
                          where e.Id_ejercicio==IdExercicie
                              select new EjerciciosModel()
                              {
                                  Id_ejercicio = e.Id_ejercicio,
                                  Clave_ejercicio = e.Clave_ejercicio,
                                  Ejercicio = e.Ejercicio,
                                  Descripcion = e.Descripcion,
                                  Posicion = e.Posicion,
                                  ubicacion_imagen = e.ubicacion_imagen
                              }).FirstOrDefault();
            return ejercicios;
        }

        //obetener estatus
        [HttpGet]
        [Route("api/Catalogo/GetStatus")]
        public List<EstatusModel> GetStatus()
        {
            listestatus = (from e in Db.Estatus                          
                          select new EstatusModel()
                          {
                              Id_estatus=e.Id_estatus,
                              Descripcion=e.Descripcion
                          }).ToList();
            return listestatus;
        }
        [HttpGet]
        public EstatusModel GetStatus(int IdStatus)
        {
            estatus = (from e in Db.Estatus
                           where e.Id_estatus==IdStatus
                           select new EstatusModel()
                           {
                               Id_estatus = e.Id_estatus,
                               Descripcion = e.Descripcion
                           }).FirstOrDefault();
            return estatus;
        }

        //Obtener Rutinas
        [HttpGet]
        [Route("api/Catalogo/GetRutines")]
        public List<RutinasModel> GetRutines()
        {
            listrutinas = (from r in Db.Rutinas                       
                       select new RutinasModel()
                       {
                           Id_rutina = r.Id_rutina,
                           Clave_rutina =r.Clave_rutina,
                           Descripcion=r.Descripcion
                       }).ToList();
            return listrutinas;
        }
        [HttpGet]
        public RutinasModel GetRutines(int IdRutine)
        {
           rutinas = (from r in Db.Rutinas
                           where r.Id_rutina==IdRutine
                           select new RutinasModel()
                           {
                               Id_rutina = r.Id_rutina,
                               Clave_rutina = r.Clave_rutina,
                               Descripcion = r.Descripcion
                           }).FirstOrDefault();
            return rutinas;
        }

        //Obtener tIPO Rutinas
        [HttpGet]
        [Route("api/Catalogo/GetTypeRutines")]
        public List<TiporutinaModel> GetTypeRutines()
        {
            listtiporutinas = (from r in Db.Tiporutina
                           select new TiporutinaModel()
                           {
                               Id_tiporutina = r.Id_tiporutina,
                               Tipo = r.Tipo,
                               Descripcion = r.Descripcion
                           }).ToList();
            return listtiporutinas;
        }
        [HttpGet]
        public TiporutinaModel GetTypeRutines(int IdTypeRutine)
        {
            tiporutinas = (from r in Db.Tiporutina
                               where r.Id_tiporutina== IdTypeRutine
                               select new TiporutinaModel()
                               {
                                   Id_tiporutina = r.Id_tiporutina,
                                   Tipo = r.Tipo,
                                   Descripcion = r.Descripcion
                               }).FirstOrDefault();
            return tiporutinas;
        }

        //Obtener Tipoentrenamiento
        [HttpGet]
        [Route("api/Catalogo/GetTypeTraining")]
        public List<TipoentrenamientoModel> GetTypeTraining()
        {
            listtipoentrena = (from t in Db.TipoEntrenamiento                       
                       select new TipoentrenamientoModel()
                       {
                           Id_TipoEntrenamiento=t.Id_TipoEntrenamiento,
                           Clave_Entrenamiento =t.Clave_Entrenamiento,
                           Tipo_entrenamiento=t.Tipo_entrenamiento
                       }).ToList();
            return listtipoentrena;
        }
        [HttpGet]
        public TipoentrenamientoModel GetTypeTraining(int IdTypeRutine)
        {
            tipoentrena = (from t in Db.TipoEntrenamiento
                               where t.Id_TipoEntrenamiento==IdTypeRutine
                               select new TipoentrenamientoModel()
                               {
                                   Id_TipoEntrenamiento = t.Id_TipoEntrenamiento,
                                   Clave_Entrenamiento = t.Clave_Entrenamiento,
                                   Tipo_entrenamiento = t.Tipo_entrenamiento
                               }).FirstOrDefault();
            return tipoentrena;
        }
        
        
    }
}