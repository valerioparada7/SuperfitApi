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
        }
        #region Variables
        public SuperflyfitEntities Db;
        #endregion
        //Obtener dias
        [HttpGet]
        public List<Dias> GetDays()
        {
            var dias = Db.Dias.ToList();
            return dias;
        }
        [HttpGet]
        public Dias GetDays(int IdDay)
        {
            var dias = Db.Dias.Where(p=>p.Id_dia== IdDay).FirstOrDefault();
            return dias;
        }

        //Obetener meses
        [HttpGet]
        public List<Meses> GetMonths()
        {
            var meses = Db.Meses.ToList();
            return meses;
        }
        [HttpGet]
        public Meses GetMonths(int IdMonth)
        {
            var meses = Db.Meses.Where(p=>p.Id_mes==IdMonth).FirstOrDefault();
            return meses;
        }

        //Obtener Ejercicios
        [HttpGet]
        public List<Ejercicios> GetExercicies()
        {
            var Ejercicios = Db.Ejercicios.ToList();
            return Ejercicios;
        }

        [HttpGet]
        public Ejercicios GetExercicies(int IdExercicie)
        {
            var Ejercicios = Db.Ejercicios.Where(p=>p.Id_ejercicio==IdExercicie).FirstOrDefault();
            return Ejercicios;
        }

        //obetener estatus
        [HttpGet]
        public List<Estatus> GetStatus()
        {
            var Estatus = Db.Estatus.ToList();
            return Estatus;
        }
        [HttpGet]
        public Estatus GetStatus(int IdStatus)
        {
            var Estatus = Db.Estatus.Where(p=>p.Id_estatus==IdStatus).FirstOrDefault();
            return Estatus;
        }

        //Obtener Rutinas
        [HttpGet]
        public List<Rutinas> GetRutines()
        {
            var Rutinas = Db.Rutinas.ToList();
            return Rutinas;
        }
        [HttpGet]
        public Rutinas GetRutines(int IdRutine)
        {
            var Rutinas = Db.Rutinas.Where(p=>p.Id_rutina==IdRutine).FirstOrDefault();
            return Rutinas;
        }

        //Obtener Tipoentrenamiento
        [HttpGet]
        public List<TipoEntrenamiento> GetTypeTraining()
        {
              var TipoEntrenamiento = Db.TipoEntrenamiento.ToList();
            return TipoEntrenamiento;
        }
        [HttpGet]
        public TipoEntrenamiento GetTypeTraining(int IdTypeRutine)
        {
            var TipoEntrenamiento = Db.TipoEntrenamiento.Where(p=>p.Id_TipoEntrenamiento==IdTypeRutine).FirstOrDefault();
            return TipoEntrenamiento;
        }
        
        
    }
}