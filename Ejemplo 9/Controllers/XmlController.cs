using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ejemplo_9.Models;

namespace Ejemplo_9.Controllers
{
    /// <summary>
    /// Controlador especializado en el procesamiento e ingesta transaccional de archivos XML.
    /// </summary>
    public class XmlController : Controller
    {
        // Expresiones Regulares Oficiales del Proyecto
        private const string PatronIdSatelite = @"^SAT-(ECU|POL)-\d{4}$";
        private const string PatronIpv4 = @"^(?:(?:25[0-5]|2[0-4]\d|[01]?\d?\d)\.){3}(?:25[0-5]|2[0-4]\d|[01]?\d?\d)$";

        // Estructura lineal temporal para atomicidad de transacciones sin genéricos
        private class SateliteTemporal
        {
            public int Fila { get; }
            public int Columna { get; }
            public string Id { get; }
            public string Nombre { get; }
            public string IpAddress { get; }
            public SateliteTemporal? Siguiente { get; set; }

            public SateliteTemporal(int fila, int col, string id, string nombre, string ip)
            {
                Fila = fila;
                Columna = col;
                Id = id;
                Nombre = nombre;
                IpAddress = ip;
                Siguiente = null;
            }
        }

        [HttpPost]
        public IActionResult CargarXml(IFormFile archivoXml)
        {
            if (archivoXml == null || archivoXml.Length == 0)
            {
                string msgErr = "Por favor, seleccione un archivo XML válido.";
                MemoriaPlano.Logs.Registrar("ALERT", msgErr);
                TempData["ErrorMessage"] = msgErr;
                return RedirectToAction("Index", "Home");
            }

            MemoriaPlano.Logs.Registrar("INFO", $"Iniciando carga de archivo: '{archivoXml.FileName}'");

            SateliteTemporal? cabezaTemporal = null;
            int contadorTemporal = 0;
            bool transaccionExitosa = true;
            string causaFallo = "";

            try
            {
                // OWASP: Mitigar XXE deshabilitando la resolución DTD externa
                XmlReaderSettings settings = new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Prohibit,
                    XmlResolver = null
                };

                using (Stream stream = archivoXml.OpenReadStream())
                using (XmlReader reader = XmlReader.Create(stream, settings))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(reader);

                    // 1. Cargar satélites ecuatoriales (se mapean a Fila = 0)
                    XmlNodeList satEcuatoriales = doc.SelectNodes("//constelaciones_ecuatoriales/satelite")!;
                    foreach (XmlNode sat in satEcuatoriales)
                    {
                        string? id = sat.Attributes?["id"]?.Value;
                        string? nombre = sat.SelectSingleNode("nombre")?.InnerText;
                        string? enlaceIp = sat.SelectSingleNode("enlace_ip")?.InnerText;

                        if (!ValidarYAgregarTemporal(0, id, nombre, enlaceIp, ref cabezaTemporal, ref contadorTemporal, ref causaFallo))
                        {
                            transaccionExitosa = false;
                            break;
                        }
                    }

                    // 2. Cargar satélites polares (se mapean a Fila = 1) si la transacción sigue exitosa
                    if (transaccionExitosa)
                    {
                        XmlNodeList satPolares = doc.SelectNodes("//constelaciones_polares/satelite")!;
                        foreach (XmlNode sat in satPolares)
                        {
                            string? id = sat.Attributes?["id"]?.Value;
                            string? nombre = sat.SelectSingleNode("nombre")?.InnerText;
                            string? enlaceIp = sat.SelectSingleNode("enlace_ip")?.InnerText;

                            if (!ValidarYAgregarTemporal(1, id, nombre, enlaceIp, ref cabezaTemporal, ref contadorTemporal, ref causaFallo))
                            {
                                transaccionExitosa = false;
                                break;
                            }
                        }
                    }
                }
            }
            catch (XmlException ex)
            {
                transaccionExitosa = false;
                causaFallo = $"Error de parseo XML: {ex.Message}";
            }
            catch (Exception ex)
            {
                transaccionExitosa = false;
                causaFallo = $"Error de procesamiento: {ex.Message}";
            }

            // Consolidación transaccional (Commit o Rollback)
            if (transaccionExitosa && cabezaTemporal != null)
            {
                // COMMIT: Insertar todos los nodos acumulados a la Matriz Dispersa principal en MemoriaPlano
                SateliteTemporal? actual = cabezaTemporal;
                while (actual != null)
                {
                    MemoriaPlano.Matriz.Insert(actual.Fila, actual.Columna, actual.Id, actual.Nombre, actual.IpAddress);
                    actual = actual.Siguiente;
                }

                string msgSucc = $"Carga masiva completada con éxito (Commit). Se insertaron {contadorTemporal} nodos en la Matriz Dispersa.";
                MemoriaPlano.Logs.Registrar("INFO", msgSucc);
                TempData["SuccessMessage"] = msgSucc;
            }
            else
            {
                // ROLLBACK: No se altera la matriz
                string msgFallo = $"Transacción de carga abortada (Rollback). Causa: {causaFallo}. La matriz permanece intacta.";
                MemoriaPlano.Logs.Registrar("ERROR", msgFallo);
                TempData["ErrorMessage"] = msgFallo;
            }

            return RedirectToAction("Index", "Home");
        }

        private bool ValidarYAgregarTemporal(int fila, string? id, string? nombre, string? enlaceIp, ref SateliteTemporal? cabeza, ref int contador, ref string causaFallo)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(enlaceIp))
            {
                causaFallo = "Nodo satélite con información o atributos incompletos en el archivo XML.";
                return false;
            }

            id = id.Trim();
            nombre = nombre.Trim();
            enlaceIp = enlaceIp.Trim();

            // 1. Validar ID con Regex
            if (!Regex.IsMatch(id, PatronIdSatelite))
            {
                causaFallo = $"El satélite con ID '{id}' no cumple con el patrón sintáctico requerido 'SAT-(ECU|POL)-0000'.";
                return false;
            }

            // 2. Validar IP con Regex
            if (!Regex.IsMatch(enlaceIp, PatronIpv4))
            {
                causaFallo = $"La dirección IP '{enlaceIp}' asignada al satélite [{id}] es inválida.";
                return false;
            }

            // Determinar columna (obteniendo los últimos 4 dígitos numéricos del ID)
            int columna;
            try
            {
                columna = int.Parse(id.Substring(8));
            }
            catch
            {
                causaFallo = $"No se pudo derivar el índice de columna a partir del ID '{id}'.";
                return false;
            }

            // 3. Comprobar duplicidad de coordenadas en la matriz principal
            if (MemoriaPlano.Matriz.Search(fila, columna) != null)
            {
                causaFallo = $"Colisión de coordenadas: ya existe un nodo físico en la posición ({fila}, {columna}).";
                return false;
            }

            // 4. Comprobar duplicidad de ID en la matriz principal
            if (MemoriaPlano.Matriz.BuscarPorId(id) != null)
            {
                causaFallo = $"El identificador de satélite '{id}' ya se encuentra registrado en el plano espacial.";
                return false;
            }

            // 5. Comprobar duplicidad de coordenadas e ID en la lista temporal de la carga activa
            SateliteTemporal? actual = cabeza;
            while (actual != null)
            {
                if (actual.Fila == fila && actual.Columna == columna)
                {
                    causaFallo = $"Colisión de coordenadas interna en XML: posición ({fila}, {columna}) declarada varias veces.";
                    return false;
                }
                if (actual.Id.Equals(id, StringComparison.OrdinalIgnoreCase))
                {
                    causaFallo = $"ID de satélite duplicado internamente en XML: '{id}'.";
                    return false;
                }
                actual = actual.Siguiente;
            }

            // Agregar a la lista temporal
            SateliteTemporal nuevo = new SateliteTemporal(fila, columna, id, nombre, enlaceIp);
            if (cabeza == null)
            {
                cabeza = nuevo;
            }
            else
            {
                SateliteTemporal temp = cabeza;
                while (temp.Siguiente != null)
                {
                    temp = temp.Siguiente;
                }
                temp.Siguiente = nuevo;
            }
            contador++;
            return true;
        }
    }
}

