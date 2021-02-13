using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SuperfitApi.Models
{
    public class CuestionarioModel
    {
        public int Id_cuestionario { get; set; }
        public  ClientesModel  Cliente { get; set; }
        public string Clave_cuestionario { get; set; }
        public  bool  Padece_enfermedad { get; set; }
        public string Medicamento_prescrito_medico { get; set; }
        public  bool  lesiones { get; set; }
        public string Alguna_recomendacion_lesiones { get; set; }
        public  bool  Fuma { get; set; }
        public  int  Veces_semana_fuma { get; set; }
        public  bool  Alcohol { get; set; }
        public  int  Veces_semana_alcohol { get; set; }
        public  bool  Actividad_fisica { get; set; }
        public string Tipo_ejercicios { get; set; }
        public string Tiempo_dedicado { get; set; }
        public string Horario_entreno { get; set; }
        public string MetasObjetivos { get; set; }
        public string Compromisos { get; set; }
        public string Comentarios { get; set; }
        public DateTime Fecha_registro { get; set; }
    }
}