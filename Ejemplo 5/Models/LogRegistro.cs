using System;

namespace Ejemplo_5.Models
{
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
            string tag = Tipo == "INFO" ? "OK" : "FAIL";
            return $"{Timestamp} | {Tipo,-5} ({tag}) | {Mensaje}";
        }
    }
}
