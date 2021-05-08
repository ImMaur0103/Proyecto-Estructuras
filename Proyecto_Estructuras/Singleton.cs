using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArbolAVL;
using Proyecto_Estructuras.Models;

namespace Proyecto_Estructuras
{
    public class Singleton
    {
        private static Singleton instance = null;
        public Arbol<PacienteArbol> ArbolPacientesNombres = new Arbol<PacienteArbol>();
        public Arbol<PacienteArbol> ArbolPacientesApellidos = new Arbol<PacienteArbol>();
        public Arbol<PacienteArbol> ArbolPacientesDPI = new Arbol<PacienteArbol>();

        protected Singleton()
        {

        }

        public static Singleton Instance
        {
            get
            {
                if (instance == null)
                    instance = new Singleton();

                return instance;
            }
        }

    }
}
