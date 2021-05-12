using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arbol;
using ArbolAVL;
using Proyecto_Estructuras.Models;
using ListaDobleEnlace;

namespace Proyecto_Estructuras
{
    public class Singleton
    {
        private static Singleton instance = null;
        public ArbolAVL.Arbol<ArbolAVL.PacienteArbol> ArbolPacientesNombres = new ArbolAVL.Arbol<ArbolAVL.PacienteArbol>();
        public ArbolAVL.Arbol<ArbolAVL.PacienteArbol> ArbolPacientesApellidos = new ArbolAVL.Arbol<ArbolAVL.PacienteArbol>();
        public ArbolAVL.Arbol<ArbolAVL.PacienteArbol> ArbolPacientesDPI = new ArbolAVL.Arbol<ArbolAVL.PacienteArbol>();
        public ListaDoble<long> ListCui = new ListaDoble<long>(); 

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
