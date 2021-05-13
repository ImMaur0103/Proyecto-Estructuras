using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_Estructuras.Models
{
    public class Citas
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public long DPI_CUI { get; set; }
        public int Edad { get; set; }
        public int Prioridad { get; set; }
        public string Fecha { get; set; }
        public string Hora { get; set; }
        public string MarcaVacuna { get; set; }
        public int Dosis { get; set; }
    }
}
