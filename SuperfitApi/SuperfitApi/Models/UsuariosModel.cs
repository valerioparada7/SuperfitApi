using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SuperfitApi.Models
{
    public class UsuariosModel
    {
        public int Id_Usuario { get; set; }
        public string Clave_Usuario { get; set; }
        public string Contraseña { get; set; }
        public string Nombres { get; set; }
        public string Apellido_Paterno { get; set; }
        public string Apellido_Materno { get; set; }
        public bool Estado { get; set; }
    }
}