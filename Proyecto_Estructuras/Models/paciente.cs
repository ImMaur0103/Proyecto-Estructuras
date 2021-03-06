using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_Estructuras.Models
{
    public class paciente
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public long DPI_CUI { get; set; }
        public string Departamento { get; set; }
        public string Municipio_residencia { get; set; }
        public int Edad { get; set; }
        public bool Vacunado { get; set; }
        public int Prioridad { get; set; }  // Se debe calcular, depende del grupo al que pertenece 
        public string Grupo { get; set; } // Depende de las opciones del drop down list 
        public string Enfermedad { get; set; } // Funciona para el radiobutton 
    }
}
