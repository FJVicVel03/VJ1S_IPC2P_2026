using System;

namespace Ejemplo_13.Models
{
    /// <summary>
    /// Estructura de Árbol Binario de Búsqueda (ABB) que funge como Cola de Prioridad en memoria.
    /// Almacena paquetes de datos (AbbNode) y los despacha según su prioridad (1 a 5).
    /// Cumple con la restricción de no usar colecciones genéricas de .NET.
    /// </summary>
    public class BufferMensajesAbb
    {
        // Raíz física del árbol binario en el Heap
        private AbbNode? root;

        /// <summary>
        /// Constructor predeterminado de la cola de prioridad.
        /// </summary>
        public BufferMensajesAbb()
        {
            root = null;
        }

        /// <summary>
        /// Obtiene el número total de paquetes almacenados actualmente en el buffer.
        /// </summary>
        public int Count
        {
            get { return ContarNodos(root); }
        }

        /// <summary>
        /// Indica si el buffer de mensajes está vacío.
        /// </summary>
        public bool IsEmpty
        {
            get { return root == null; }
        }

        /// <summary>
        /// Inserta un nuevo paquete de datos en el árbol binario.
        /// La ordenación se basa en la prioridad: valores iguales o mayores se dirigen al subárbol derecho,
        /// y valores menores se dirigen al subárbol izquierdo.
        /// </summary>
        /// <param name="packet">Nodo de datos a encolar.</param>
        public void Enqueue(AbbNode packet)
        {
            if (packet == null) return;
            root = Insertar(root, packet);
        }

        /// <summary>
        /// Lógica recursiva de inserción en el ABB.
        /// </summary>
        private AbbNode Insertar(AbbNode? actual, AbbNode nuevo)
        {
            // Caso base: Se encontró la posición de inserción disponible
            if (actual == null)
            {
                return nuevo;
            }

            // Si la prioridad es mayor o igual, se envía a la derecha.
            // Esto garantiza que el elemento de máxima prioridad siempre tienda hacia la extrema derecha.
            if (nuevo.Priority >= actual.Priority)
            {
                actual.Right = Insertar(actual.Right, nuevo);
            }
            else
            {
                actual.Left = Insertar(actual.Left, nuevo);
            }

            return actual;
        }

        /// <summary>
        /// Extrae y retorna el mensaje de máxima prioridad en el árbol (el nodo ubicado más a la derecha).
        /// Reestructura el árbol reconectando las ramas según las reglas estándar de eliminación de un ABB.
        /// </summary>
        /// <returns>El nodo con la prioridad más alta extraído, o null si está vacío.</returns>
        public AbbNode? Dequeue()
        {
            if (root == null)
            {
                return null;
            }

            // Caso especial: El nodo de extrema derecha es la raíz misma (no hay subárbol derecho)
            if (root.Right == null)
            {
                AbbNode maxNode = root;
                // La raíz se reemplaza por su subárbol izquierdo
                root = root.Left;

                // Desvincular enlaces físicos para evitar retención de referencias en memoria
                maxNode.Left = null;
                maxNode.Right = null;
                return maxNode;
            }

            // Caso general: Recorrer el árbol para encontrar el nodo más a la derecha y su padre
            AbbNode parent = root;
            AbbNode current = root.Right;

            while (current.Right != null)
            {
                parent = current;
                current = current.Right;
            }

            // 'current' es el nodo de extrema derecha (máxima prioridad)
            // Reconectar el posible subárbol izquierdo de 'current' a la rama derecha del padre
            parent.Right = current.Left;

            // Desvincular referencias del nodo extraído
            current.Left = null;
            current.Right = null;

            return current;
        }

        /// <summary>
        /// Recorrido recursivo que cuenta la cantidad total de nodos en el árbol.
        /// </summary>
        private int ContarNodos(AbbNode? nodo)
        {
            if (nodo == null) return 0;
            return 1 + ContarNodos(nodo.Left) + ContarNodos(nodo.Right);
        }

        /// <summary>
        /// Retorna un arreglo lineal nativo de C# conteniendo los paquetes en orden prioritario descendente
        /// (de mayor a menor prioridad: 5, 4, 3, 2, 1) para despliegue interactivo en la interfaz gráfica.
        /// </summary>
        public AbbNode[] ObtenerMensajesOrdenados()
        {
            int conteo = Count;
            AbbNode[] arr = new AbbNode[conteo];
            int index = 0;
            
            // Recorrido In-Order Inverso (Derecha -> Raíz -> Izquierda)
            // para rellenar el arreglo de mayor a menor prioridad.
            LlenarArregloInOrderInverso(root, arr, ref index);
            return arr;
        }

        /// <summary>
        /// Lógica recursiva de recorrido in-orden inverso para poblar el arreglo nativo.
        /// </summary>
        private void LlenarArregloInOrderInverso(AbbNode? nodo, AbbNode[] arr, ref int index)
        {
            if (nodo == null) return;

            // Procesar primero el subárbol derecho (prioridades mayores)
            LlenarArregloInOrderInverso(nodo.Right, arr, ref index);

            // Procesar el nodo actual
            if (index < arr.Length)
            {
                arr[index++] = nodo;
            }

            // Procesar el subárbol izquierdo (prioridades menores)
            LlenarArregloInOrderInverso(nodo.Left, arr, ref index);
        }
    }
}
