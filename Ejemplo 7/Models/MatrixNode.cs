namespace Ejemplo_7.Models
{
    /// <summary>
    /// Representa un nodo de datos enlazado de forma ortogonal en la Matriz Dispersa.
    /// </summary>
    public class MatrixNode
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public string Id { get; set; } = "";
        public string Nombre { get; set; } = "";
        public string IpAddress { get; set; } = "";

        // Punteros de enlazado ortogonal bidireccional
        public MatrixNode? Up { get; set; }
        public MatrixNode? Down { get; set; }
        public MatrixNode? Left { get; set; }
        public MatrixNode? Right { get; set; }

        public MatrixNode(int row, int col, string id, string nombre, string ipAddress)
        {
            Row = row;
            Col = col;
            Id = id;
            Nombre = nombre;
            IpAddress = ipAddress;
            Up = null;
            Down = null;
            Left = null;
            Right = null;
        }
    }
}
