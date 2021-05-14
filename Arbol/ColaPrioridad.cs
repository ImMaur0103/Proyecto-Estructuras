using System;
using System.Collections.Generic;
using System.Text;
using ListaDobleEnlace;
using Arbol;

namespace Arbol
{
    public class ColaPrioridad
    {
        ListaDoble<Nodo<Prioridad>> Cola;

        public ColaPrioridad()
        {
            Cola = new ListaDoble<Nodo<Prioridad>>();
        }
        ~ColaPrioridad() { }

        public void Insertar(Prioridad paciente)
        {
            Nodo<Prioridad> nodo = new Nodo<Prioridad>();
            nodo.valor = paciente;
            if(paciente.prioridad < 4 && Cola.contador > 0)
            {
                int NodosEnArbol = Cola.contador;
                for (int i = 0; i < NodosEnArbol; i++)
                {
                    if(Cola.ObtenerValor(i).valor.prioridad > paciente.prioridad)
                    {
                        Cola.InsertarEnPosicion(nodo, (i));
                        Cola.ObtenerValor(i).indice = i;
                        break;
                    }
                    Cola.ObtenerValor(i).indice = i;
                }
                if(NodosEnArbol == Cola.contador)
                {
                    Cola.InsertarFinal(nodo);
                }
                for (int i = 0; i < Cola.contador; i++)
                {
                    Cola.ObtenerValor(i).indice = i;
                }
            }
            else
            {
                Cola.InsertarFinal(nodo);
                for (int i = 0; i < Cola.contador; i++)
                {
                    Cola.ObtenerValor(i).indice = i;
                }
            }
        }

        public void InsertarConArbol(Arbol<Prioridad> arbol)
        {
            for (int i = 0; i < arbol.contador; i++)
            {
                Insertar(arbol.BuscarConIndice(i+1).valor);
            }
        }

        public int Buscar(Prioridad paciente)
        {
            for (int i = 0; i < Cola.contador; i++)
            {
                if(paciente == Cola.ObtenerValor(i).valor)
                {
                    return i;
                }
            }
            return -1;
        }
        public ListaDoble<Nodo<Prioridad>> ObtenerCola()
        {
            return Cola;
        }
    }
}
