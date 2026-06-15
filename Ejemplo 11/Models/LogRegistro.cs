using System;

namespace Ejemplo_11.Models
{
    /// <summary>
    /// Representa una entrada individual de la bitácora de auditoría.
    /// </summary>
    public class LogRegistro
    {
        public string Timestamp { get; }
        public string Tipo { get; } // INFO, ALERT, ERROR
        public string Mensaje { get; }

        public LogRegistro(string tipo, string mensaje)
        {
            Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            Tipo = tipo;
            Mensaje = mensaje;
        }

        public string ObtenerLineaFormateada()
        {
            string tag = Tipo == "INFO" ? "OK" : (Tipo == "ALERT" ? "WARN" : "FAIL");
            return $"{Timestamp} | {Tipo,-5} ({tag}) | {Mensaje}";
        }
    }
}



