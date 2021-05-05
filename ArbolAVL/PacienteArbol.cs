using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArbolAVL
{
    public class PacienteArbol
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public int DPI_CUI { get; set; }
        public string Departamento { get; set; }
        public string Municipio_residencia { get; set; }
        public int Edad { get; set; }
        public bool Cuerpo_medico { get; set; }
    }
}
