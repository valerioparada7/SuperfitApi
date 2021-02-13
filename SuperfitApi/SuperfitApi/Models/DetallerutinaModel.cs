using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SuperfitApi.Models
{
    public class DetallerutinaModel
    {
        public int Id_detallerutina { get; set; }
        public DiasModel Dias { get; set; }
        public EjerciciosModel Ejercicios { get; set; }
        public MensualidadModel Mensualidad { get; set; }
        public RutinasModel Rutinas { get; set; }
        public int Repeticiones { get; set; }
        public int Series { get; set; }
        public int Tipo_set { get; set; }  
        //TIPO SET PARA MANDAR LETRAS
        public string TipoSet { get; set; }
    }
}