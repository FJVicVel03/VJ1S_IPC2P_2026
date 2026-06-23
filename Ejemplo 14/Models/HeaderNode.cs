namespace Ejemplo_14.Models
{
    /// <summary>
    /// Representa un nodo cabecera de fila o columna para la Matriz Dispersa.
    /// </summary>
    public class HeaderNode
    {
        public int Index { get; set; }
        public HeaderNode? Next { get; set; }
        
        // Puntero de acceso al primer nodo de datos en esta fila o columna
        public MatrixNode? Access { get; set; }

        public HeaderNode(int index)
        {
            Index = index;
            Next = null;
            Access = null;
        }
    }
}





