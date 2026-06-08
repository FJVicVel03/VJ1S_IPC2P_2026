namespace Ejemplo_6.Models
{
    /// <summary>
    /// Representa un nodo autorreferenciado para el árbol AVL (RegistroSatelites).
    /// </summary>
    public class SateliteAvlNode
    {
        public Satelite Valor { get; set; }
        public SateliteAvlNode? Izquierdo { get; set; }
        public SateliteAvlNode? Derecho { get; set; }
        public int Altura { get; set; }

        public SateliteAvlNode(Satelite satelite)
        {
            Valor = satelite;
            Izquierdo = null;
            Derecho = null;
            Altura = 1; // La altura inicial de un nodo hoja es 1
        }
    }
}
