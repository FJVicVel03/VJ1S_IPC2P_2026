using System;

namespace Ejemplo2
{
    /// <summary>
    /// Representa un satélite en la red de telemetría.
    /// Implementa encapsulamiento mediante campos privados y propiedades con validación.
    /// </summary>
    public class Satelite
    {
        // Campos privados (inaccesibles directamente desde fuera de la clase)
        private string id;
        private string nombre = "";
        private string enlaceIP = "";

        /// <summary>
        /// Constructor para inicializar un objeto de la clase Satelite.
        /// </summary>
        public Satelite(string id, string nombre, string enlaceIp)
        {
            this.id = id;
            
            // Usamos las propiedades en el constructor para forzar las validaciones desde el inicio
            Nombre = nombre;
            EnlaceIP = enlaceIp;
        }

        // --- Propiedades con Encapsulamiento y Validación ---

        /// <summary>
        /// Propiedad de sólo lectura para el identificador.
        /// El ID se asigna en el constructor y no puede ser alterado externamente.
        /// </summary>
        public string Id
        {
            get { return id; }
        }

        /// <summary>
        /// Propiedad para leer y escribir el nombre, con validación de cadenas vacías.
        /// </summary>
        public string Nombre
        {
            get { return nombre; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("El nombre del satélite no puede estar vacío o contener solo espacios.");
                }
                nombre = value;
            }
        }

        /// <summary>
        /// Propiedad para leer y escribir la dirección IP, con validación básica de formato.
        /// </summary>
        public string EnlaceIP
        {
            get { return enlaceIP; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("La dirección de red IP no puede estar vacía.");
                }
                // Validación básica preliminar: debe contener puntos para simular un formato IPv4
                if (!value.Contains("."))
                {
                    throw new ArgumentException($"La IP '{value}' es inválida. Debe contener formato IPv4 (ej. 192.168.1.1).");
                }
                enlaceIP = value;
            }
        }

        /// <summary>
        /// Retorna la cadena detallada con el estado actual del satélite.
        /// </summary>
        public string ObtenerDescripcion()
        {
            return $"Satélite: {Nombre} (ID: {Id}) -> Conectado a la IP: {EnlaceIP}";
        }
    }
}
