using System;
using System.Collections.Generic;

namespace Ejemplo_6.Models
{
    /// <summary>
    /// Estructura manual de tipo Árbol AVL Auto-balanceado para Satélites.
    /// No utiliza colecciones dinámicas de .NET para la persistencia.
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

        // --- Operaciones Principales del TDA ---

        public void Insertar(Satelite satelite)
        {
            if (satelite == null) return;
            raiz = InsertarRecursivo(raiz, satelite);
        }

        public void Eliminar(string id)
        {
            if (string.IsNullOrEmpty(id)) return;
            if (Buscar(id) != null)
            {
                raiz = EliminarRecursivo(raiz, id);
                conteo--;
            }
        }

        public Satelite? Buscar(string id)
        {
            return BuscarRecursivo(raiz, id);
        }

        // --- Lógica Recursiva y de Balanceo AVL ---

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

        // Rotación Simple a la Derecha (LL Rotation)
        private SateliteAvlNode RotarDerecha(SateliteAvlNode y)
        {
            SateliteAvlNode? x = y.Izquierdo!;
            SateliteAvlNode? T2 = x.Derecho;

            // Realizar rotación
            x.Derecho = y;
            y.Izquierdo = T2;

            // Actualizar alturas
            ActualizarAltura(y);
            ActualizarAltura(x);

            return x;
        }

        // Rotación Simple a la Izquierda (RR Rotation)
        private SateliteAvlNode RotarIzquierda(SateliteAvlNode x)
        {
            SateliteAvlNode? y = x.Derecho!;
            SateliteAvlNode? T2 = y.Izquierdo;

            // Realizar rotación
            y.Izquierdo = x;
            x.Derecho = T2;

            // Actualizar alturas
            ActualizarAltura(x);
            ActualizarAltura(y);

            return y;
        }

        private SateliteAvlNode InsertarRecursivo(SateliteAvlNode? nodo, Satelite satelite)
        {
            // 1. Inserción normal de un BST
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
                // Clave duplicada: no se realiza inserción (o se actualiza valor)
                return nodo;
            }

            // 2. Actualizar la altura del nodo ancestro
            ActualizarAltura(nodo);

            // 3. Obtener factor de balance
            int balance = ObtenerFactorBalance(nodo);

            // 4. Si el nodo se desbalancea, aplicar rotaciones
            
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

        private SateliteAvlNode? EliminarRecursivo(SateliteAvlNode? nodo, string id)
        {
            if (nodo == null) return null;

            int comparacion = string.Compare(id, nodo.Valor.Id, StringComparison.OrdinalIgnoreCase);

            if (comparacion < 0)
            {
                nodo.Izquierdo = EliminarRecursivo(nodo.Izquierdo, id);
            }
            else if (comparacion > 0)
            {
                nodo.Derecho = EliminarRecursivo(nodo.Derecho, id);
            }
            else
            {
                // Nodo encontrado: eliminar este nodo
                
                // Caso 1: Sin hijos o un solo hijo
                if (nodo.Izquierdo == null || nodo.Derecho == null)
                {
                    SateliteAvlNode? temporal = nodo.Izquierdo ?? nodo.Derecho;

                    if (temporal == null)
                    {
                        // Caso sin hijos
                        nodo = null;
                    }
                    else
                    {
                        // Caso un hijo: copiar contenido del hijo
                        nodo = temporal;
                    }
                }
                else
                {
                    // Caso 2: Dos hijos. Obtener el sucesor en inorden (el menor del subárbol derecho)
                    SateliteAvlNode successor = ObtenerNodoMinimo(nodo.Derecho);
                    
                    // Copiar los datos del sucesor al nodo actual
                    nodo.Valor = successor.Valor;

                    // Eliminar el sucesor
                    nodo.Derecho = EliminarRecursivo(nodo.Derecho, successor.Valor.Id);
                }
            }

            if (nodo == null) return null;

            // Actualizar altura
            ActualizarAltura(nodo);

            // Obtener balance
            int balance = ObtenerFactorBalance(nodo);

            // Casos de desbalanceo por eliminación
            
            // Caso LL
            if (balance > 1 && ObtenerFactorBalance(nodo.Izquierdo) >= 0)
            {
                return RotarDerecha(nodo);
            }

            // Caso LR
            if (balance > 1 && ObtenerFactorBalance(nodo.Izquierdo) < 0)
            {
                nodo.Izquierdo = RotarIzquierda(nodo.Izquierdo!);
                return RotarDerecha(nodo);
            }

            // Caso RR
            if (balance < -1 && ObtenerFactorBalance(nodo.Derecho) <= 0)
            {
                return RotarIzquierda(nodo);
            }

            // Caso RL
            if (balance < -1 && ObtenerFactorBalance(nodo.Derecho) > 0)
            {
                nodo.Derecho = RotarDerecha(nodo.Derecho!);
                return RotarIzquierda(nodo);
            }

            return nodo;
        }

        private SateliteAvlNode ObtenerNodoMinimo(SateliteAvlNode nodo)
        {
            SateliteAvlNode actual = nodo;
            while (actual.Izquierdo != null)
            {
                actual = actual.Izquierdo;
            }
            return actual;
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

        // --- Recorridos ---

        public IEnumerable<Satelite> ObtenerInOrden()
        {
            return RecorrerInOrden(raiz);
        }

        private IEnumerable<Satelite> RecorrerInOrden(SateliteAvlNode? nodo)
        {
            if (nodo == null) yield break;

            foreach (var sat in RecorrerInOrden(nodo.Izquierdo))
            {
                yield return sat;
            }

            yield return nodo.Valor;

            foreach (var sat in RecorrerInOrden(nodo.Derecho))
            {
                yield return sat;
            }
        }

        /// <summary>
        /// Genera una lista de cadenas que representan visualmente la jerarquía del árbol AVL.
        /// </summary>
        public List<string> ObtenerEstructuraVisual()
        {
            List<string> resultado = new List<string>();
            ConstruirEstructuraVisual(raiz, "", true, resultado);
            return resultado;
        }

        private void ConstruirEstructuraVisual(SateliteAvlNode? nodo, string prefijo, bool esUltimo, List<string> resultado)
        {
            if (nodo == null) return;

            int balance = ObtenerFactorBalance(nodo);
            resultado.Add($"{prefijo}{(esUltimo ? "└── " : "├── ")}{nodo.Valor.Id} ({nodo.Valor.Nombre}) [H:{nodo.Altura} B:{balance}]");

            string nuevoPrefijo = prefijo + (esUltimo ? "    " : "│   ");

            if (nodo.Izquierdo != null || nodo.Derecho != null)
            {
                if (nodo.Izquierdo != null)
                {
                    ConstruirEstructuraVisual(nodo.Izquierdo, nuevoPrefijo, nodo.Derecho == null, resultado);
                }
                else
                {
                    resultado.Add($"{nuevoPrefijo}├── [Izq: Vacío]");
                }

                if (nodo.Derecho != null)
                {
                    ConstruirEstructuraVisual(nodo.Derecho, nuevoPrefijo, true, resultado);
                }
                else
                {
                    resultado.Add($"{nuevoPrefijo}└── [Der: Vacío]");
                }
            }
        }
    }
}
