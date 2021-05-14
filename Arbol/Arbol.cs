using System;
using System.Collections.Generic;
using System.Text;
using ListaDobleEnlace;


namespace Arbol
{
    public class Arbol<T> : Nodo<T>
    {
        public Nodo<T> raiz;
        public int contador;
        private int UltimoIndice;

        //constructor 
        public Arbol()
        {
            raiz = null;
            contador = 0;
        }

        ~Arbol() { }

        public void Insertar(Prioridad valor, int indice)
        {
            Nodo<T> NuevoNodo = new Nodo<T>();
            NuevoNodo.valor = valor;
            NuevoNodo.indice = indice;
            NuevoNodo.izquierda = null;
            NuevoNodo.derecha = null;

            if (raiz == null)
            {
                raiz = NuevoNodo;
            }
            else
            {
                raiz = InsertarHeap(raiz, NuevoNodo);
            }
            contador++;
        }

        private Nodo<T> InsertarNodo(Nodo<T> actual, Nodo<T> nuevo)
        {
            if (actual == null)
            {
                return nuevo;
            }
            if (actual.valor.Cui.CompareTo(nuevo.valor.Cui) < 0)
            {
                actual.derecha = InsertarNodo(actual.derecha, nuevo);
                if (actual.derecha.valor.prioridad < actual.valor.prioridad)
                {
                    actual = RotarIzquierda(actual);
                }
            }
            else
            {
                actual.izquierda = InsertarNodo(actual.izquierda, nuevo);
                if (actual.izquierda.valor.prioridad < actual.valor.prioridad)
                {
                    actual = RotarDerecha(actual);
                }
            }
            return actual;
        }

        public int Buscar(string nombre)
        {
            Nodo<T> recorrer = raiz;
            bool encontrar = false;
            while (recorrer != null && encontrar == false)
            {
                string valor = recorrer.valor.Cui;
                valor = valor.ToLower();
                if (nombre == valor)
                {
                    encontrar = true;
                }
                else
                {
                    if (nombre.CompareTo(recorrer.valor.Cui) > 0)
                    {
                        recorrer = recorrer.derecha;
                        encontrar = false;
                    }
                    else
                    {
                        recorrer = recorrer.izquierda;
                        encontrar = false;
                    }
                }
            }
            if (recorrer == null)
            {
                return 0;
            }
            return recorrer.valor.prioridad;
        }
        public Nodo<T> BuscarConIndice(int Indice)
        {
            return Buscando(Indice, raiz);
        }
        private Nodo<T> Buscando(int Indice, Nodo<T> Padre)
        {
            Nodo<T> Retornar = new Nodo<T>();
            int buscando = Indice;
            while (buscando > Padre.izquierda.indice && buscando > Padre.derecha.indice)
            {
                buscando /= 2;
            }
            if(Indice != buscando)
            {
                if (Padre.izquierda.indice == buscando)
                    Retornar = Buscando(Indice, Padre.izquierda);
                else
                    Retornar = Buscando(Indice, Padre.derecha);
            }
            else
            {
                if (Indice == Padre.indice)
                    Retornar = Padre;
                else if (Indice == Padre.izquierda.indice)
                    Retornar = Padre.izquierda;
                else
                    Retornar = Padre.derecha;
            }
            return Retornar;
        }

        public Nodo<T> Eliminar()
        {
            if (raiz == null)
                return raiz;
            else
            {
                //Si el nodo raiz del árbol tiene dos hijos
                if (raiz.izquierda != null)
                {
                    if (raiz.derecha != null)
                    {
                        //Se valida la prioridad
                        if (raiz.izquierda.valor.prioridad < raiz.derecha.valor.prioridad)
                        {
                            // Método rotar a la derecha
                            raiz = RotarDerecha(raiz);
                            raiz.derecha = Eliminar();
                        }
                        else
                        {
                            raiz = RotarIzquierda(raiz);
                            raiz.izquierda = Eliminar();
                        }
                    }
                    else
                    {
                        Nodo<T> auxiliar = raiz;
                        raiz.valor = null;
                        contador--;
                        return auxiliar;
                    }
                }
                else
                {
                    if (raiz.derecha != null)
                    {
                        Nodo<T> auxiliar = raiz;
                        raiz.valor = null;
                        contador--;
                        return auxiliar;
                    }
                    else
                    {
                        // El nodo es hoja
                        Nodo<T> auxiliar = raiz;
                        raiz = null;
                        contador--;
                        return auxiliar;
                    }
                }
            }
            return raiz;
        }
        private Nodo<T> RotarDerecha(Nodo<T> nodo)
        {
            Nodo<T> auxilar = nodo.izquierda;
            nodo.izquierda = auxilar.derecha;
            auxilar.derecha = nodo;
            return auxilar;
        }

        private Nodo<T> RotarIzquierda(Nodo<T> nodo)
        {
            Nodo<T> auxiliar = nodo.derecha;
            nodo.derecha = auxiliar.izquierda;
            auxiliar.izquierda = nodo;
            return auxiliar;
        }

        public void Delete()
        {
            raiz = null;
            contador = 0;
        }

        private Nodo<T> InsertarHeap(Nodo<T> Padre, Nodo<T> Nuevo)
        {
            int buscando = Nuevo.indice;
            if (Padre.derecha != null || Padre.indice == Nuevo.indice)
            {
                if(buscando == Padre.indice)
                {
                    Nodo<T> aux = new Nodo<T>();
                    aux.valor = Padre.valor;
                    aux.indice = Padre.indice + 1;
                    Padre.valor = Nuevo.valor;
                    InsertarHeap(raiz, aux);
                    return Padre;
                }
                while (buscando > Padre.izquierda.indice && buscando > Padre.derecha.indice)
                {
                    buscando /= 2;
                }
                if (buscando == Padre.izquierda.indice)
                {
                    Padre.izquierda = InsertarHeap(Padre.izquierda, Nuevo);
                }
                else if (buscando == Padre.derecha.indice)
                {
                    Padre.derecha = InsertarHeap(Padre.derecha, Nuevo);
                }
            }
            else
            {
                if (Padre.izquierda != null)
                {
                    Padre.derecha = Nuevo;
                    return Padre;
                }
                else
                {
                    Padre.izquierda = Nuevo;
                    return Padre;
                }
            }
            return Padre;
        }

        private int CalAlturas(Nodo<T> obtener)
        {
            if (obtener == null)
            {
                return -1;
            }
            else
            {
                return obtener.altura;
            }
        }
        // Insertar nodos en el árbol 
        /*public void Insertar(Prioridad valor)
        {
            Nodo<T> NuevoNodo = new Nodo<T>();
            NuevoNodo.valor = valor;
            NuevoNodo.izquierda = null;
            NuevoNodo.derecha = null;

            if (raiz == null)
            {
                raiz = NuevoNodo;
                raiz.indice = 1;
            }
            else
            {
                raiz = InsertarNodo(raiz, NuevoNodo);
            }
            contador++;
        }

        public T Mayor<T>(T valor1, T valor2) where T : IComparable
        {
            if (valor1.CompareTo(valor2) > 0) return valor1;
            return valor2;
        }

        private Nodo<T> InsertarHeap(Nodo<T> Padre, Nodo<T> Nuevo)
        {
            int buscando = Nuevo.indice;
            if (Padre.derecha != null)
            {
                while (buscando > Padre.izquierda.indice && buscando > Padre.derecha.indice)
                {
                    buscando /= 2;
                }
                if (buscando == Padre.izquierda.indice)
                {
                    Padre.izquierda = InsertarHeap(Padre.izquierda, Nuevo);
                }
                else if (buscando == Padre.derecha.indice)
                {
                    Padre.derecha = InsertarHeap(Padre.derecha, Nuevo);
                }
            }
            else
            {
                if(Padre.izquierda != null)
                {
                    Padre.derecha = Nuevo;
                    return Padre;
                }
                else
                {
                    Padre.izquierda = Nuevo;
                    return Padre;
                }
            }
            return Padre;
        }



        private Nodo<T> InsertarNodo(Nodo<T> actual, Nodo<T> nuevo)
        {
            if (nuevo.valor.Nombre.CompareTo(actual.valor.Nombre) > 0)//al ser mayor  se tiene que mover el arbol
            {
                actual = RestructurarArbol(actual, nuevo);
                actual = ArregloIndices(actual);
                return actual;
            }
            else if(nuevo.valor.Nombre.CompareTo(actual.valor.Nombre) < 0)//si es menor se tiene que verificar si se va a la derecha o a la izquierda
            {
                if (actual.izquierda == null)
                {
                    actual.izquierda = nuevo;
                    return actual;
                }
                else if(actual.izquierda != null && actual.derecha == null)
                {
                    actual.derecha = nuevo;
                    actual.altura++;
                    return actual;
                }
                else if(actual.izquierda.altura > actual.derecha.altura)
                {
                    actual.derecha = InsertarNodo(actual.derecha, nuevo);
                    if (actual.altura < CalAlturas(actual.derecha) + 1)
                        actual.altura++;
                    return actual;
                }
                else
                {
                    actual.izquierda = InsertarNodo(actual.izquierda, nuevo);
                    if(actual.altura < CalAlturas(actual.derecha) + 1)
                        actual.altura++;
                    return actual;
                }
            }
            else
            {
                return null;
            }
        }

        private Nodo<T> RestructurarArbol(Nodo<T> actual, Nodo<T> nuevo)
        {
            if (actual.izquierda == null)
            {
                nuevo.izquierda = actual;
                return nuevo;
            }
            else if (actual.derecha == null)
            {
                nuevo.izquierda = actual.izquierda;
                actual.izquierda = null;
                nuevo.derecha = actual;
                return nuevo;
            }
            else if (CalAlturas(actual.izquierda) > CalAlturas(actual.derecha))
            {
                nuevo.izquierda = actual.izquierda;
                actual.izquierda = actual.derecha;
                actual.derecha = null;
                nuevo.derecha = actual;
                return nuevo;
            }
            else
            {
                if (CalAlturas(actual.izquierda.izquierda) + 1 > CalAlturas(actual.derecha))
                {
                    nuevo.derecha = actual.derecha;
                    actual.derecha = actual.izquierda.izquierda;
                    actual.izquierda.izquierda = null;
                    nuevo.izquierda = actual;
                    return nuevo;
                }
                else if (actual.izquierda.derecha != null)
                {
                    nuevo.derecha = actual.derecha;
                    actual.derecha = null;
                    nuevo.izquierda = actual;
                    return nuevo;
                }
                else
                {
                    nuevo.derecha = actual.derecha;
                    actual.derecha = null;
                    nuevo.izquierda = actual;
                    return nuevo;
                }
            }
        }

        private Nodo<T> ArregloIndices(Nodo<T> subArbol)
        {
            Nodo<T> Auxiliar = subArbol;

            if(Auxiliar.izquierda != null)
            {
                Auxiliar.altura = 0;
                Auxiliar.izquierda = ArregloIndices(Auxiliar.izquierda);
            }
            
            if (Auxiliar.derecha != null)
            {
                Auxiliar.derecha = ArregloIndices(Auxiliar.derecha);
                if(Auxiliar.altura < Auxiliar.derecha.altura + 1)
                    Auxiliar.altura = Auxiliar.derecha.altura + 1;
            }
            
            if(Auxiliar.derecha == null && Auxiliar.izquierda == null)
            {
                Auxiliar.altura = 0;
                return Auxiliar;
            }
            return Auxiliar;
        }

        private int CalAlturas(Nodo<T> obtener)
        {
            if(obtener == null)
            {
                return -1;
            }
            else
            {
                return obtener.altura;
            }
        }

        public int Buscar(string nombre)// Acomodar buscar
        {
            Nodo<T> recorrer = raiz;
            bool encontrar = false;
            while(recorrer != null && encontrar == false)
            {
                string valor = recorrer.valor.Nombre;
                valor = valor.ToLower();
                if(nombre == valor)
                {
                    encontrar = true; 
                }
                else
                {
                    if(nombre.CompareTo(recorrer.valor.Nombre) > 0)
                    {
                        recorrer = recorrer.derecha;
                        encontrar = false;
                    }
                    else
                    {
                        recorrer = recorrer.izquierda;
                        encontrar = false; 
                    }
                }
            }
            if(recorrer == null)
            {
                return 0;
            }
            return recorrer.indice;
        }

        public void Delete()
        {
            raiz = null;
            contador = 0;
        }
    }*/
    }
}
