using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ejemplo_6.Models;

namespace Ejemplo_6.Controllers
{
    /// <summary>
    /// Controlador principal para coordinar el catálogo AVL y la bitácora de auditoría.
    /// </summary>
    public class HomeController : Controller
    {
        // Estructuras de datos estáticas persistidas en memoria RAM
        private static readonly ArbolSatelitesAvl baseDatosSatelites = new ArbolSatelitesAvl();
        private static readonly ListaLogs bitacoraAuditoria = new ListaLogs();

        // Patrones Regex de Validación Oficiales
        private const string PatronIdSatelite = @"^SAT-(ECU|POL)-\d{4}$";
        private const string PatronIpv4 = @"^(?:(?:25[0-5]|2[0-4]\d|[01]?\d?\d)\.){3}(?:25[0-5]|2[0-4]\d|[01]?\d?\d)$";

        // Estructura lineal temporal para la atomicidad transaccional
        private class NodoTemporal
        {
            public Satelite Valor { get; set; }
            public NodoTemporal? Siguiente { get; set; }
            public NodoTemporal(Satelite valor)
            {
                Valor = valor;
                Siguiente = null;
            }
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (bitacoraAuditoria.EstaVacia)
            {
                bitacoraAuditoria.Registrar("INFO", "Sistema de catálogo satelital inicializado.");
                bitacoraAuditoria.Registrar("INFO", "Estructura de datos Árbol AVL lista para operar.");
            }

            return View(ObtenerViewModel());
        }

        [HttpPost]
        public IActionResult CargarXml(IFormFile archivoXml)
        {
            var viewModel = ObtenerViewModel();

            if (archivoXml == null || archivoXml.Length == 0)
            {
                string msgErr = "Por favor, seleccione un archivo XML válido.";
                bitacoraAuditoria.Registrar("ALERT", msgErr);
                ViewBag.ErrorMessage = msgErr;
                return View("Index", viewModel);
            }

            bitacoraAuditoria.Registrar("INFO", $"Iniciando carga de archivo: '{archivoXml.FileName}'");

            NodoTemporal? cabezaTemporal = null;
            int contadorTemporal = 0;
            bool transaccionExitosa = true;
            string causaFallo = "";

            try
            {
                // OWASP: Mitigar vulnerabilidades XXE prohibiendo procesamiento DTD externo
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
                        foreach (XmlNode nodo in satelitesNodos)
                        {
                            string? id = nodo.Attributes?["id"]?.Value;
                            string? nombre = nodo.SelectSingleNode("nombre")?.InnerText;
                            string? enlaceIp = nodo.SelectSingleNode("enlace_ip")?.InnerText;

                            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(enlaceIp))
                            {
                                transaccionExitosa = false;
                                causaFallo = "Se encontró un satélite con datos faltantes en el XML.";
                                break;
                            }

                            id = id.Trim();
                            nombre = nombre.Trim();
                            enlaceIp = enlaceIp.Trim();

                            // 1. Validar ID del satélite con Regex
                            if (!Regex.IsMatch(id, PatronIdSatelite))
                            {
                                transaccionExitosa = false;
                                causaFallo = $"El satélite con ID '{id}' no cumple con el formato requerido 'SAT-(ECU|POL)-0000'.";
                                break;
                            }

                            // 2. Validar IP con Regex
                            if (!Regex.IsMatch(enlaceIp, PatronIpv4))
                            {
                                transaccionExitosa = false;
                                causaFallo = $"El satélite [{id}] contiene una dirección IP inválida: '{enlaceIp}'.";
                                break;
                            }

                            // 3. Validar duplicados en la base de datos principal (AVL)
                            if (baseDatosSatelites.Buscar(id) != null)
                            {
                                transaccionExitosa = false;
                                causaFallo = $"El satélite con ID '{id}' ya existe en el catálogo AVL.";
                                break;
                            }

                            // 4. Validar duplicados en la misma carga temporal
                            bool duplicadoTemporal = false;
                            NodoTemporal? actual = cabezaTemporal;
                            while (actual != null)
                            {
                                if (actual.Valor.Id.Equals(id, StringComparison.OrdinalIgnoreCase))
                                {
                                    duplicadoTemporal = true;
                                    break;
                                }
                                actual = actual.Siguiente;
                            }

                            if (duplicadoTemporal)
                            {
                                transaccionExitosa = false;
                                causaFallo = $"El satélite con ID '{id}' está duplicado dentro del mismo archivo XML.";
                                break;
                            }

                            // Insertar en la lista temporal de carga
                            Satelite sat = new Satelite(id, nombre, enlaceIp);
                            NodoTemporal nuevo = new NodoTemporal(sat);
                            if (cabezaTemporal == null)
                            {
                                cabezaTemporal = nuevo;
                            }
                            else
                            {
                                NodoTemporal temp = cabezaTemporal;
                                while (temp.Siguiente != null)
                                {
                                    temp = temp.Siguiente;
                                }
                                temp.Siguiente = nuevo;
                            }
                            contadorTemporal++;
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

            // Consolidación transaccional (Commit / Rollback)
            if (transaccionExitosa && cabezaTemporal != null)
            {
                // COMMIT: Insertar todo al catálogo AVL
                NodoTemporal? actual = cabezaTemporal;
                while (actual != null)
                {
                    baseDatosSatelites.Insertar(actual.Valor); // Inserta en el AVL
                    actual = actual.Siguiente;
                }

                string msgSucc = $"Carga transaccional exitosa (Commit). Se integraron {contadorTemporal} satélites al catálogo AVL.";
                bitacoraAuditoria.Registrar("INFO", msgSucc);
                ViewBag.SuccessMessage = msgSucc;
            }
            else
            {
                // ROLLBACK: No se altera el Árbol AVL
                string msgFallo = $"Carga transaccional abortada (Rollback). Causa: {causaFallo}. El catálogo permanece intacto.";
                bitacoraAuditoria.Registrar("ERROR", msgFallo);
                ViewBag.ErrorMessage = msgFallo;
            }

            return View("Index", ObtenerViewModel());
        }

        [HttpPost]
        public IActionResult AgregarSatelite(string id, string nombre, string enlaceIp)
        {
            var viewModel = ObtenerViewModel();

            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(enlaceIp))
            {
                string msgErr = "Error al registrar: Todos los campos del satélite son obligatorios.";
                bitacoraAuditoria.Registrar("ALERT", msgErr);
                ViewBag.ErrorMessage = msgErr;
                return View("Index", viewModel);
            }

            id = id.Trim();
            nombre = nombre.Trim();
            enlaceIp = enlaceIp.Trim();

            // 1. Validar ID del satélite con Regex
            if (!Regex.IsMatch(id, PatronIdSatelite))
            {
                string msgErr = $"Error de formato en ID '{id}': Debe cumplir con 'SAT-ECU-####' o 'SAT-POL-####'.";
                bitacoraAuditoria.Registrar("ALERT", msgErr);
                ViewBag.ErrorMessage = msgErr;
                return View("Index", viewModel);
            }

            // 2. Validar IP con Regex
            if (!Regex.IsMatch(enlaceIp, PatronIpv4))
            {
                string msgErr = $"Error de formato en IP '{enlaceIp}': Debe ser una dirección IPv4 válida.";
                bitacoraAuditoria.Registrar("ALERT", msgErr);
                ViewBag.ErrorMessage = msgErr;
                return View("Index", viewModel);
            }

            // 3. Validar duplicados en el Árbol AVL
            if (baseDatosSatelites.Buscar(id) != null)
            {
                string msgErr = $"El satélite con ID '{id}' ya existe en el árbol AVL.";
                bitacoraAuditoria.Registrar("ERROR", msgErr);
                ViewBag.ErrorMessage = msgErr;
                return View("Index", viewModel);
            }

            try
            {
                // Crear e insertar satélite en el Árbol AVL
                Satelite nuevoSatelite = new Satelite(id, nombre, enlaceIp);
                baseDatosSatelites.Insertar(nuevoSatelite);

                string msgSucc = $"Satélite registrado con éxito: {nombre} ({id}). Árbol AVL auto-balanceado.";
                bitacoraAuditoria.Registrar("INFO", msgSucc);
                ViewBag.SuccessMessage = msgSucc;
            }
            catch (Exception ex)
            {
                string msgErr = $"Error al insertar satélite en árbol AVL: {ex.Message}";
                bitacoraAuditoria.Registrar("ERROR", msgErr);
                ViewBag.ErrorMessage = msgErr;
            }

            return View("Index", ObtenerViewModel());
        }

        [HttpPost]
        public IActionResult LimpiarTodo()
        {
            baseDatosSatelites.Limpiar();
            bitacoraAuditoria.Registrar("INFO", "Se purgaron todos los datos de Satélites del catálogo AVL.");
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
            return new DashboardViewModel
            {
                Satelites = baseDatosSatelites,
                Logs = bitacoraAuditoria
            };
        }
    }
}
