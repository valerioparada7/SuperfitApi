//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SuperfitApi.Models.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class Pagos_mensualidades
    {
        public int Id_pago { get; set; }
        public int Id_mensualidad { get; set; }
        public decimal Monto { get; set; }
        public Nullable<System.DateTime> Fecha_pago { get; set; }
        public string Descripcion { get; set; }
        public string Ubicacion_imagen_pago { get; set; }
    
        public virtual Mensualidades Mensualidades { get; set; }
    }
}
