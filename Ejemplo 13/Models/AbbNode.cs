using System;

namespace Ejemplo_13.Models
{
    /// <summary>
    /// Representa un nodo individual dentro del Árbol Binario de Búsqueda (ABB).
    /// Este nodo actúa como el contenedor físico de un paquete de datos/mensaje
    /// transitando por el simulador, ordenado por su nivel de prioridad.
    /// No utiliza colecciones genéricas de .NET.
    /// </summary>
    public class AbbNode
    {
        /// <summary>
        /// Código hexadecimal único que identifica unívocamente al paquete.
        /// </summary>
        public string HexCode { get; set; } = "";

        /// <summary>
        /// Identificador único del satélite de origen que emitió el paquete.
        /// </summary>
        public string EmisorId { get; set; } = "";

        /// <summary>
        /// Dirección IP del nodo terrestre de destino final.
        /// </summary>
        public string DestIp { get; set; } = "";

        /// <summary>
        /// Nivel de prioridad asignado al paquete de datos.
        /// Valores enteros admitidos de 1 (Mínima prioridad) a 5 (Alerta Crítica / Máxima prioridad).
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Cuerpo o contenido de texto plano del mensaje transmitido.
        /// </summary>
        public string Content { get; set; } = "";

        /// <summary>
        /// Referencia al hijo izquierdo (subárbol con prioridades estrictamente menores).
        /// </summary>
        public AbbNode? Left { get; set; }

        /// <summary>
        /// Referencia al hijo derecho (subárbol con prioridades mayores o iguales).
        /// </summary>
        public AbbNode? Right { get; set; }

        /// <summary>
        /// Constructor principal para inicializar un paquete de datos en el Heap.
        /// </summary>
        public AbbNode(string hexCode, string emisorId, string destIp, int priority, string content)
        {
            HexCode = hexCode;
            EmisorId = emisorId;
            DestIp = destIp;
            Priority = priority;
            Content = content;
            Left = null;
            Right = null;
        }
    }
}
