using System;
using Microsoft.AspNetCore.Mvc;
using Ejemplo_8.Models;
using Ejemplo_8.Services;

namespace Ejemplo_8.Controllers
{
    /// <summary>
    /// Controlador principal que se encarga exclusivamente de renderizar la vista del panel principal (Dashboard).
    /// </summary>
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            // Cargar los mensajes de retroalimentación provenientes de TempData (redireccionados de otros controladores)
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"]!.ToString();
            }
            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"]!.ToString();
            }

            // Registrar los logs iniciales si la bitácora está completamente vacía
            if (MemoriaPlano.Logs.EstaVacia)
            {
                MemoriaPlano.Logs.Registrar("INFO", "Sistema de visualización de plano satelital activo (Ejemplo 8).");
                MemoriaPlano.Logs.Registrar("INFO", "TDA Matriz Dispersa Ortogonal inicializada en memoria RAM.");
                MemoriaPlano.Logs.Registrar("INFO", "Arquitectura refactorizada: Controladores modulares.");
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
                SvgDiagrama = svgOutput
            };
        }
    }
}
