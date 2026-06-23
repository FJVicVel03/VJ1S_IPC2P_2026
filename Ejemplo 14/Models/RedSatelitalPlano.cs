using System;
using System.Text;

namespace Ejemplo_14.Models
{
    /// <summary>
    /// TDA RedSatelitalPlano: Matriz Dispersa Ortogonal Bidireccional.
    /// No utiliza System.Collections o System.Collections.Generic.
    /// </summary>
    public class RedSatelitalPlano
    {
        private HeaderNode? rowsHead; // Cabecera para filas (Y)
        private HeaderNode? colsHead; // Cabecera para columnas (X)
        private int nodeCount;

        public RedSatelitalPlano()
        {
            rowsHead = null;
            colsHead = null;
            nodeCount = 0;
        }

        public int Count => nodeCount;
        public bool IsEmpty => nodeCount == 0;

        public void Clear()
        {
            rowsHead = null;
            colsHead = null;
            nodeCount = 0;
        }

        // --- Operación de Inserción ---
        public void Insert(int row, int col, string id, string nombre, string ip)
        {
            // Validar que no exista colisión de coordenadas
            if (Search(row, col) != null)
            {
                throw new InvalidOperationException($"Colisión detectada: ya existe un satélite en las coordenadas ({row}, {col}).");
            }

            MatrixNode nuevoNodo = new MatrixNode(row, col, id, nombre, ip);

            // Obtener o crear cabeceras correspondientes de forma ordenada
            HeaderNode filaCabecera = GetOrCreateRowHeader(row);
            HeaderNode colCabecera = GetOrCreateColHeader(col);

            // 1. Inserción ordenada horizontalmente (en la fila, de menor a mayor columna)
            if (filaCabecera.Access == null)
            {
                filaCabecera.Access = nuevoNodo;
            }
            else if (col < filaCabecera.Access.Col)
            {
                // Insertar al inicio de la fila
                nuevoNodo.Right = filaCabecera.Access;
                filaCabecera.Access.Left = nuevoNodo;
                filaCabecera.Access = nuevoNodo;
            }
            else
            {
                MatrixNode actual = filaCabecera.Access;
                while (actual.Right != null && actual.Right.Col < col)
                {
                    actual = actual.Right;
                }
                // Insertar en medio o al final de la fila
                nuevoNodo.Right = actual.Right;
                if (actual.Right != null)
                {
                    actual.Right.Left = nuevoNodo;
                }
                actual.Right = nuevoNodo;
                nuevoNodo.Left = actual;
            }

            // 2. Inserción ordenada verticalmente (en la columna, de menor a mayor fila)
            if (colCabecera.Access == null)
            {
                colCabecera.Access = nuevoNodo;
            }
            else if (row < colCabecera.Access.Row)
            {
                // Insertar al inicio de la columna
                nuevoNodo.Down = colCabecera.Access;
                colCabecera.Access.Up = nuevoNodo;
                colCabecera.Access = nuevoNodo;
            }
            else
            {
                MatrixNode actual = colCabecera.Access;
                while (actual.Down != null && actual.Down.Row < row)
                {
                    actual = actual.Down;
                }
                // Insertar en medio o al final de la columna
                nuevoNodo.Down = actual.Down;
                if (actual.Down != null)
                {
                    actual.Down.Up = nuevoNodo;
                }
                actual.Down = nuevoNodo;
                nuevoNodo.Up = actual;
            }

            nodeCount++;
        }

        // --- Operación de Eliminación ---
        public void Delete(int row, int col)
        {
            HeaderNode? filaCabecera = FindRowHeader(row);
            HeaderNode? colCabecera = FindColHeader(col);

            if (filaCabecera == null || colCabecera == null) return;

            MatrixNode? target = Search(row, col);
            if (target == null) return;

            // 1. Desconectar horizontalmente
            if (filaCabecera.Access == target)
            {
                filaCabecera.Access = target.Right;
                if (filaCabecera.Access != null)
                {
                    filaCabecera.Access.Left = null;
                }
            }
            else
            {
                if (target.Left != null)
                {
                    target.Left.Right = target.Right;
                }
                if (target.Right != null)
                {
                    target.Right.Left = target.Left;
                }
            }

            // 2. Desconectar verticalmente
            if (colCabecera.Access == target)
            {
                colCabecera.Access = target.Down;
                if (colCabecera.Access != null)
                {
                    colCabecera.Access.Up = null;
                }
            }
            else
            {
                if (target.Up != null)
                {
                    target.Up.Down = target.Down;
                }
                if (target.Down != null)
                {
                    target.Down.Up = target.Up;
                }
            }

            // 3. Limpiar cabeceras si quedan vacías
            RemoveRowHeaderIfEmpty(filaCabecera);
            RemoveColHeaderIfEmpty(colCabecera);

            nodeCount--;
        }

        // --- Operación de Búsqueda ---
        public MatrixNode? Search(int row, int col)
        {
            HeaderNode? filaCabecera = FindRowHeader(row);
            if (filaCabecera == null) return null;

            MatrixNode? actual = filaCabecera.Access;
            while (actual != null)
            {
                if (actual.Col == col)
                {
                    return actual;
                }
                actual = actual.Right;
            }
            return null;
        }

        public MatrixNode? BuscarPorId(string id)
        {
            HeaderNode? rowHeader = rowsHead;
            while (rowHeader != null)
            {
                MatrixNode? current = rowHeader.Access;
                while (current != null)
                {
                    if (current.Id.Equals(id, StringComparison.OrdinalIgnoreCase))
                    {
                        return current;
                    }
                    current = current.Right;
                }
                rowHeader = rowHeader.Next;
            }
            return null;
        }

        // --- Helpers de Cabeceras ---
        private HeaderNode? FindRowHeader(int row)
        {
            HeaderNode? actual = rowsHead;
            while (actual != null)
            {
                if (actual.Index == row) return actual;
                actual = actual.Next;
            }
            return null;
        }

        private HeaderNode? FindColHeader(int col)
        {
            HeaderNode? actual = colsHead;
            while (actual != null)
            {
                if (actual.Index == col) return actual;
                actual = actual.Next;
            }
            return null;
        }

        private HeaderNode GetOrCreateRowHeader(int row)
        {
            if (rowsHead == null)
            {
                rowsHead = new HeaderNode(row);
                return rowsHead;
            }

            if (row < rowsHead.Index)
            {
                HeaderNode nuevo = new HeaderNode(row) { Next = rowsHead };
                rowsHead = nuevo;
                return nuevo;
            }

            HeaderNode actual = rowsHead;
            while (actual.Next != null && actual.Next.Index < row)
            {
                actual = actual.Next;
            }

            if (actual.Next != null && actual.Next.Index == row)
            {
                return actual.Next;
            }

            if (actual.Index == row)
            {
                return actual;
            }

            HeaderNode nuevoNodo = new HeaderNode(row) { Next = actual.Next };
            actual.Next = nuevoNodo;
            return nuevoNodo;
        }

        private HeaderNode GetOrCreateColHeader(int col)
        {
            if (colsHead == null)
            {
                colsHead = new HeaderNode(col);
                return colsHead;
            }

            if (col < colsHead.Index)
            {
                HeaderNode nuevo = new HeaderNode(col) { Next = colsHead };
                colsHead = nuevo;
                return nuevo;
            }

            HeaderNode actual = colsHead;
            while (actual.Next != null && actual.Next.Index < col)
            {
                actual = actual.Next;
            }

            if (actual.Next != null && actual.Next.Index == col)
            {
                return actual.Next;
            }

            if (actual.Index == col)
            {
                return actual;
            }

            HeaderNode nuevoNodo = new HeaderNode(col) { Next = actual.Next };
            actual.Next = nuevoNodo;
            return nuevoNodo;
        }

        private void RemoveRowHeaderIfEmpty(HeaderNode rowHeader)
        {
            if (rowHeader.Access != null) return;

            if (rowsHead == rowHeader)
            {
                rowsHead = rowHeader.Next;
                return;
            }

            HeaderNode? actual = rowsHead;
            while (actual != null && actual.Next != rowHeader)
            {
                actual = actual.Next;
            }

            if (actual != null)
            {
                actual.Next = rowHeader.Next;
            }
        }

        private void RemoveColHeaderIfEmpty(HeaderNode colHeader)
        {
            if (colHeader.Access != null) return;

            if (colsHead == colHeader)
            {
                colsHead = colHeader.Next;
                return;
            }

            HeaderNode? actual = colsHead;
            while (actual != null && actual.Next != colHeader)
            {
                actual = actual.Next;
            }

            if (actual != null)
            {
                actual.Next = colHeader.Next;
            }
        }

        // --- Exportación en Arreglos Nativos (Sin usar System.Collections.Generic) ---
        public int[] ObtenerFilas()
        {
            int count = 0;
            HeaderNode? temp = rowsHead;
            while (temp != null)
            {
                count++;
                temp = temp.Next;
            }

            int[] arr = new int[count];
            temp = rowsHead;
            for (int i = 0; i < count; i++)
            {
                arr[i] = temp!.Index;
                temp = temp.Next;
            }
            return arr;
        }

        public int[] ObtenerColumnas()
        {
            int count = 0;
            HeaderNode? temp = colsHead;
            while (temp != null)
            {
                count++;
                temp = temp.Next;
            }

            int[] arr = new int[count];
            temp = colsHead;
            for (int i = 0; i < count; i++)
            {
                arr[i] = temp!.Index;
                temp = temp.Next;
            }
            return arr;
        }

        public MatrixNode[] ObtenerTodosLosNodos()
        {
            MatrixNode[] arr = new MatrixNode[nodeCount];
            int idx = 0;
            HeaderNode? rowHeader = rowsHead;
            while (rowHeader != null)
            {
                MatrixNode? current = rowHeader.Access;
                while (current != null)
                {
                    if (idx < nodeCount)
                    {
                        arr[idx++] = current;
                    }
                    current = current.Right;
                }
                rowHeader = rowHeader.Next;
            }
            return arr;
        }

        // --- Helpers de Resaltado de Ruta ---
        private bool EstaEnRuta(MatrixNode nodo, MatrixNode[]? ruta)
        {
            if (ruta == null) return false;
            for (int i = 0; i < ruta.Length; i++)
            {
                if (ruta[i] == nodo) return true;
            }
            return false;
        }

        private bool EsBordeDeRuta(MatrixNode n1, MatrixNode n2, MatrixNode[]? ruta)
        {
            if (ruta == null) return false;
            for (int i = 0; i < ruta.Length - 1; i++)
            {
                if ((ruta[i] == n1 && ruta[i + 1] == n2) || (ruta[i] == n2 && ruta[i + 1] == n1))
                {
                    return true;
                }
            }
            return false;
        }

        // --- Generación del Código DOT para Graphviz (Física de Memoria en HTML-like Records) ---
        public string GenerarCodigoDot(MatrixNode[]? ruta = null)
        {
            StringBuilder dot = new StringBuilder();
            dot.AppendLine("digraph G {");
            dot.AppendLine("    rankdir=TB;");
            dot.AppendLine("    node [fontname=\"Courier New\", fontsize=9, shape=none];");
            dot.AppendLine("    edge [fontname=\"Courier New\", fontsize=8];");
            dot.AppendLine("    bg [style=invisible];");

            // Nodo de entrada raíz de la matriz
            dot.AppendLine("    root [label=<");
            dot.AppendLine("        <TABLE BORDER=\"0\" CELLBORDER=\"1\" CELLSPACING=\"0\" BGCOLOR=\"#EBF5FB\">");
            dot.AppendLine("            <TR><TD COLSPAN=\"2\"><B>Raíz Matriz</B></TD></TR>");
            dot.AppendLine("            <TR><TD PORT=\"rows\">Filas</TD><TD PORT=\"cols\">Columnas</TD></TR>");
            dot.AppendLine("        </TABLE>");
            dot.AppendLine("    >];");

            // 1. Declarar Cabeceras de Filas
            HeaderNode? rowNode = rowsHead;
            while (rowNode != null)
            {
                dot.AppendLine($"    row_{rowNode.Index} [label=<");
                dot.AppendLine($"        <TABLE BORDER=\"0\" CELLBORDER=\"1\" CELLSPACING=\"0\" BGCOLOR=\"#FADBD8\">");
                dot.AppendLine($"            <TR><TD COLSPAN=\"2\"><B>Fila: {rowNode.Index}</B></TD></TR>");
                dot.AppendLine($"            <TR><TD PORT=\"next\">Sig</TD><TD PORT=\"access\">Acceso</TD></TR>");
                dot.AppendLine($"        </TABLE>");
                dot.AppendLine($"    >];");
                rowNode = rowNode.Next;
            }

            // 2. Declarar Cabeceras de Columnas
            HeaderNode? colNode = colsHead;
            while (colNode != null)
            {
                dot.AppendLine($"    col_{colNode.Index} [label=<");
                dot.AppendLine($"        <TABLE BORDER=\"0\" CELLBORDER=\"1\" CELLSPACING=\"0\" BGCOLOR=\"#FCF3CF\">");
                dot.AppendLine($"            <TR><TD COLSPAN=\"2\"><B>Col: {colNode.Index}</B></TD></TR>");
                dot.AppendLine($"            <TR><TD PORT=\"next\">Sig</TD><TD PORT=\"access\">Acceso</TD></TR>");
                dot.AppendLine($"        </TABLE>");
                dot.AppendLine($"    >];");
                colNode = colNode.Next;
            }

            // 3. Declarar Nodos de Datos (HTML-like tables showing: up, down, left, right pointers and values)
            rowNode = rowsHead;
            while (rowNode != null)
            {
                MatrixNode? node = rowNode.Access;
                while (node != null)
                {
                    // Si el nodo forma parte del enrutamiento trazado, cambiar fondo a verde brillante
                    string bgColor = EstaEnRuta(node, ruta) ? "#2ECC71" : "#D5F5E3";
                    dot.AppendLine($"    node_{node.Row}_{node.Col} [label=<");
                    dot.AppendLine($"        <TABLE BORDER=\"0\" CELLBORDER=\"1\" CELLSPACING=\"0\" BGCOLOR=\"{bgColor}\">");
                    dot.AppendLine($"            <TR><TD PORT=\"up\">Up</TD><TD PORT=\"down\">Down</TD></TR>");
                    dot.AppendLine($"            <TR><TD COLSPAN=\"2\"><B>Row: {node.Row}<BR/>Col: {node.Col}<BR/>ID: {node.Id}<BR/>IP: {node.IpAddress}</B></TD></TR>");
                    dot.AppendLine($"            <TR><TD PORT=\"left\">Left</TD><TD PORT=\"right\">Right</TD></TR>");
                    dot.AppendLine($"        </TABLE>");
                    dot.AppendLine($"    >];");
                    node = node.Right;
                }
                rowNode = rowNode.Next;
            }

            // 4. Conectar Cabeceras de Filas
            if (rowsHead != null)
            {
                dot.AppendLine("    root:rows -> row_" + rowsHead.Index + ";");
                rowNode = rowsHead;
                while (rowNode.Next != null)
                {
                    dot.AppendLine($"    row_{rowNode.Index}:next -> row_{rowNode.Next.Index};");
                    rowNode = rowNode.Next;
                }
            }

            // 5. Conectar Cabeceras de Columnas
            if (colsHead != null)
            {
                dot.AppendLine("    root:cols -> col_" + colsHead.Index + ";");
                colNode = colsHead;
                while (colNode.Next != null)
                {
                    dot.AppendLine($"    col_{colNode.Index}:next -> col_{colNode.Next.Index};");
                    colNode = colNode.Next;
                }
            }

            // 6. Conectar Nodos de Datos de Forma Ortogonal (Up/Down y Left/Right)
            rowNode = rowsHead;
            while (rowNode != null)
            {
                if (rowNode.Access != null)
                {
                    // Cabecera de fila apunta al primer nodo
                    dot.AppendLine($"    row_{rowNode.Index}:access -> node_{rowNode.Access.Row}_{rowNode.Access.Col}:left;");
                    
                    MatrixNode? node = rowNode.Access;
                    while (node != null)
                    {
                        // Enlace derecho
                        if (node.Right != null)
                        {
                            bool esRuta = EsBordeDeRuta(node, node.Right, ruta);
                            string color = esRuta ? "#27AE60" : "blue";
                            string styleOpts = esRuta ? ", penwidth=3.0" : "";
                            dot.AppendLine($"    node_{node.Row}_{node.Col}:right -> node_{node.Right.Row}_{node.Right.Col}:left [dir=both, color=\"{color}\"{styleOpts}];");
                        }
                        node = node.Right;
                    }
                }
                rowNode = rowNode.Next;
            }

            colNode = colsHead;
            while (colNode != null)
            {
                if (colNode.Access != null)
                {
                    // Cabecera de columna apunta al primer nodo
                    dot.AppendLine($"    col_{colNode.Index}:access -> node_{colNode.Access.Row}_{colNode.Access.Col}:up;");
                    
                    MatrixNode? node = colNode.Access;
                    while (node != null)
                    {
                        // Enlace hacia abajo
                        if (node.Down != null)
                        {
                            bool esRuta = EsBordeDeRuta(node, node.Down, ruta);
                            string color = esRuta ? "#27AE60" : "red";
                            string styleOpts = esRuta ? ", penwidth=3.0" : "";
                            dot.AppendLine($"    node_{node.Row}_{node.Col}:down -> node_{node.Down.Row}_{node.Down.Col}:up [dir=both, color=\"{color}\"{styleOpts}];");
                        }
                        node = node.Down;
                    }
                }
                colNode = colNode.Next;
            }

            // 7. Alinear Cabeceras Horizontales (rank=same para root y cabeceras de columnas)
            if (colsHead != null)
            {
                dot.Append("    { rank=same; root; ");
                colNode = colsHead;
                while (colNode != null)
                {
                    dot.Append($"col_{colNode.Index}; ");
                    colNode = colNode.Next;
                }
                dot.AppendLine("}");
            }

            // 8. Alinear Filas Horizontales (rank=same para cada fila cabecera y sus nodos)
            rowNode = rowsHead;
            while (rowNode != null)
            {
                dot.Append($"    {{ rank=same; row_{rowNode.Index}; ");
                MatrixNode? node = rowNode.Access;
                while (node != null)
                {
                    dot.Append($"node_{node.Row}_{node.Col}; ");
                    node = node.Right;
                }
                dot.AppendLine("}");
                rowNode = rowNode.Next;
            }

            dot.AppendLine("}");
            return dot.ToString();
        }
    }
}





