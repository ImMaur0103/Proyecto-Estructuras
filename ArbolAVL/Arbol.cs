using System;
using System.Collections.Generic;
using System.Text;

namespace ArbolAVL
{
    // Firma del delegado
    public delegate PacienteArbol BuscarCampo(string dato, ArbolAVL.Arbol<PacienteArbol> ArbolAvl);
    
    public class Arbol<T>
    {
        ArbolAVL<T> ArbolAVL = new ArbolAVL<T>();

        public NodoAVL<T> raiz;
        public int contador;

        //constructor 
        public Arbol()
        {
            raiz = null;
            contador = 0;
        }

        ~Arbol() { }

        // Insertar nodos en el árbol 
        public void InsertarNombres(PacienteArbol valor, ArbolAVL.Arbol<PacienteArbol> Arbol)
        {
            NodoAVL<T> NuevoNodo = new NodoAVL<T>();
            NuevoNodo.valor = valor;
            NuevoNodo.izquierda = null;
            NuevoNodo.derecha = null;
            if (Buscar(valor.Nombre, Arbol) != null)
            {
                raiz = InsertarNodoApellido(raiz, NuevoNodo);
                contador++;
                return;
            }

            if (raiz == null)
            {
                raiz = NuevoNodo;
            }
            else
            {
                raiz = InsertarNodoNombre(raiz, NuevoNodo);
            }
            contador++;
        }
        public void InsertarApellidos(PacienteArbol valor, ArbolAVL.Arbol<PacienteArbol> Arbol)
        {
            NodoAVL<T> NuevoNodo = new NodoAVL<T>();
            NuevoNodo.valor = valor;
            NuevoNodo.izquierda = null;
            NuevoNodo.derecha = null;
            if (Buscar(valor.Nombre, Arbol) != null)
            {
                raiz = InsertarNodoNombre(raiz, NuevoNodo);
                contador++;
                return;
            }

            if (raiz == null)
            {
                raiz = NuevoNodo;
            }
            else
            {
                raiz = InsertarNodoApellido(raiz, NuevoNodo);
            }
            contador++;
        }
        private NodoAVL<T> InsertarNodoNombre(NodoAVL<T> actual, NodoAVL<T> nuevo)
        {
            NodoAVL<T> Raiz = actual;

            if (nuevo.valor.Nombre.CompareTo(actual.valor.Nombre) < 0)
            {
                if (actual.izquierda == null)
                {
                    actual.izquierda = nuevo;
                }
                else
                {
                    actual.izquierda = InsertarNodoNombre(actual.izquierda, nuevo);
                    if ((ArbolAVL.CalcFe(actual.izquierda) - ArbolAVL.CalcFe(actual.derecha)) == 2)
                    {
                        if (nuevo.valor.Nombre.CompareTo(actual.izquierda.valor.Nombre) < 0)
                        {
                            Raiz = ArbolAVL.RotarIzquierda(actual);
                        }
                        else
                        {
                            Raiz = ArbolAVL.RDobleIzquierda(actual);
                        }
                    }
                }
            }
            else if (nuevo.valor.Nombre.CompareTo(actual.valor.Nombre) > 0)
            {
                if (actual.derecha == null)
                {
                    actual.derecha = nuevo;
                }
                else
                {
                    actual.derecha = InsertarNodoNombre(actual.derecha, nuevo);
                    if ((ArbolAVL.CalcFe(actual.derecha) - ArbolAVL.CalcFe(actual.izquierda)) == 2)
                    {
                        if (nuevo.valor.Nombre.CompareTo(actual.derecha.valor.Nombre) > 0)
                        {
                            Raiz = ArbolAVL.RotarDerecha(actual);
                        }
                        else
                        {
                            Raiz = ArbolAVL.RDobleDerecha(actual);
                        }
                    }
                }
            }
            else
            {
                return null;
            }

            if ((actual.izquierda == null) && (actual.derecha != null))
            {
                actual.Fe = actual.derecha.Fe + 1;
            }
            else if ((actual.derecha == null) && (actual.izquierda != null))
            {
                actual.Fe = actual.izquierda.Fe + 1;
            }
            else
            {
                actual.Fe = Math.Max(ArbolAVL.CalcFe(actual.izquierda), ArbolAVL.CalcFe(actual.derecha)) + 1;
            }

            return Raiz;
        }

        private NodoAVL<T> InsertarNodoApellido(NodoAVL<T> actual, NodoAVL<T> nuevo)
        {
            NodoAVL<T> Raiz = actual;

            if (nuevo.valor.Apellido.CompareTo(actual.valor.Apellido) < 0)
            {
                if (actual.izquierda == null)
                {
                    actual.izquierda = nuevo;
                }
                else
                {
                    actual.izquierda = InsertarNodoApellido(actual.izquierda, nuevo);
                    if ((ArbolAVL.CalcFe(actual.izquierda) - ArbolAVL.CalcFe(actual.derecha)) == 2)
                    {
                        if (nuevo.valor.Apellido.CompareTo(actual.izquierda.valor.Apellido) < 0)
                        {
                            Raiz = ArbolAVL.RotarIzquierda(actual);
                        }
                        else
                        {
                            Raiz = ArbolAVL.RDobleIzquierda(actual);
                        }
                    }
                }
            }
            else if (nuevo.valor.Apellido.CompareTo(actual.valor.Apellido) > 0)
            {
                if (actual.derecha == null)
                {
                    actual.derecha = nuevo;
                }
                else
                {
                    actual.derecha = InsertarNodoApellido(actual.derecha, nuevo);
                    if ((ArbolAVL.CalcFe(actual.derecha) - ArbolAVL.CalcFe(actual.izquierda)) == 2)
                    {
                        if (nuevo.valor.Apellido.CompareTo(actual.derecha.valor.Apellido) > 0)
                        {
                            Raiz = ArbolAVL.RotarDerecha(actual);
                        }
                        else
                        {
                            Raiz = ArbolAVL.RDobleDerecha(actual);
                        }
                    }
                }
            }
            else
            {
                return null;
            }

            if ((actual.izquierda == null) && (actual.derecha != null))
            {
                actual.Fe = actual.derecha.Fe + 1;
            }
            else if ((actual.derecha == null) && (actual.izquierda != null))
            {
                actual.Fe = actual.izquierda.Fe + 1;
            }
            else
            {
                actual.Fe = Math.Max(ArbolAVL.CalcFe(actual.izquierda), ArbolAVL.CalcFe(actual.derecha)) + 1;
            }

            return Raiz;
        }
        public void InsertarValor(PacienteArbol valor, ArbolAVL.Arbol<PacienteArbol> Arbol)
        {
            NodoAVL<T> NuevoNodo = new NodoAVL<T>();
            NuevoNodo.valor = valor;
            NuevoNodo.izquierda = null;
            NuevoNodo.derecha = null;
            if (BuscarNumero(valor.DPI_CUI.ToString(), Arbol/*valor.DPI_CUI*/) != null)
            {
                return;
            }

            if (raiz == null)
            {
                raiz = NuevoNodo;
            }
            else
            {
                raiz = InsertarNodoValor(raiz, NuevoNodo);
            }
            contador++;
        }




        private NodoAVL<T> InsertarNodoValor(NodoAVL<T> actual, NodoAVL<T> nuevo)
        {
            NodoAVL<T> Raiz = actual;

            if (nuevo.valor.DPI_CUI < actual.valor.DPI_CUI)
            {
                if (actual.izquierda == null)
                {
                    actual.izquierda = nuevo;
                }
                else
                {
                    actual.izquierda = InsertarNodoValor(actual.izquierda, nuevo);
                    if ((ArbolAVL.CalcFe(actual.izquierda) - ArbolAVL.CalcFe(actual.derecha)) == 2)
                    {
                        if (nuevo.valor.Nombre.CompareTo(actual.izquierda.valor.Nombre) < 0)
                        {
                            Raiz = ArbolAVL.RotarIzquierda(actual);
                        }
                        else
                        {
                            Raiz = ArbolAVL.RDobleIzquierda(actual);
                        }
                    }
                }
            }
            else if (nuevo.valor.DPI_CUI > actual.valor.DPI_CUI)
            {
                if (actual.derecha == null)
                {
                    actual.derecha = nuevo;
                }
                else
                {
                    actual.derecha = InsertarNodoValor(actual.derecha, nuevo);
                    if ((ArbolAVL.CalcFe(actual.derecha) - ArbolAVL.CalcFe(actual.izquierda)) == 2)
                    {
                        if (nuevo.valor.Nombre.CompareTo(actual.derecha.valor.Nombre) > 0)
                        {
                            Raiz = ArbolAVL.RotarDerecha(actual);
                        }
                        else
                        {
                            Raiz = ArbolAVL.RDobleDerecha(actual);
                        }
                    }
                }
            }
            else
            {
                return null;
            }

            if ((actual.izquierda == null) && (actual.derecha != null))
            {
                actual.Fe = actual.derecha.Fe + 1;
            }
            else if ((actual.derecha == null) && (actual.izquierda != null))
            {
                actual.Fe = actual.izquierda.Fe + 1;
            }
            else
            {
                actual.Fe = Math.Max(ArbolAVL.CalcFe(actual.izquierda), ArbolAVL.CalcFe(actual.derecha)) + 1;
            }

            return Raiz;
        }


        public T Mayor<T>(T valor1, T valor2) where T : IComparable
        {
            if (valor1.CompareTo(valor2) > 0) return valor1;
            return valor2;
        }

        //Métodos de búsqueda
        public PacienteArbol BuscarNumero(string numero, ArbolAVL.Arbol<PacienteArbol> ArbolAvl)
        {
            numero.Replace(" ", "");
            long dpi = Convert.ToInt64(numero);
            NodoAVL<PacienteArbol> recorrer = ArbolAvl.raiz;
            bool encontrar = false;
            while (recorrer != null && encontrar == false)
            {
                long valor = recorrer.valor.DPI_CUI;
                if (dpi == valor)
                {
                    encontrar = true;
                }
                else
                {
                    if (dpi > recorrer.valor.DPI_CUI)
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
                return null;
            }
            return recorrer.valor;
        }

        public PacienteArbol Buscar(string nombre, ArbolAVL.Arbol<PacienteArbol> ArbolAvl)
        {
            NodoAVL<PacienteArbol> recorrer = ArbolAvl.raiz;
            nombre = nombre.ToLower();
            nombre = nombre.Replace(" ", "");
            bool encontrar = false;
            while (recorrer != null && encontrar == false)
            {
                string valor = recorrer.valor.Nombre.Replace(" ","");
                valor = valor.ToLower();
                if (nombre == valor)
                {
                    encontrar = true;
                }
                else
                {
                    if (nombre.CompareTo(recorrer.valor.Nombre) > 0)
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
                return null;
            }
            return recorrer.valor;
        }

        public PacienteArbol BuscarA(string nombre, ArbolAVL.Arbol<PacienteArbol> ArbolAvl)
        {
            NodoAVL<PacienteArbol> recorrer = ArbolAvl.raiz;
            nombre = nombre.ToLower();
            nombre.Replace(" ", "");
            bool encontrar = false;
            while (recorrer != null && encontrar == false)
            {
                string valor = recorrer.valor.Apellido.Replace(" ", "");
                valor = valor.ToLower();
                if (nombre == valor)
                {
                    encontrar = true;
                }
                else
                {
                    if (nombre.CompareTo(recorrer.valor.Apellido) > 0)
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
                return null;
            }
            return recorrer.valor;
        }

        // Método del delegado
        public PacienteArbol RetornarValor(ArbolAVL.Arbol<PacienteArbol> ListaDatos, string dato, BuscarCampo CampoBusqueda)
        {
            return CampoBusqueda(dato, ListaDatos);
        }

        public NodoAVL<T> DeleteNodo(NodoAVL<T> actual, NodoAVL<T> Borrar)
        {
            if (actual == null)
                return null;

            if (Borrar.valor.Nombre.CompareTo(actual.valor.Nombre) < 0)
            {
                if (actual.izquierda == null)
                {
                    return actual;
                }
                else if (actual.izquierda.valor.Nombre == Borrar.valor.Nombre)
                {
                    if (actual.izquierda.izquierda == null && actual.izquierda.derecha == null)
                    {
                        actual.izquierda = null;
                    }
                    else if (actual.izquierda.izquierda != null && actual.izquierda.derecha != null)
                    {
                        NodoAVL<T> aux = actual.izquierda;
                        aux = Rearmar(aux, aux.derecha);
                        AjusteFeDerecha(ref aux);
                        aux = ArbolAVL.Balancear(aux);
                        actual.izquierda = aux;
                    }
                    else if (actual.izquierda.izquierda != null && actual.izquierda.derecha == null)
                    {
                        actual.izquierda = actual.izquierda.izquierda;
                    }
                    else if (actual.izquierda.izquierda == null && actual.izquierda.derecha != null)
                    {
                        actual.izquierda = actual.izquierda.derecha;
                    }
                    actual = ArbolAVL.Balancear(actual);
                }
                else
                {
                    actual.izquierda = DeleteNodo(actual.izquierda, Borrar);
                }
            }
            else if (Borrar.valor.Nombre.CompareTo(actual.valor.Nombre) > 0)
            {
                if (actual.derecha == null)
                {
                    return actual;
                }
                else if (actual.derecha.valor.Nombre == Borrar.valor.Nombre)
                {
                    if (actual.derecha.izquierda == null && actual.derecha.derecha == null)
                    {
                        actual.derecha = null;
                    }
                    else if (actual.derecha.izquierda != null && actual.derecha.derecha != null)
                    {
                        NodoAVL<T> aux = actual.derecha;
                        aux = Rearmar(aux, aux.derecha);
                        AjusteFeDerecha(ref aux);
                        aux = ArbolAVL.Balancear(aux);
                        actual.derecha = aux;
                    }
                    else if (actual.derecha.izquierda != null && actual.derecha.derecha == null)
                    {
                        actual.derecha = actual.derecha.izquierda;
                    }
                    else if (actual.derecha.izquierda == null && actual.derecha.derecha != null)
                    {
                        actual.derecha = actual.derecha.derecha;
                    }
                    actual = ArbolAVL.Balancear(actual);
                }
                else
                {
                    actual.derecha = DeleteNodo(actual.derecha, Borrar);
                }
            }
            else if (Borrar.valor.Nombre.CompareTo(actual.valor.Nombre) == 0)
            {
                if (actual.izquierda == null && actual.derecha == null)
                {
                    return null;
                }
                else if (actual.izquierda != null && actual.derecha != null)
                {
                    NodoAVL<T> aux = actual;
                    aux = Rearmar(aux, aux.derecha);
                    AjusteFeDerecha(ref aux);
                    aux = ArbolAVL.Balancear(aux);
                    actual = aux;
                }
                else if (actual.izquierda.izquierda != null && actual.izquierda.derecha == null)
                {
                    actual = actual.izquierda;
                }
                else if (actual.izquierda.izquierda == null && actual.izquierda.derecha != null)
                {
                    actual = actual.derecha;
                }
                actual = ArbolAVL.Balancear(actual);
            }

            if ((actual.izquierda == null) && (actual.derecha != null))
            {
                actual.Fe = actual.derecha.Fe - 1;
            }
            else if ((actual.derecha == null) && (actual.izquierda != null))
            {
                actual.Fe = actual.izquierda.Fe - 1;
            }
            else
            {
                actual.Fe = Math.Max(ArbolAVL.CalcFe(actual.izquierda), ArbolAVL.CalcFe(actual.derecha)) + 1;
            }

            NodoAVL<T> Raiz = actual;
            return Raiz;
        }

        private NodoAVL<T> Rearmar(NodoAVL<T> raiz, NodoAVL<T> auxiliar)
        {
            NodoAVL<T> RaizAux = new NodoAVL<T>();

            if (auxiliar.izquierda != null)
            {
                RaizAux = Rearmar(auxiliar, auxiliar.izquierda);
                if (RaizAux.derecha != null)
                {
                    auxiliar.izquierda = RaizAux.derecha;
                    RaizAux.derecha = auxiliar;
                }
                else
                {
                    auxiliar.izquierda = null;
                    RaizAux.derecha = auxiliar;
                }
            }
            else
            {
                RaizAux = auxiliar;
            }
            RaizAux.izquierda = raiz.izquierda;
            return RaizAux;
        }

        private void AjusteFeIzquierda(ref NodoAVL<T> raiz)
        {
            if (raiz.izquierda == null && raiz.derecha == null)
            {
                raiz.Fe = 0;
                return;
            }
            else if (raiz.izquierda == null && raiz.derecha != null)
            {
                NodoAVL<T> aux = raiz.derecha;
                AjusteFeDerecha(ref aux);
                raiz.Fe = raiz.derecha.Fe + 1;
            }
            else if (raiz.izquierda != null && raiz.derecha == null)
            {
                NodoAVL<T> aux = raiz.izquierda;
                AjusteFeIzquierda(ref aux);
                raiz.Fe = raiz.izquierda.Fe + 1;
            }
            else if (raiz.izquierda != null && raiz.derecha != null)
            {
                NodoAVL<T> AuxI = raiz.izquierda;
                NodoAVL<T> AuxD = raiz.derecha;
                AjusteFeDerecha(ref AuxD);
                AjusteFeIzquierda(ref AuxI);
                raiz.Fe = Math.Max(raiz.izquierda.Fe, raiz.derecha.Fe) + 1;
            }
        }

        private void AjusteFeDerecha(ref NodoAVL<T> raiz)
        {
            if (raiz.izquierda == null && raiz.derecha == null)
            {
                raiz.Fe = 0;
                return;
            }
            else if (raiz.izquierda == null && raiz.derecha != null)
            {
                NodoAVL<T> aux = raiz.derecha;
                AjusteFeDerecha(ref aux);
                raiz.Fe = raiz.derecha.Fe + 1;
            }
            else if (raiz.izquierda != null && raiz.derecha == null)
            {
                NodoAVL<T> aux = raiz.izquierda;
                AjusteFeIzquierda(ref aux);
                raiz.Fe = raiz.izquierda.Fe + 1;
            }
            else if (raiz.izquierda != null && raiz.derecha != null)
            {
                NodoAVL<T> AuxI = raiz.izquierda;
                NodoAVL<T> AuxD = raiz.derecha;
                AjusteFeDerecha(ref AuxD);
                AjusteFeIzquierda(ref AuxI);
                raiz.Fe = Math.Max(raiz.izquierda.Fe, raiz.derecha.Fe) + 1;
            }
        }

        public void Delete()
        {
            raiz = null;
            contador = 0;
        }
        //Verifica el estado del índice, por lo que guarda los valores dentro de una lista tipo FARMACO
        public void Preorden(NodoAVL<PacienteArbol> raiz, ref ListaDobleEnlace.ListaDoble<PacienteArbol> ListaInventario)
        {
            if (raiz != null)
            {
                ListaInventario.InsertarFinal(raiz.valor);
                Preorden(raiz.izquierda, ref ListaInventario);
                Preorden(raiz.derecha, ref ListaInventario);
            }
        }

        public void InOrden(NodoAVL<PacienteArbol> raiz, ref ListaDobleEnlace.ListaDoble<PacienteArbol> ListaInventario)
        {
            if (raiz != null)
            {
                InOrden(raiz.izquierda, ref ListaInventario);
                ListaInventario.InsertarFinal(raiz.valor);
                InOrden(raiz.derecha, ref ListaInventario);
            }
        }

        public void PostOrden(NodoAVL<PacienteArbol> raiz, ref ListaDobleEnlace.ListaDoble<PacienteArbol> ListaInventario)
        {
            if (raiz != null)
            {
                PostOrden(raiz.izquierda, ref ListaInventario);
                PostOrden(raiz.derecha, ref ListaInventario);
                ListaInventario.InsertarFinal(raiz.valor);
            }
        }
    }
}
