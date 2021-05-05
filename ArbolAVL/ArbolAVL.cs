using System;
using ArbolAVL;

namespace ArbolAVL
{
    public class ArbolAVL<T>
    {
        public NodoAVL<T> Raiz;
        public int Contador;

        public ArbolAVL()
        {
            Raiz = null;
            Contador = 0;
        }

        ~ArbolAVL() { }

        //Factor de equilibrio 
        public int CalcFe(NodoAVL<T> nodo)
        {
            if (nodo == null)
            {
                return -1;
            }
            else
            {
                return nodo.Fe;
            }
        }

        public NodoAVL<T> RotarIzquierda(NodoAVL<T> nodo)
        {
            NodoAVL<T> aux = nodo.izquierda;
            nodo.izquierda = aux.derecha;
            aux.derecha = nodo;

            nodo.Fe = Math.Max(CalcFe(nodo.izquierda), CalcFe(nodo.derecha)) + 1;
            aux.Fe = Math.Max(CalcFe(aux.izquierda), nodo.Fe) + 1;

            return aux;
        }

        public NodoAVL<T> RotarDerecha(NodoAVL<T> nodo)
        {
            NodoAVL<T> aux = nodo.derecha;
            nodo.derecha = aux.izquierda;
            aux.izquierda = nodo;

            nodo.Fe = Math.Max(CalcFe(nodo.izquierda), CalcFe(nodo.derecha)) + 1;
            //aux.Fe = Math.Max(CalcFe(aux.izquierda), CalcFe(aux.derecha)) + 1;
            aux.Fe = Math.Max(CalcFe(aux.derecha), nodo.Fe) + 1;
            //CalcFe(aux.derecha)
            return aux;
        }

        public NodoAVL<T> RDobleIzquierda(NodoAVL<T> nodo)
        {
            NodoAVL<T> aux;
            nodo.izquierda = RotarDerecha(nodo.izquierda);
            aux = RotarIzquierda(nodo);

            return aux;
        }

        public NodoAVL<T> RDobleDerecha(NodoAVL<T> nodo)
        {
            NodoAVL<T> aux;
            nodo.derecha = RotarIzquierda(nodo.derecha);
            aux = RotarDerecha(nodo);

            return aux;
        }

        public NodoAVL<T> Balancear(NodoAVL<T> actual)
        {
            NodoAVL<T> raiz = actual;
            if (((CalcFe(actual.derecha)) - CalcFe(actual.izquierda)) == 2)
            {
                if (actual.izquierda != null)
                {
                    if (raiz.izquierda.izquierda == null && raiz.izquierda.derecha == null)
                    {
                        raiz = RDobleDerecha(actual);
                    }
                    else
                    {
                        raiz = RotarIzquierda(actual);
                    }
                }
                else
                {
                    if (raiz.derecha.izquierda == null && raiz.derecha.derecha == null)
                    {
                        raiz = RDobleDerecha(actual);
                    }
                    else
                    {
                        raiz = RotarIzquierda(actual);
                    }
                }
                Balancear(raiz);
            }
            else if ((CalcFe(actual.derecha) - CalcFe(actual.izquierda)) == -2)
            {
                if (actual.izquierda != null)
                {
                    if (raiz.izquierda.izquierda == null && raiz.izquierda.derecha == null)
                    {
                        raiz = RDobleIzquierda(actual);
                    }
                    else
                    {
                        raiz = RotarDerecha(actual);
                    }
                }
                else
                {
                    if (raiz.derecha.izquierda == null && raiz.derecha.derecha == null)
                    {
                        raiz = RDobleIzquierda(actual);
                    }
                    else
                    {
                        raiz = RotarDerecha(actual);
                    }
                }
                Balancear(raiz);
            }

            return raiz;
        }
    }
}
