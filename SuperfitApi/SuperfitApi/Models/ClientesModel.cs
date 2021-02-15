using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SuperfitApi.Models
{
    public class ClientesModel
    {
        public int Id_Cliente { get; set; }
        public string Clave_Cliente { get; set; }
        public string Nombres { get; set; }
        public string Apellido_Paterno { get; set; }
        public string Apellido_Materno { get; set; }
        public int Edad { get; set; }
        public decimal Telefono { get; set; }
        public string Correo_electronico { get; set; }
        public string Apodo { get; set; }
        public string Contraseña { get; set; }
        public bool Validar { get; set; }        
    }
}