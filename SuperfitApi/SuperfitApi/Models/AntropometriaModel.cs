using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SuperfitApi.Models
{
    public class AntropometriaModel
    {
        public int Id { get; set; }
        public MensualidadModel Mensualidad { get; set; }
        public double Peso { get; set; }
        public int Altura { get; set; }
        public double IMC { get; set; }
        public double Brazoderechorelajado { get; set; }
        public double Brazoderechofuerza { get; set; }
        public double Brazoizquierdorelajado { get; set; }
        public double Brazoizquierdofuerza { get; set; }
        public double Cintura { get; set; }
        public double Cadera { get; set; }
        public double Piernaizquierda { get; set; }
        public double Piernaderecho { get; set; }
        public double Pantorrilladerecha { get; set; }
        public double Pantorrillaizquierda { get; set; }
        public string Fotofrontal { get; set; }
        public string Fotoperfil { get; set; }
        public string Fotoposterior { get; set; }
        public DateTime Fecha_registro { get; set; }
        public string Medidas_actualizadas { get; set; }
    }
}