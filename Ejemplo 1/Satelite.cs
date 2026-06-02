namespace Ejemplo1
{
    /// <summary>
    /// Representa un satélite básico de la red de telemetría.
    /// Este primer ejemplo introduce los conceptos de Clase, Objeto, Constructor y Campos Públicos en C#.
    /// </summary>
    public class Satelite
    {
        // Campos públicos (accesibles y modificables directamente desde fuera de la clase)
        public string id;
        public string nombre;
        public string enlaceIP;

        /// <summary>
        /// Constructor para inicializar una nueva instancia de la clase Satelite.
        /// </summary>
        public Satelite(string id, string nombre, string enlaceIp)
        {
            this.id = id;
            this.nombre = nombre;
            this.enlaceIP = enlaceIp;
        }

        /// <summary>
        /// Retorna una cadena detallada con el estado actual del satélite.
        /// </summary>
        public string ObtenerDescripcion()
        {
            return $"Satélite: {nombre} (ID: {id}) -> Conectado a la IP: {enlaceIP}";
        }
    }
}
