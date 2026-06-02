using System;

namespace Ejemplo2
{
    /// <summary>
    /// Representa un registro de auditoría en la bitácora del simulador.
    /// Introduce la encapsulación con propiedades automáticas de sólo lectura.
    /// </summary>
    public class LogRegistro
    {
        // Propiedades de sólo lectura (se asignan únicamente en el constructor, inmutables desde fuera)
        public string Timestamp { get; }
        public string Tipo { get; } // "INFO" o "ERROR"
        public string Mensaje { get; }

        /// <summary>
        /// Constructor para instanciar un nuevo registro de log.
        /// </summary>
        public LogRegistro(string tipo, string mensaje)
        {
            Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            Tipo = tipo;
            Mensaje = mensaje;
        }

        /// <summary>
        /// Retorna una línea formateada de auditoría para su impresión en consola.
        /// </summary>
        public string ObtenerLineaFormateada()
        {
            string tag = Tipo == "INFO" ? "OK" : "FAIL";
            return $"{Timestamp} | {Tipo,-5} ({tag}) | {Mensaje}";
        }
    }
}
