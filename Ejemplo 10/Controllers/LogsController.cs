using System;
using Microsoft.AspNetCore.Mvc;
using Ejemplo_10.Models;

namespace Ejemplo_10.Controllers
{
    /// <summary>
    /// Controlador especializado en administrar los registros de auditoría del sistema.
    /// </summary>
    public class LogsController : Controller
    {
        [HttpPost]
        public IActionResult LimpiarLogs()
        {
            MemoriaPlano.Logs.Limpiar();
            MemoriaPlano.Logs.Registrar("INFO", "Se purgó la bitácora de auditoría.");
            TempData["SuccessMessage"] = "Bitácora de auditoría reiniciada.";
            return RedirectToAction("Index", "Home");
        }
    }
}


