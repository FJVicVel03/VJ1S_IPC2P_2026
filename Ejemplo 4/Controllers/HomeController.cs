using System;
using System.IO;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ejemplo_4.Models;

namespace Ejemplo_4.Controllers
{
    /// <summary>
    /// Controlador que gestiona la carga masiva y visualización web de los satélites.
    /// Utiliza XmlDocument con mitigación de ataques XXE y consultas XPath directas.
    /// </summary>
    public class HomeController : Controller
    {
        // Instancia estática en memoria RAM para emular la persistencia física
        private static readonly ListaSatelites satelitesBase = new ListaSatelites();

        [HttpGet]
        public IActionResult Index()
        {
            // Pasamos el TDA directamente como modelo
            return View(satelitesBase);
        }

        [HttpPost]
        public IActionResult CargarXml(IFormFile archivoXml)
        {
            if (archivoXml == null || archivoXml.Length == 0)
            {
                ViewBag.ErrorMessage = "Por favor, seleccione un archivo XML válido.";
                return View("Index", satelitesBase);
            }

            try
            {
                // 1. Mitigación de Vulnerabilidades XXE (XML External Entity Injection)
                // Se configura el XmlReaderSettings para deshabilitar el procesamiento de DTDs externas.
                XmlReaderSettings settings = new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Prohibit, // Evita la carga de entidades externas (Mitigación XXE)
                    XmlResolver = null                     // Deshabilita el resolutor XML
                };

                using (Stream stream = archivoXml.OpenReadStream())
                using (XmlReader reader = XmlReader.Create(stream, settings))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(reader);

                    // 2. Selección de nodos mediante consulta XPath estructurada
                    // Buscamos todos los elementos 'satelite' bajo la ruta lógica indicada
                    XmlNodeList satelitesNodos = doc.SelectNodes("/orbitnet/constelaciones_ecuatoriales/satelite")!;

                    if (satelitesNodos.Count == 0)
                    {
                        ViewBag.WarningMessage = "No se encontraron satélites bajo la ruta XPath: '/orbitnet/constelaciones_ecuatoriales/satelite'.";
                        return View("Index", satelitesBase);
                    }

                    int cargadosConExito = 0;
                    int erroresValidacion = 0;

                    // 3. Procesamiento de los nodos XML
                    foreach (XmlNode nodo in satelitesNodos)
                    {
                        string? id = nodo.Attributes?["id"]?.Value;
                        string? nombre = nodo.SelectSingleNode("nombre")?.InnerText;
                        string? enlaceIp = nodo.SelectSingleNode("enlace_ip")?.InnerText;

                        if (id == null || nombre == null || enlaceIp == null)
                        {
                            erroresValidacion++;
                            continue;
                        }

                        try
                        {
                            // Intentamos instanciar el Satélite (las validaciones internas del setter se disparan aquí)
                            Satelite nuevoSatelite = new Satelite(id.Trim(), nombre.Trim(), enlaceIp.Trim());
                            satelitesBase.InsertarAlFinal(nuevoSatelite);
                            cargadosConExito++;
                        }
                        catch (ArgumentException ex)
                        {
                            // Captura errores de validación de datos
                            erroresValidacion++;
                            // Registramos el error de validación en consola de depuración (o TempData/ViewBag)
                            System.Diagnostics.Debug.WriteLine($"Fallo de validación en nodo XML: {ex.Message}");
                        }
                    }

                    ViewBag.SuccessMessage = $"Procesamiento de XML finalizado. Satélites agregados con éxito: {cargadosConExito}. Descartados por validación: {erroresValidacion}.";
                }
            }
            catch (XmlException ex)
            {
                ViewBag.ErrorMessage = $"Error de sintaxis XML: {ex.Message}";
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error inesperado al procesar el archivo: {ex.Message}";
            }

            return View("Index", satelitesBase);
        }

        [HttpPost]
        public IActionResult Limpiar()
        {
            // Vaciar la lista enlazada en memoria RAM
            satelitesBase.Limpiar();
            ViewBag.SuccessMessage = "Memoria RAM purgada con éxito.";
            return View("Index", satelitesBase);
        }
    }
}
