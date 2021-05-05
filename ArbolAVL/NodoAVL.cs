using System;
using System.Collections.Generic;
using System.Text;

namespace ArbolAVL
{
    public class NodoAVL<T>
    {
        // Valor del nodo, el cual es el NoLinea y Nombre del fármaco
        public PacienteArbol valor { get; set; }

        //Posiciones del árbol binario
        public NodoAVL<T> derecha { get; set; }
        public NodoAVL<T> izquierda { get; set; }

        //Factor de equilibrio, propio del árbol AVL
        public int Fe;

        // constructor de la clase Nodo
        public NodoAVL()
        {
            Fe = 0;
            derecha = null;
            izquierda = null;
        }

        // public Nodo<T> raiz; 

        ~NodoAVL() { }
    }
}
