using System;
using System.Collections.Generic;
using System.Text;

namespace ListaDobleEnlace
{
    public class Nodo<T> 
    {
        private T valor { get; set; }

        private Nodo<T> siguiente;
        private Nodo<T> anterior;

        public T Valor
        {

            get { return valor; }
            set { valor = value; }
        }

        public Nodo<T> Siguiente
        {
            get { return siguiente; }
            set { siguiente = value; }
        }

        public Nodo<T> Anterior
        {
            get { return anterior; }
            set { anterior = value; }
        }
    }
}
