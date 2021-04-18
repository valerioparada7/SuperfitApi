using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SuperfitApi.Models
{
    public class ClientesModel
    {
        public int Id_cliente { get; set; }
        public string Clave_cliente { get; set; }
        public string Nombres { get; set; }
        public string Apellido_paterno { get; set; }
        public string Apellido_materno { get; set; }
        public int Edad { get; set; }
        public decimal Telefono { get; set; }
        public string Correo_electronico { get; set; }
        public bool Estado { get; set; }
        public string Apodo { get; set; }
        public string Contraseña { get; set; }
        public string Foto_perfil { get; set; }
        public string Sexo { get; set; }
        public bool Validar { get; set; }        
        public HttpPostedFileBase Imagen { get; set; }
    }
}