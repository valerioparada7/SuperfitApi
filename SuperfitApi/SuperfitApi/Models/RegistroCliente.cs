using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SuperfitApi.Models
{
    public class RegistroCliente
    {
        public ClientesModel Cliente { get; set; }
        public MensualidadModel Mensualidad { get; set; }
        public CuestionarioModel Cuestionario { get; set; }
        public AntropometriaModel Medidas { get; set; }
        public Imagenes Imagenes { get; set; }
    }
}