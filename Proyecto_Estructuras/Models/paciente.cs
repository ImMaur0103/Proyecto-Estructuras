using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_Estructuras.Models
{
    public class paciente
    {
        string Nombre { get; set; }
        string Apellido { get; set; }
        int DPI_CUI { get; set; }
        string Departamento { get; set; }
        string Municipio_residencia { get; set; }
        int Edad { get; set; }
        bool Cuerpo_medico { get; set; }
        bool Vacunado { get; set; }
    }
}
