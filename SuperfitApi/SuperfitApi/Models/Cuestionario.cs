//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SuperfitApi.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Cuestionario
    {
        public int Id_cuestionario { get; set; }
        public Nullable<int> Id_Cliente { get; set; }
        public string Clave_cuestionario { get; set; }
        public Nullable<bool> Padece_enfermedad { get; set; }
        public string Medicamento_prescrito_medico { get; set; }
        public Nullable<bool> lesiones { get; set; }
        public string Alguna_recomendacion_lesiones { get; set; }
        public Nullable<bool> Fuma { get; set; }
        public Nullable<int> Veces_semana_fuma { get; set; }
        public Nullable<bool> Alcohol { get; set; }
        public Nullable<int> Veces_semana_alcohol { get; set; }
        public Nullable<bool> Actividad_fisica { get; set; }
        public string Tipo_ejercicios { get; set; }
        public string Tiempo_dedicado { get; set; }
        public string Horario_entreno { get; set; }
        public string MetasObjetivos { get; set; }
        public string Compromisos { get; set; }
        public string Comentarios { get; set; }
        public Nullable<System.DateTime> Fecha_registro { get; set; }
    
        public virtual Clientes Clientes { get; set; }
    }
}
