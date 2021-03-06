using System;

namespace Arbol
{
    public class Nodo<T>
    {
        public Prioridad valor { get; set; }
        public int altura;
        public int indice;

        //Posiciones del árbol binario
        public Nodo<T> derecha { get; set; }
        public Nodo<T> izquierda { get; set; }


        // constructor de la clase Nodo
        public Nodo()
        {
           // indice = 0;
            altura = 0;
            derecha = null;
            izquierda = null;
        }
        
       // public Nodo<T> raiz; 

        ~Nodo() { }
    }
}
