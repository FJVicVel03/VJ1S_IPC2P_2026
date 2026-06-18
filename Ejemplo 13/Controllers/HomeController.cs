using System;
using Microsoft.AspNetCore.Mvc;
using Ejemplo_13.Models;
using Ejemplo_13.Services;

namespace Ejemplo_13.Controllers
{
    /// <summary>
    /// Controlador principal que se encarga exclusivamente de renderizar la vista del panel principal (Dashboard).
    /// </summary>
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index(string? sateliteId)
        {
            // Pasar el satélite actualmente seleccionado para mantener el foco en la UI
            ViewBag.SateliteSeleccionado = sateliteId;

            // Cargar los mensajes de retroalimentación provenientes de TempData (redireccionados de otros controladores)
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"]!.ToString();
            }
            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"]!.ToString();
            }

            // Recuperar la respuesta de la petición del cliente HTTP externa guardada en TempData
            if (TempData["ResultadoHttp"] != null)
            {
                ViewBag.ResultadoHttp = TempData["ResultadoHttp"]!.ToString();
            }

            // Registrar los logs iniciales si la bitácora está completamente vacía
            if (MemoriaPlano.Logs.EstaVacia)
            {
                MemoriaPlano.Logs.Registrar("INFO", "Sistema de visualizacion de plano satelital activo (Ejemplo 13).");
                MemoriaPlano.Logs.Registrar("INFO", "TDA Matriz Dispersa, Arbol AVL y Buffer de Mensajes (ABB) inicializados en RAM.");
                MemoriaPlano.Logs.Registrar("INFO", "Seguridad HTTP Basic Auth y encolamiento prioritario ABB de mensajes activos.");
            }

            return View(ObtenerViewModel());
        }

        private DashboardViewModel ObtenerViewModel()
        {
            // 1. Generar código fuente DOT a partir de las referencias de la Matriz en MemoriaPlano
            string codigoDot = MemoriaPlano.Matriz.GenerarCodigoDot();

            // 2. Compilar en caliente a SVG vectorial utilizando la clase de compilación
            string svgOutput = GraphvizCompilador.CompilarDotASvg(codigoDot);

            return new DashboardViewModel
            {
                Matriz = MemoriaPlano.Matriz,
                Logs = MemoriaPlano.Logs,
                Catalogo = MemoriaPlano.Catalogo,
                SvgDiagrama = svgOutput
            };
        }
    }
}



