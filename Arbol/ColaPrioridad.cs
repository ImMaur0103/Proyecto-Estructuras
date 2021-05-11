using System;
using System.Collections.Generic;
using System.Text;
using ListaDobleEnlace;
using Arbol;

namespace Arbol
{
    class ColaPrioridad
    {
        private ListaDoble<Nodo<PacienteArbol>> Cola;

        ColaPrioridad()
        {
            Cola = new ListaDoble<Nodo<PacienteArbol>>();
        }
        ~ColaPrioridad() { }

        public void Insertar(PacienteArbol paciente)
        {
            Nodo<PacienteArbol> nodo = new Nodo<PacienteArbol>();
            nodo.valor = paciente;
            if(paciente.Prioridad < 3)
            {
                for (int i = 0; i < Cola.contador; i++)
                {
                    if(Cola.ObtenerValor(i).valor.Prioridad > paciente.Prioridad)
                    {
                        Cola.InsertarEnPosicion(nodo, (i));
                    }
                    Cola.ObtenerValor(i).indice = i;
                }
            }
            else
            {
                Cola.InsertarFinal(nodo);
            }
        }

        public ListaDoble<Nodo<PacienteArbol>> ObtenerCola()
        {
            return Cola;
        }
    }
}
