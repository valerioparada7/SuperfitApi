using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SuperfitApi.Models
{
    public class EjerciciosModel
    {
        public int Id_ejercicio { get; set; }
        public string Clave_ejercicio { get; set; }
        public string Ejercicio { get; set; }
        public string Descripcion { get; set; }
        public string ubicacion_imagen { get; set; }
        public string Posicion { get; set; }
    }
}