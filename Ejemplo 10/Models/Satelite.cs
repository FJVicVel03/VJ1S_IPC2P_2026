using System;

namespace Ejemplo_10.Models
{
    /// <summary>
    /// Representa un satélite en el catálogo de telemetría.
    /// </summary>
    public class Satelite
    {
        private string id;
        private string nombre = "";
        private string enlaceIP = "";

        public Satelite(string id, string nombre, string enlaceIp)
        {
            this.id = id;
            Nombre = nombre;
            EnlaceIP = enlaceIp;
        }

        public string Id
        {
            get { return id; }
        }

        public string Nombre
        {
            get { return nombre; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("El nombre del satélite no puede estar vacío.");
                }
                nombre = value;
            }
        }

        public string EnlaceIP
        {
            get { return enlaceIP; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("La dirección IP no puede estar vacía.");
                }
                if (!value.Contains("."))
                {
                    throw new ArgumentException("La dirección IP debe tener un formato IPv4 válido.");
                }
                enlaceIP = value;
            }
        }

        public string ObtenerDescripcion()
        {
            return $"Satélite: {Nombre} (ID: {Id}) -> IP: {EnlaceIP}";
        }
    }
}



