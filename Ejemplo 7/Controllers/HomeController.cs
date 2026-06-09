using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ejemplo_7.Models;
using Ejemplo_7.Services;

namespace Ejemplo_7.Controllers
{
    /// <summary>
    /// Controlador principal para orquestar la simulación de la Matriz Dispersa y la bitácora de logs.
    /// </summary>
    public class HomeController : Controller
    {
        // Persistencia estática en memoria RAM (mantenida activa durante la ejecución del proceso)
        private static readonly RedSatelitalPlano baseDatosMatriz = new RedSatelitalPlano();
        private static readonly ListaLogs bitacoraAuditoria = new ListaLogs();

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

        [HttpGet]
        public IActionResult Index()
        {
            if (bitacoraAuditoria.EstaVacia)
            {
                bitacoraAuditoria.Registrar("INFO", "Sistema de visualización de plano satelital activo.");
                bitacoraAuditoria.Registrar("INFO", "TDA Matriz Dispersa Ortogonal inicializada en memoria RAM.");
            }

            return View(ObtenerViewModel());
        }

        [HttpPost]
        public IActionResult CargarXml(IFormFile archivoXml)
        {
            if (archivoXml == null || archivoXml.Length == 0)
            {
                string msgErr = "Por favor, seleccione un archivo XML válido.";
                bitacoraAuditoria.Registrar("ALERT", msgErr);
                ViewBag.ErrorMessage = msgErr;
                return View("Index", ObtenerViewModel());
            }

            bitacoraAuditoria.Registrar("INFO", $"Iniciando carga de archivo: '{archivoXml.FileName}'");

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
                // COMMIT: Insertar todos los nodos acumulados a la Matriz Dispersa principal
                SateliteTemporal? actual = cabezaTemporal;
                while (actual != null)
                {
                    baseDatosMatriz.Insert(actual.Fila, actual.Columna, actual.Id, actual.Nombre, actual.IpAddress);
                    actual = actual.Siguiente;
                }

                string msgSucc = $"Carga masiva completada con éxito (Commit). Se insertaron {contadorTemporal} nodos en la Matriz Dispersa.";
                bitacoraAuditoria.Registrar("INFO", msgSucc);
                ViewBag.SuccessMessage = msgSucc;
            }
            else
            {
                // ROLLBACK: No se altera la matriz
                string msgFallo = $"Transacción de carga abortada (Rollback). Causa: {causaFallo}. La matriz permanece intacta.";
                bitacoraAuditoria.Registrar("ERROR", msgFallo);
                ViewBag.ErrorMessage = msgFallo;
            }

            return View("Index", ObtenerViewModel());
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
            if (baseDatosMatriz.Search(fila, columna) != null)
            {
                causaFallo = $"Colisión de coordenadas: ya existe un nodo físico en la posición ({fila}, {columna}).";
                return false;
            }

            // 4. Comprobar duplicidad de ID en la matriz principal
            if (baseDatosMatriz.BuscarPorId(id) != null)
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

        [HttpPost]
        public IActionResult InsertarNodo(int row, int col, string id, string nombre, string ip)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(ip))
            {
                string msgErr = "Error al insertar: Todos los campos del satélite son obligatorios.";
                bitacoraAuditoria.Registrar("ALERT", msgErr);
                ViewBag.ErrorMessage = msgErr;
                return View("Index", ObtenerViewModel());
            }

            id = id.Trim();
            nombre = nombre.Trim();
            ip = ip.Trim();

            if (row < 0 || col < 0)
            {
                string msgErr = "Las coordenadas del nodo deben ser números enteros no negativos.";
                bitacoraAuditoria.Registrar("ALERT", msgErr);
                ViewBag.ErrorMessage = msgErr;
                return View("Index", ObtenerViewModel());
            }

            // 1. Validar ID con Regex
            if (!Regex.IsMatch(id, PatronIdSatelite))
            {
                string msgErr = $"Error sintáctico en ID '{id}': Debe cumplir con el formato 'SAT-(ECU|POL)-0000'.";
                bitacoraAuditoria.Registrar("ALERT", msgErr);
                ViewBag.ErrorMessage = msgErr;
                return View("Index", ObtenerViewModel());
            }

            // 2. Validar IP con Regex
            if (!Regex.IsMatch(ip, PatronIpv4))
            {
                string msgErr = $"Error sintáctico en IP '{ip}': Debe ser una dirección IPv4 válida.";
                bitacoraAuditoria.Registrar("ALERT", msgErr);
                ViewBag.ErrorMessage = msgErr;
                return View("Index", ObtenerViewModel());
            }

            // 3. Validar Colisión de Coordenadas
            if (baseDatosMatriz.Search(row, col) != null)
            {
                string msgErr = $"Colisión detectada: ya existe un nodo en las coordenadas ({row}, {col}).";
                bitacoraAuditoria.Registrar("ERROR", msgErr);
                ViewBag.ErrorMessage = msgErr;
                return View("Index", ObtenerViewModel());
            }

            // 4. Validar Identificador Duplicado
            if (baseDatosMatriz.BuscarPorId(id) != null)
            {
                string msgErr = $"El identificador de satélite '{id}' ya existe en el plano.";
                bitacoraAuditoria.Registrar("ERROR", msgErr);
                ViewBag.ErrorMessage = msgErr;
                return View("Index", ObtenerViewModel());
            }

            try
            {
                baseDatosMatriz.Insert(row, col, id, nombre, ip);
                string msgSucc = $"Nodo satelital [{id}] insertado con éxito en ({row}, {col}). Enlaces ortogonales actualizados.";
                bitacoraAuditoria.Registrar("INFO", msgSucc);
                ViewBag.SuccessMessage = msgSucc;
            }
            catch (Exception ex)
            {
                string msgErr = $"Error de inserción física: {ex.Message}";
                bitacoraAuditoria.Registrar("ERROR", msgErr);
                ViewBag.ErrorMessage = msgErr;
            }

            return View("Index", ObtenerViewModel());
        }

        [HttpPost]
        public IActionResult EliminarNodo(int row, int col)
        {
            if (row < 0 || col < 0)
            {
                string msgErr = "Las coordenadas a eliminar deben ser no negativas.";
                bitacoraAuditoria.Registrar("ALERT", msgErr);
                ViewBag.ErrorMessage = msgErr;
                return View("Index", ObtenerViewModel());
            }

            MatrixNode? target = baseDatosMatriz.Search(row, col);
            if (target == null)
            {
                string msgErr = $"No existe ningún nodo en la coordenada ({row}, {col}) para eliminar.";
                bitacoraAuditoria.Registrar("ALERT", msgErr);
                ViewBag.ErrorMessage = msgErr;
                return View("Index", ObtenerViewModel());
            }

            try
            {
                string idEliminado = target.Id;
                baseDatosMatriz.Delete(row, col);
                string msgSucc = $"Nodo [{idEliminado}] eliminado de ({row}, {col}). Enlaces vecinos reconectados.";
                bitacoraAuditoria.Registrar("INFO", msgSucc);
                ViewBag.SuccessMessage = msgSucc;
            }
            catch (Exception ex)
            {
                string msgErr = $"Error al eliminar nodo: {ex.Message}";
                bitacoraAuditoria.Registrar("ERROR", msgErr);
                ViewBag.ErrorMessage = msgErr;
            }

            return View("Index", ObtenerViewModel());
        }

        [HttpPost]
        public IActionResult LimpiarMatriz()
        {
            baseDatosMatriz.Clear();
            bitacoraAuditoria.Registrar("INFO", "Se purgaron todos los nodos de la Matriz Dispersa.");
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult LimpiarLogs()
        {
            bitacoraAuditoria.Limpiar();
            bitacoraAuditoria.Registrar("INFO", "Se purgó la bitácora de auditoría.");
            return RedirectToAction("Index");
        }

        private DashboardViewModel ObtenerViewModel()
        {
            // 1. Generar código fuente DOT a partir de las referencias de memoria de la Matriz
            string codigoDot = baseDatosMatriz.GenerarCodigoDot();

            // 2. Compilar en caliente a SVG vectorial utilizando redirección asíncrona de subprocesos
            string svgOutput = GraphvizCompilador.CompilarDotASvg(codigoDot);

            return new DashboardViewModel
            {
                Matriz = baseDatosMatriz,
                Logs = bitacoraAuditoria,
                SvgDiagrama = svgOutput
            };
        }
    }
}
