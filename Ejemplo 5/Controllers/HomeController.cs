using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ejemplo_5.Models;

namespace Ejemplo_5.Controllers
{
    /// <summary>
    /// Controlador que implementa la carga masiva transaccional atómica con validación RegEx.
    /// Utiliza XmlDocument para el parseo y ListaLogs (TDA LogAuditoria) para la bitácora.
    /// </summary>
    public class HomeController : Controller
    {
        // Persistencia estática en memoria RAM para simular bases de datos locales
        private static readonly ListaSatelites baseDatosSatelites = new ListaSatelites();
        private static readonly ListaLogs bitacoraAuditoria = new ListaLogs();

        // Expresiones Regulares Oficiales según el Enunciado del Proyecto
        private const string PatronIdSatelite = @"^SAT-(ECU|POL)-\d{4}$";
        private const string PatronIpv4 = @"^(?:(?:25[0-5]|2[0-4]\d|[01]?\d?\d)\.){3}(?:25[0-5]|2[0-4]\d|[01]?\d?\d)$";

        [HttpGet]
        public IActionResult Index()
        {
            // Inicialización de logs básicos de arranque si la bitácora está vacía
            if (bitacoraAuditoria.EstaVacia)
            {
                bitacoraAuditoria.Registrar("INFO", "Sistema de simulación espacial inicializado correctamente.");
                bitacoraAuditoria.Registrar("INFO", "Esperando archivo XML de configuración...");
            }

            var viewModel = new DashboardViewModel
            {
                Satelites = baseDatosSatelites,
                Logs = bitacoraAuditoria
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult CargarXml(IFormFile archivoXml)
        {
            var viewModel = new DashboardViewModel
            {
                Satelites = baseDatosSatelites,
                Logs = bitacoraAuditoria
            };

            if (archivoXml == null || archivoXml.Length == 0)
            {
                ViewBag.ErrorMessage = "Por favor, seleccione un archivo XML válido.";
                return View("Index", viewModel);
            }

            bitacoraAuditoria.Registrar("INFO", $"Iniciando carga de archivo: '{archivoXml.FileName}'");

            // 1. Instanciación del TDA Temporal para Carga Transaccional (Atómica)
            // Se acumularán los satélites validados aquí antes de pasarlos a la base de datos principal
            ListaSatelites listaTemporal = new ListaSatelites();
            bool transaccionExitosa = true;
            string causaFallo = "";

            try
            {
                // Mitigación de Vulnerabilidades XXE
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

                    // Selección de elementos mediante XPath
                    XmlNodeList satelitesNodos = doc.SelectNodes("/orbitnet/constelaciones_ecuatoriales/satelite")!;

                    if (satelitesNodos.Count == 0)
                    {
                        transaccionExitosa = false;
                        causaFallo = "No se encontraron nodos de satélites bajo el XPath '/orbitnet/constelaciones_ecuatoriales/satelite'.";
                    }
                    else
                    {
                        // 2. Procesamiento de los nodos con validaciones RegEx
                        foreach (XmlNode nodo in satelitesNodos)
                        {
                            string? id = nodo.Attributes?["id"]?.Value;
                            string? nombre = nodo.SelectSingleNode("nombre")?.InnerText;
                            string? enlaceIp = nodo.SelectSingleNode("enlace_ip")?.InnerText;

                            // Comprobamos la existencia física de campos
                            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(enlaceIp))
                            {
                                transaccionExitosa = false;
                                causaFallo = "Se encontró un satélite con datos faltantes en el XML.";
                                break;
                            }

                            id = id.Trim();
                            nombre = nombre.Trim();
                            enlaceIp = enlaceIp.Trim();

                            // --- VALIDACIÓN 1 REGEX: ID del Satélite ---
                            if (!Regex.IsMatch(id, PatronIdSatelite))
                            {
                                transaccionExitosa = false;
                                causaFallo = $"El satélite con ID '{id}' no cumple con el formato requerido 'SAT-(ECU|POL)-0000'.";
                                break;
                            }

                            // --- VALIDACIÓN 2 REGEX: Dirección IPv4 ---
                            if (!Regex.IsMatch(enlaceIp, PatronIpv4))
                            {
                                transaccionExitosa = false;
                                causaFallo = $"El satélite [{id}] contiene una dirección IP inválida: '{enlaceIp}'.";
                                break;
                            }

                            // Intentamos añadir a la lista temporal si pasa las validaciones de encapsulación
                            try
                            {
                                Satelite nuevoSatelite = new Satelite(id, nombre, enlaceIp);
                                listaTemporal.InsertarAlFinal(nuevoSatelite);
                            }
                            catch (ArgumentException ex)
                            {
                                transaccionExitosa = false;
                                causaFallo = $"Error de lógica en el modelo del satélite [{id}]: {ex.Message}";
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

            // 3. Resolución Transaccional (Commit / Rollback)
            if (transaccionExitosa)
            {
                // COMMIT: Todos los satélites son válidos, por lo tanto, se insertan en la base de datos principal
                foreach (Satelite sat in listaTemporal.Recorrer())
                {
                    baseDatosSatelites.InsertarAlFinal(sat);
                }

                string msgExito = $"Transacción de carga completada con éxito. Se agregaron {listaTemporal.Tamano} satélites.";
                bitacoraAuditoria.Registrar("INFO", msgExito);
                ViewBag.SuccessMessage = msgExito;
            }
            else
            {
                // ROLLBACK: Al menos uno falló, por lo tanto, se aborta la carga completa. La base de datos no se modifica
                string msgFallo = $"Transacción abortada (Rollback). Causa: {causaFallo}. La base de datos permanece intacta.";
                bitacoraAuditoria.Registrar("ERROR", msgFallo);
                ViewBag.ErrorMessage = msgFallo;
            }

            return View("Index", viewModel);
        }

        [HttpPost]
        public IActionResult Limpiar()
        {
            baseDatosSatelites.Limpiar();
            bitacoraAuditoria.Registrar("INFO", "Se ejecutó la purga de satélites de la memoria RAM.");
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult LimpiarLogs()
        {
            bitacoraAuditoria.Limpiar();
            bitacoraAuditoria.Registrar("INFO", "Se ejecutó la purga del historial de auditoría de la memoria RAM.");
            return RedirectToAction("Index");
        }
    }
}
