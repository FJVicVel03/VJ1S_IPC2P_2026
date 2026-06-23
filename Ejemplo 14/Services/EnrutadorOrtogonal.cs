using System;
using Ejemplo_14.Models;

namespace Ejemplo_14.Services
{
    /// <summary>
    /// Servicio especializado en calcular la ruta de saltos lógicos ortogonales
    /// a través de los punteros directos (Up, Down, Left, Right) de los satélites de la Matriz Dispersa.
    /// No utiliza colecciones genéricas de .NET.
    /// </summary>
    public class EnrutadorOrtogonal
    {
        // Almacena temporalmente la mejor ruta encontrada durante el recorrido DFS
        private MatrixNode[]? rutaEncontrada;

        /// <summary>
        /// Busca y retorna un arreglo nativo de MatrixNode que representa la ruta de saltos
        /// ortogonales desde un satélite origen hasta un satélite de destino.
        /// </summary>
        /// <param name="matriz">Instancia de la Matriz Dispersa Ortogonal.</param>
        /// <param name="origenId">Identificador del satélite de origen.</param>
        /// <param name="destinoId">Identificador del satélite de destino.</param>
        /// <returns>Arreglo conteniendo la secuencia de nodos de tránsito, o un arreglo vacío si no hay ruta.</returns>
        public MatrixNode[] EncontrarRuta(RedSatelitalPlano matriz, string origenId, string destinoId)
        {
            rutaEncontrada = null;

            // 1. Validar que la matriz no se encuentre vacía y los identificadores sean válidos.
            if (matriz.IsEmpty || string.IsNullOrWhiteSpace(origenId) || string.IsNullOrWhiteSpace(destinoId))
            {
                return Array.Empty<MatrixNode>();
            }

            // 2. Localizar los nodos físicos correspondientes en la memoria de la matriz.
            MatrixNode? origenNode = matriz.BuscarPorId(origenId);
            MatrixNode? destinoNode = matriz.BuscarPorId(destinoId);

            // 3. Si alguno de los dos extremos no existe, es imposible trazar una ruta.
            if (origenNode == null || destinoNode == null)
            {
                return Array.Empty<MatrixNode>();
            }

            // 4. Si el origen es el mismo destino, la ruta consta de un único nodo y cero saltos.
            if (origenNode == destinoNode)
            {
                return new MatrixNode[] { origenNode };
            }

            // 5. Instanciar estructuras nativas de apoyo para rastrear el camino explorado.
            //    Dado que no podemos usar List<T>, reservamos un arreglo con el tamaño total
            //    de nodos posibles en la matriz.
            MatrixNode[] caminoActual = new MatrixNode[matriz.Count];

            // 6. Lanzar la búsqueda en profundidad (DFS) en busca del destino.
            ResolverDfs(origenNode, destinoNode, caminoActual, 0);

            // 7. Retornar el resultado definitivo (o un arreglo vacío si el DFS no localizó ningún camino).
            return rutaEncontrada ?? Array.Empty<MatrixNode>();
        }

        /// <summary>
        /// Algoritmo recursivo DFS con backtracking que explora los vecinos ortogonales.
        /// </summary>
        private void ResolverDfs(MatrixNode actual, MatrixNode destino, MatrixNode[] camino, int index)
        {
            // Si ya encontramos un camino válido en otra rama, abortamos búsquedas redundantes
            if (rutaEncontrada != null) return;

            // Evitar desbordamiento de límites de seguridad
            if (index >= camino.Length) return;

            // Registrar el nodo actual en el paso del camino
            camino[index] = actual;

            // Caso base: Hemos alcanzado el satélite destino final
            if (actual == destino)
            {
                // Instanciar un arreglo nativo ajustado al tamaño exacto de saltos dados
                rutaEncontrada = new MatrixNode[index + 1];
                for (int i = 0; i <= index; i++)
                {
                    rutaEncontrada[i] = camino[i];
                }
                return;
            }

            // Probar vecinos ortogonales según la física de enlazado de la matriz dispersa
            // Orden de exploración: Derecha, Izquierda, Abajo, Arriba
            MatrixNode?[] vecinos = new MatrixNode?[] { actual.Right, actual.Left, actual.Down, actual.Up };

            for (int i = 0; i < vecinos.Length; i++)
            {
                MatrixNode? vecino = vecinos[i];
                
                // Si el vecino existe y no provoca un bucle o ciclo (no visitado en el camino actual)
                if (vecino != null && !EstaEnCamino(vecino, camino, index))
                {
                    // Avanzar recursivamente incrementando el índice del camino
                    ResolverDfs(vecino, destino, camino, index + 1);
                }
            }
        }

        /// <summary>
        /// Comprueba de forma lineal si un vecino ya forma parte del camino actual
        /// para prevenir ciclos infinitos de backtracking.
        /// </summary>
        private bool EstaEnCamino(MatrixNode nodo, MatrixNode[] camino, int index)
        {
            for (int i = 0; i <= index; i++)
            {
                if (camino[i] == nodo)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
