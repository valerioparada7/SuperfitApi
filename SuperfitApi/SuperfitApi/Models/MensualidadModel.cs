using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SuperfitApi.Models
{
    public class MensualidadModel
    {
        public int Id_mensualidad { get; set; }
        public ClientesModel Cliente { get; set; }
        public TiporutinaModel Tiporutina { get; set; }
        public MesesModel Mes { get; set; }
        public EstatusModel Estatus { get; set; }
        public TipoentrenamientoModel TipoEntrenamiento { get; set; }
        public DateTime Fecha_inicio { get; set; }
        public DateTime Fecha_fin { get; set; }
        public string SinfechaAsignadaI { get; set; }
        public string SinfechaAsignadaF { get; set; }
    }
}