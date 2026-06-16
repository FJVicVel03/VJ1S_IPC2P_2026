using System;

namespace Ejemplo_12.Models
{
    /// <summary>
    /// Estructura manual de tipo Arbol AVL Auto-balanceado para Satelites.
    /// No utiliza colecciones dinamicas ni genericas de .NET (System.Collections.Generic).
    /// </summary>
    public class ArbolSatelitesAvl
    {
        private SateliteAvlNode? raiz;
        private int conteo;

        public ArbolSatelitesAvl()
        {
            raiz = null;
            conteo = 0;
        }

        public int Conteo => conteo;
        public bool EstaVacio => raiz == null;

        public void Limpiar()
        {
            raiz = null;
            conteo = 0;
        }

        /// <summary>
        /// Inserta un nuevo satelite en el catalogo AVL. Re-balancea el arbol de ser necesario.
        /// </summary>
        public void Insertar(Satelite satelite)
        {
            if (satelite == null) return;
            raiz = InsertarRecursivo(raiz, satelite);
        }

        /// <summary>
        /// Busca un satelite en el catalogo por su identificador unico.
        /// </summary>
        public Satelite? Buscar(string id)
        {
            return BuscarRecursivo(raiz, id);
        }

        // --- Metodos Auxiliares de Balanceo y Rotaciones AVL ---

        private int ObtenerAltura(SateliteAvlNode? nodo)
        {
            return nodo?.Altura ?? 0;
        }

        private int ObtenerFactorBalance(SateliteAvlNode? nodo)
        {
            if (nodo == null) return 0;
            return ObtenerAltura(nodo.Izquierdo) - ObtenerAltura(nodo.Derecho);
        }

        private void ActualizarAltura(SateliteAvlNode nodo)
        {
            nodo.Altura = Math.Max(ObtenerAltura(nodo.Izquierdo), ObtenerAltura(nodo.Derecho)) + 1;
        }

        // Rotacion Simple a la Derecha (LL Rotation)
        private SateliteAvlNode RotarDerecha(SateliteAvlNode y)
        {
            SateliteAvlNode? x = y.Izquierdo!;
            SateliteAvlNode? T2 = x.Derecho;

            // Ejecutar la rotacion de punteros
            x.Derecho = y;
            y.Izquierdo = T2;

            // Actualizar alturas calculadas de abajo hacia arriba
            ActualizarAltura(y);
            ActualizarAltura(x);

            return x;
        }

        // Rotacion Simple a la Izquierda (RR Rotation)
        private SateliteAvlNode RotarIzquierda(SateliteAvlNode x)
        {
            SateliteAvlNode? y = x.Derecho!;
            SateliteAvlNode? T2 = y.Izquierdo;

            // Ejecutar la rotacion de punteros
            y.Izquierdo = x;
            x.Derecho = T2;

            // Actualizar alturas calculadas de abajo hacia arriba
            ActualizarAltura(x);
            ActualizarAltura(y);

            return y;
        }

        private SateliteAvlNode InsertarRecursivo(SateliteAvlNode? nodo, Satelite satelite)
        {
            // 1. Insercion estandar de Arbol Binario de Busqueda (BST)
            if (nodo == null)
            {
                conteo++;
                return new SateliteAvlNode(satelite);
            }

            int comparacion = string.Compare(satelite.Id, nodo.Valor.Id, StringComparison.OrdinalIgnoreCase);

            if (comparacion < 0)
            {
                nodo.Izquierdo = InsertarRecursivo(nodo.Izquierdo, satelite);
            }
            else if (comparacion > 0)
            {
                nodo.Derecho = InsertarRecursivo(nodo.Derecho, satelite);
            }
            else
            {
                // Clave duplicada: no se realiza la insercion
                return nodo;
            }

            // 2. Actualizar altura del ancestro actual
            ActualizarAltura(nodo);

            // 3. Obtener el factor de balanceo para determinar si hay desequilibrio
            int balance = ObtenerFactorBalance(nodo);

            // 4. Evaluar desbalanceo y aplicar rotaciones correspondientes

            // Caso Izquierda-Izquierda (LL)
            if (balance > 1 && string.Compare(satelite.Id, nodo.Izquierdo!.Valor.Id, StringComparison.OrdinalIgnoreCase) < 0)
            {
                return RotarDerecha(nodo);
            }

            // Caso Derecha-Derecha (RR)
            if (balance < -1 && string.Compare(satelite.Id, nodo.Derecho!.Valor.Id, StringComparison.OrdinalIgnoreCase) > 0)
            {
                return RotarIzquierda(nodo);
            }

            // Caso Izquierda-Derecha (LR)
            if (balance > 1 && string.Compare(satelite.Id, nodo.Izquierdo!.Valor.Id, StringComparison.OrdinalIgnoreCase) > 0)
            {
                nodo.Izquierdo = RotarIzquierda(nodo.Izquierdo);
                return RotarDerecha(nodo);
            }

            // Caso Derecha-Izquierda (RL)
            if (balance < -1 && string.Compare(satelite.Id, nodo.Derecho!.Valor.Id, StringComparison.OrdinalIgnoreCase) < 0)
            {
                nodo.Derecho = RotarDerecha(nodo.Derecho);
                return RotarIzquierda(nodo);
            }

            return nodo;
        }

        private Satelite? BuscarRecursivo(SateliteAvlNode? nodo, string id)
        {
            if (nodo == null) return null;

            int comparacion = string.Compare(id, nodo.Valor.Id, StringComparison.OrdinalIgnoreCase);

            if (comparacion == 0) return nodo.Valor;

            if (comparacion < 0)
            {
                return BuscarRecursivo(nodo.Izquierdo, id);
            }

            return BuscarRecursivo(nodo.Derecho, id);
        }

        // --- Recorrido In-Order en un Arreglo Nativo C# ---

        /// <summary>
        /// Recorre el arbol en in-orden de menor a mayor por ID y retorna los datos en un arreglo nativo.
        /// Evita el uso de System.Collections.Generic.
        /// </summary>
        public Satelite[] ObtenerTodos()
        {
            Satelite[] arrayResult = new Satelite[conteo];
            int index = 0;
            LlenarArregloInOrden(raiz, arrayResult, ref index);
            return arrayResult;
        }

        private void LlenarArregloInOrden(SateliteAvlNode? nodo, Satelite[] arr, ref int idx)
        {
            if (nodo == null) return;

            // 1. Recorrer subarbol izquierdo
            LlenarArregloInOrden(nodo.Izquierdo, arr, ref idx);

            // 2. Procesar nodo actual
            if (idx < arr.Length)
            {
                arr[idx++] = nodo.Valor;
            }

            // 3. Recorrer subarbol derecho
            LlenarArregloInOrden(nodo.Derecho, arr, ref idx);
        }
    }
}
