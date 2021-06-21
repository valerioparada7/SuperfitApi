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
        public string Ubicacion_imagen { get; set; }
        public string Posicion { get; set; }
        public HttpPostedFileBase Imagen { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Tipo { get; set; }

        public TiporutinaModel Rutinas { get; set; }
    }
}