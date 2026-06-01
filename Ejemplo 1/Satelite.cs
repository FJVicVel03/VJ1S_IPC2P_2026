namespace Ejemplo1
{
    /// <summary>
    /// Representa un satélite básico de la red de telemetría.
    /// Este primer ejemplo introduce los conceptos de Clase, Objeto, Constructor y Campos Públicos en C#.
    /// </summary>
    public class Satelite
    {
        // Campos públicos (accesibles y modificables directamente desde fuera de la clase)
        public string Id;
        public string Nombre;
        public string EnlaceIp;

        /// <summary>
        /// Constructor para inicializar una nueva instancia de la clase Satelite.
        /// </summary>
        public Satelite(string id, string nombre, string enlaceIp)
        {
            Id = id;
            Nombre = nombre;
            EnlaceIp = enlaceIp;
        }

        /// <summary>
        /// Retorna una cadena detallada con el estado actual del satélite.
        /// </summary>
        public string ObtenerDescripcion()
        {
            return $"Satélite: {Nombre} (ID: {Id}) -> Conectado a la IP: {EnlaceIp}";
        }
    }
}
