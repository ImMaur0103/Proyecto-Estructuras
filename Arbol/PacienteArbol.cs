using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arbol
{
    public class PacienteArbol
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public long DPI_CUI { get; set; }
        public string Departamento { get; set; }
        public string Municipio_residencia { get; set; }
        public int Edad { get; set; }
        public bool Vacunado { get; set; }
        public int Prioridad { get; set; }
    }
}
