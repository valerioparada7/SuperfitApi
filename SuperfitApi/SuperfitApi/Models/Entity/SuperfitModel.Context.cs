﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class SuperfitEntities : DbContext
    {
        public SuperfitEntities()
            : base("name=SuperfitEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Asesoria_antropometria> Asesoria_antropometria { get; set; }
        public virtual DbSet<Clientes> Clientes { get; set; }
        public virtual DbSet<Cuestionario> Cuestionario { get; set; }
        public virtual DbSet<Detalle_rutina> Detalle_rutina { get; set; }
        public virtual DbSet<Dias> Dias { get; set; }
        public virtual DbSet<Ejercicios> Ejercicios { get; set; }
        public virtual DbSet<Estatus> Estatus { get; set; }
        public virtual DbSet<Mensualidad> Mensualidad { get; set; }
        public virtual DbSet<Meses> Meses { get; set; }
        public virtual DbSet<Pagos_mensualidades> Pagos_mensualidades { get; set; }
        public virtual DbSet<Rutinas> Rutinas { get; set; }
        public virtual DbSet<Tipo_entrenamiento> Tipo_entrenamiento { get; set; }
        public virtual DbSet<Tipo_rutina> Tipo_rutina { get; set; }
        public virtual DbSet<Usuarios> Usuarios { get; set; }
    }
}