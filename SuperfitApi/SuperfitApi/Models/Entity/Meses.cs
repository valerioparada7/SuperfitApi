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
    
    public partial class Meses
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Meses()
        {
            this.Mensualidades = new HashSet<Mensualidades>();
        }
    
        public int Id_mes { get; set; }
        public string Clave_mes { get; set; }
        public string Mes { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Mensualidades> Mensualidades { get; set; }
    }
}
