using System;
using ListaDobleEnlace;

namespace THash
{
    public class THash<T> : ListaDoble<T> 
    {
        ListaDoble<T>[] HashTable = new ListaDoble<T>[10];
        public ListaDoble<T> Lista0 = new ListaDoble<T>();
        public ListaDoble<T> Lista1 = new ListaDoble<T>();
        public ListaDoble<T> Lista2 = new ListaDoble<T>();
        public ListaDoble<T> Lista3 = new ListaDoble<T>();
        public ListaDoble<T> Lista4 = new ListaDoble<T>();
        public ListaDoble<T> Lista5 = new ListaDoble<T>();
        public ListaDoble<T> Lista6 = new ListaDoble<T>();
        public ListaDoble<T> Lista7 = new ListaDoble<T>();
        public ListaDoble<T> Lista8 = new ListaDoble<T>();
        public ListaDoble<T> Lista9 = new ListaDoble<T>();

        public THash()
        {
            HashTable[0] = Lista0;
            HashTable[1] = Lista1;
            HashTable[2] = Lista2;
            HashTable[3] = Lista3;
            HashTable[4] = Lista4;
            HashTable[5] = Lista5;
            HashTable[6] = Lista6;
            HashTable[7] = Lista7;
            HashTable[8] = Lista8;
            HashTable[9] = Lista9;
        }

        public int Llave(string Titulo)
        {
            char[] array = new char[Titulo.Length];
            char letra;
            int posicion;
            int codigo = 0;

            for (int i = 0; i < Titulo.Length; i++)
            {
                array[i] = Titulo[i];
                letra = array[i];
                posicion = Convert.ToInt32(letra) * (i + 1);
                codigo = posicion + codigo;
                codigo = codigo % HashTable.Length;
            }

            return codigo;
        }

        public void Insertar(T valor, string Titulo)
        {
            int llave = Llave(Titulo);

            switch (llave)
            {
                case 0:
                    Lista0.InsertarInicio(valor);
                    break;
                case 1:
                    Lista1.InsertarInicio(valor);
                    break;
                case 2:
                    Lista2.InsertarInicio(valor);
                    break;
                case 3:
                    Lista3.InsertarInicio(valor);
                    break;
                case 4:
                    Lista4.InsertarInicio(valor);
                    break;
                case 5:
                    Lista5.InsertarInicio(valor);
                    break;
                case 6:
                    Lista6.InsertarInicio(valor);
                    break;
                case 7:
                    Lista7.InsertarInicio(valor);
                    break;
                case 8:
                    Lista8.InsertarInicio(valor);
                    break;
                case 9:
                    Lista9.InsertarInicio(valor);
                    break;
                default:
                    break;
            }
        }

        /* public T ObtenerValor(T titulo, int llave)
        {
            T valor;
            bool encontrar = false;
            int i = 0;
            switch (llave)
            {
                case 0:
                    while (encontrar == false)
                    {
                        valor = Lista0.ObtenerValor(i);
                        if (valor.CompareTo(titulo) == 0)
                            encontrar = true;
                        else
                            i++;
                    }
                    return valor;
                    break;
                case 1:
                    Lista1.InsertarInicio(valor);
                    break;
                case 2:
                    Lista2.InsertarInicio(valor);
                    break;
                case 3:
                    Lista3.InsertarInicio(valor);
                    break;
                case 4:
                    Lista4.InsertarInicio(valor);
                    break;
                case 5:
                    Lista5.InsertarInicio(valor);
                    break;
                case 6:
                    Lista6.InsertarInicio(valor);
                    break;
                case 7:
                    Lista7.InsertarInicio(valor);
                    break;
                case 8:
                    Lista8.InsertarInicio(valor);
                    break;
                case 9:
                    Lista9.InsertarInicio(valor);
                    break;
                default:
                    break;
            }
        }*/

    }
}
