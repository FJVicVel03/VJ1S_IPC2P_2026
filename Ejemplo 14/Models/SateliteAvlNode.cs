using System;

namespace Ejemplo_14.Models
{
    /// <summary>
    /// Representa un nodo autorreferenciado para la estructura manual del Arbol AVL (Catalogo).
    /// No utiliza colecciones genericas de .NET.
    /// </summary>
    public class SateliteAvlNode
    {
        // Contenido del satelite encapsulado
        public Satelite Valor { get; set; }

        // Referencia al subarbol izquierdo
        public SateliteAvlNode? Izquierdo { get; set; }

        // Referencia al subarbol derecho
        public SateliteAvlNode? Derecho { get; set; }

        // Altura del nodo en el arbol para calculo de factores de balanceo
        public int Altura { get; set; }

        /// <summary>
        /// Constructor base que inicializa el nodo con su respectivo valor y altura inicial.
        /// </summary>
        /// <param name="satelite">Objeto satelite a almacenar.</param>
        public SateliteAvlNode(Satelite satelite)
        {
            Valor = satelite ?? throw new ArgumentNullException(nameof(satelite));
            Izquierdo = null;
            Derecho = null;
            Altura = 1; // Un nodo recien creado tiene altura inicial de 1
        }
    }
}


