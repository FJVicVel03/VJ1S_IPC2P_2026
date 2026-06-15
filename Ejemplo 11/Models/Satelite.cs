using System;

namespace Ejemplo_11.Models
{
    /// <summary>
    /// Representa un satélite en el catálogo de telemetría.
    /// </summary>
    public class Satelite
    {
        private string id;
        private string nombre = "";
        private string enlaceIP = "";
        private double frecuencia = 0.0;

        public Satelite(string id, string nombre, string enlaceIp)
        {
            this.id = id;
            Nombre = nombre;
            EnlaceIP = enlaceIp;
            Frecuencia = 0.0;
        }

        public Satelite(string id, string nombre, double frecuencia)
        {
            this.id = id;
            Nombre = nombre;
            EnlaceIP = ""; // Polar satellites do not have an IP address in the configuration XML
            Frecuencia = frecuencia;
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
                    throw new ArgumentException("El nombre del satelite no puede estar vacio.");
                }
                nombre = value;
            }
        }

        public string EnlaceIP
        {
            get { return enlaceIP; }
            set
            {
                // Solo validar formato si la IP no esta en blanco (para dar soporte a satelites polares)
                if (!string.IsNullOrEmpty(value))
                {
                    if (!value.Contains("."))
                    {
                        throw new ArgumentException("La direccion IP debe tener un formato IPv4 valido.");
                    }
                }
                enlaceIP = value ?? "";
            }
        }

        public double Frecuencia
        {
            get { return frecuencia; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("La frecuencia de operacion no puede ser negativa.");
                }
                frecuencia = value;
            }
        }

        public string ObtenerDescripcion()
        {
            if (frecuencia > 0)
            {
                return $"Satelite: {Nombre} (ID: {Id}) -> Freq: {Frecuencia} MHz";
            }
            return $"Satelite: {Nombre} (ID: {Id}) -> IP: {EnlaceIP}";
        }
    }
}



