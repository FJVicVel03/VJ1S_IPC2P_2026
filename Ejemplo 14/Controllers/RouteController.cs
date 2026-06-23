using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Ejemplo_14.Models;
using Ejemplo_14.Services;

namespace Ejemplo_14.Controllers
{
    /// <summary>
    /// Controlador encargado de gestionar las operaciones de enrutamiento lógico
    /// de saltos ortogonales a través de la matriz dispersa.
    /// </summary>
    public class RouteController : Controller
    {
        private readonly EnrutadorOrtogonal enrutador = new EnrutadorOrtogonal();

        /// <summary>
        /// Acción POST para trazar la ruta de saltos ortogonales entre dos satélites.
        /// </summary>
        [HttpPost]
        public IActionResult Trazar(string origenId, string destinoId)
        {
            // 1. Validar que se hayan seleccionado origen y destino
            if (string.IsNullOrWhiteSpace(origenId) || string.IsNullOrWhiteSpace(destinoId))
            {
                TempData["ErrorMessage"] = "Debe seleccionar un satélite de origen y uno de destino.";
                return RedirectToAction("Index", "Home");
            }

            if (origenId.Equals(destinoId, StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] = "El satélite de origen y destino no pueden ser el mismo para trazar saltos.";
                return RedirectToAction("Index", "Home");
            }

            // 2. Calcular la ruta a través de los punteros físicos de la matriz
            MatrixNode[] ruta = enrutador.EncontrarRuta(MemoriaPlano.Matriz, origenId, destinoId);

            // 3. Evaluar el resultado del cálculo de ruta
            if (ruta.Length > 0)
            {
                // Guardar la ruta calculada en memoria para su destaque visual
                MemoriaPlano.RutaActiva = ruta;

                int saltos = ruta.Length - 1;
                
                // Construir una descripción textual del camino para los logs y la interfaz
                StringBuilder pathStr = new StringBuilder();
                for (int i = 0; i < ruta.Length; i++)
                {
                    pathStr.Append(ruta[i].Id);
                    if (i < ruta.Length - 1)
                    {
                        pathStr.Append(" -> ");
                    }
                }

                // Registrar éxito en la bitácora de auditoría
                MemoriaPlano.Logs.Registrar("INFO", $"Enrutamiento: Ruta calculada con éxito [{pathStr}]. Saltos: {saltos}.");
                TempData["SuccessMessage"] = $"Ruta trazada con éxito. Saltos totales: {saltos}.";
            }
            else
            {
                // Si no hay ruta disponible, limpiar la ruta activa y advertir
                MemoriaPlano.RutaActiva = null;
                
                string msgAlerta = $"Enrutamiento: No existe camino de conexión física (malla rota) entre '{origenId}' y '{destinoId}'.";
                MemoriaPlano.Logs.Registrar("ALERT", msgAlerta);
                TempData["ErrorMessage"] = "No se pudo trazar una ruta física entre los satélites seleccionados (sin conexión por punteros).";
            }

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Acción POST para limpiar el trazado de ruta actual.
        /// </summary>
        [HttpPost]
        public IActionResult Limpiar()
        {
            // Restablecer la ruta activa en memoria a null
            MemoriaPlano.RutaActiva = null;
            
            MemoriaPlano.Logs.Registrar("INFO", "Enrutamiento: Trazado de ruta e historial de saltos limpiados de la vista.");
            TempData["SuccessMessage"] = "Trazado de ruta limpiado.";
            
            return RedirectToAction("Index", "Home");
        }
    }
}
