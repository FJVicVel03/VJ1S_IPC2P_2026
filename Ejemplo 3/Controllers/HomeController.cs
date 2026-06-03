using Microsoft.AspNetCore.Mvc;
using Ejemplo_3.Models;

namespace Ejemplo_3.Controllers
{
    /// <summary>
    /// Controlador principal para la gestión de la vista de inicio.
    /// Administra la instancia estática del TDA ListaSatelites en memoria RAM.
    /// </summary>
    public class HomeController : Controller
    {
        // Instancia estática en memoria RAM que actúa como base de datos local para la constelación
        private static readonly ListaSatelites satelitesBase = new ListaSatelites();

        public IActionResult Index()
        {
            // Carga inicial de datos de prueba si la lista enlazada manual está vacía
            if (satelitesBase.EstaVacia)
            {
                satelitesBase.InsertarAlFinal(new Satelite("S001", "Satelite 1", "127.0.0.1"));
                satelitesBase.InsertarAlFinal(new Satelite("S002", "Satelite 2", "192.168.20.1"));
                satelitesBase.InsertarAlFinal(new Satelite("S003", "Satelite 3", "124.123.54.2"));
            }

            // Enviamos el TDA directamente como modelo a la vista Razor
            return View(satelitesBase);
        }
    }
}
