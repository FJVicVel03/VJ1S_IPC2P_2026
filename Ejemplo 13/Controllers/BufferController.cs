using System;
using Microsoft.AspNetCore.Mvc;
using Ejemplo_13.Models;

namespace Ejemplo_13.Controllers
{
    /// <summary>
    /// Controlador especializado en administrar las operaciones del TDA BufferMensajes (ABB)
    /// asociado a cada satélite de la red.
    /// Permite encolar y desencolar mensajes de forma prioritaria en caliente.
    /// </summary>
    public class BufferController : Controller
    {
        /// <summary>
        /// Acción POST para encolar un paquete de datos en el buffer de un satélite específico.
        /// </summary>
        [HttpPost]
        public IActionResult Encolar(string sateliteId, string hexCode, string emisorId, string destIp, int priority, string content)
        {
            // 1. Validar que el identificador del satélite no se encuentre vacío.
            if (string.IsNullOrWhiteSpace(sateliteId))
            {
                TempData["ErrorMessage"] = "Debe especificar un satélite válido.";
                return RedirectToAction("Index", "Home");
            }

            // 2. Validar que la prioridad esté dentro del rango admitido (1 al 5).
            if (priority < 1 || priority > 5)
            {
                TempData["ErrorMessage"] = "La prioridad debe estar en un rango de 1 a 5.";
                return RedirectToAction("Index", "Home", new { sateliteId });
            }

            // 3. Validar otros datos requeridos.
            if (string.IsNullOrWhiteSpace(hexCode) || string.IsNullOrWhiteSpace(destIp))
            {
                TempData["ErrorMessage"] = "El código de paquete y la IP de destino son obligatorios.";
                return RedirectToAction("Index", "Home", new { sateliteId });
            }

            // 4. Buscar el nodo del satélite en la matriz dispersa compartida.
            //    Recorremos el arreglo plano de nodos de la matriz para evitar el uso de colecciones genéricas.
            MatrixNode[] nodos = MemoriaPlano.Matriz.ObtenerTodosLosNodos();
            MatrixNode? satelite = null;

            for (int i = 0; i < nodos.Length; i++)
            {
                if (nodos[i].Id.Equals(sateliteId, StringComparison.OrdinalIgnoreCase))
                {
                    satelite = nodos[i];
                    break;
                }
            }

            // 5. Si no se localiza el satélite, reportar el error en la bitácora y retornar.
            if (satelite == null)
            {
                string msgError = $"Buffer: Intento fallido de encolamiento. No existe el nodo satelital '{sateliteId}'.";
                MemoriaPlano.Logs.Registrar("ERROR", msgError);
                TempData["ErrorMessage"] = msgError;
                return RedirectToAction("Index", "Home");
            }

            // 6. Instanciar el nodo del paquete (AbbNode) e insertarlo en el buffer del satélite.
            AbbNode nuevoPaquete = new AbbNode(hexCode.Trim().ToUpper(), emisorId.Trim().ToUpper(), destIp.Trim(), priority, content ?? string.Empty);
            satelite.Buffer.Enqueue(nuevoPaquete);

            // 7. Registrar el evento en la bitácora de auditoría.
            MemoriaPlano.Logs.Registrar("INFO", $"Buffer: Paquete {nuevoPaquete.HexCode} (Prioridad {priority}) encolado exitosamente en '{sateliteId}'.");
            
            TempData["SuccessMessage"] = $"Paquete {nuevoPaquete.HexCode} encolado en el buffer de {satelite.Nombre}.";
            
            // Redireccionar al Dashboard manteniendo seleccionado el satélite actual para visualización rápida de la cola.
            return RedirectToAction("Index", "Home", new { sateliteId });
        }

        /// <summary>
        /// Acción POST para desencolar el mensaje de máxima prioridad en el buffer de un satélite específico.
        /// </summary>
        [HttpPost]
        public IActionResult Desencolar(string sateliteId)
        {
            // 1. Validar el identificador del satélite.
            if (string.IsNullOrWhiteSpace(sateliteId))
            {
                TempData["ErrorMessage"] = "Debe especificar un satélite válido.";
                return RedirectToAction("Index", "Home");
            }

            // 2. Buscar el nodo del satélite correspondiente en la matriz.
            MatrixNode[] nodos = MemoriaPlano.Matriz.ObtenerTodosLosNodos();
            MatrixNode? satelite = null;

            for (int i = 0; i < nodos.Length; i++)
            {
                if (nodos[i].Id.Equals(sateliteId, StringComparison.OrdinalIgnoreCase))
                {
                    satelite = nodos[i];
                    break;
                }
            }

            // 3. Reportar error si no se encuentra.
            if (satelite == null)
            {
                TempData["ErrorMessage"] = $"No se encontró el satélite '{sateliteId}' en la matriz.";
                return RedirectToAction("Index", "Home");
            }

            // 4. Invocar el método Dequeue() para extraer el nodo de máxima prioridad (el de la extrema derecha).
            AbbNode? despachado = satelite.Buffer.Dequeue();

            if (despachado == null)
            {
                // Si retorna null, el buffer está vacío.
                string msgAlerta = $"Buffer: Intento de desencolado fallido en '{sateliteId}'. El buffer de mensajes se encuentra vacio.";
                MemoriaPlano.Logs.Registrar("ALERT", msgAlerta);
                TempData["ErrorMessage"] = $"El buffer del satélite {sateliteId} está vacío.";
            }
            else
            {
                // Si se extrajo un paquete, registrar sus datos de tránsito en la bitácora de auditoría.
                string msgInfo = $"Buffer: Paquete {despachado.HexCode} (Prioridad {despachado.Priority}) extraido de '{sateliteId}' hacia la IP Terrestre {despachado.DestIp}.";
                MemoriaPlano.Logs.Registrar("INFO", msgInfo);
                
                TempData["SuccessMessage"] = $"Desencolado paquete {despachado.HexCode} (Prioridad: {despachado.Priority}) del satélite {sateliteId}.";
            }

            return RedirectToAction("Index", "Home", new { sateliteId });
        }
    }
}
