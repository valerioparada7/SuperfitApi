using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SuperfitApi.Models;

namespace SuperfitApi.Models
{
    public class PagosmensualModel
    {
        public int Id_pago { get; set; }
        public MensualidadModel  Id_mensualidad { get; set; }
        public double Monto { get; set; }
        public DateTime Fecha_pago   { get; set; }
        public string Descripcion { get; set; }
        public string Ubicacion_imagen_pago { get; set; }
        public HttpPostedFileBase ImagenPago { get; set; }
    }
}