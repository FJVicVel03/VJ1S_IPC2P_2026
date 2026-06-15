using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ejemplo_11.Models;

namespace Ejemplo_11.Controllers
{
    /// <summary>
    /// Controlador encargado de realizar peticiones HTTP salientes hacia otros procesos o servidores.
    /// Utiliza un cliente HttpClient compartido y estático para optimizar la reutilización de sockets de red.
    /// </summary>
    public class HttpClienteController : Controller
    {
        // Instancia estática de HttpClient recomendada para evitar el agotamiento de sockets bajo cargas concurrentes.
        private static readonly HttpClient clienteHttp = new HttpClient();

        /// <summary>
        /// Realiza una petición GET de forma asíncrona a la URL de destino proporcionada por el usuario.
        /// Registra las etapas de la petición en la bitácora física y redirecciona el JSON de respuesta a la vista.
        /// </summary>
        /// <param name="urlDestino">Dirección web completa a consultar.</param>
        [HttpPost]
        public async Task<IActionResult> ConsultarApi(string urlDestino)
        {
            // Validar que la dirección URL no se encuentre en blanco
            if (string.IsNullOrWhiteSpace(urlDestino))
            {
                TempData["ErrorMessage"] = "La URL de destino no puede estar vacía.";
                return RedirectToAction("Index", "Home");
            }

            urlDestino = urlDestino.Trim();
            
            // Registrar en la consola de logs local el inicio de la petición
            MemoriaPlano.Logs.Registrar("INFO", $"Cliente HTTP: Iniciando petición GET a '{urlDestino}'");

            try
            {
                // Realizar la solicitud GET de forma asíncrona hacia el servidor externo
                HttpResponseMessage response = await clienteHttp.GetAsync(urlDestino);
                
                // Evaluar si el código de estado retornado está en el rango exitoso (2xx). Si falla, lanza una excepción.
                response.EnsureSuccessStatusCode();

                // Leer la secuencia de bytes del cuerpo de la respuesta como una cadena de texto (formato JSON esperado)
                string rawJson = await response.Content.ReadAsStringAsync();

                // Registrar en la bitácora que la transacción finalizó correctamente con su código de estado
                MemoriaPlano.Logs.Registrar("INFO", $"Cliente HTTP: Petición exitosa a '{urlDestino}'. Código: {(int)response.StatusCode}");
                
                // Almacenar el JSON de respuesta y el mensaje de éxito en TempData para recuperarlos después de la redirección
                TempData["ResultadoHttp"] = rawJson;
                TempData["SuccessMessage"] = "Respuesta HTTP recibida con éxito.";
            }
            catch (Exception ex)
            {
                // Capturar fallos de red, DNS o códigos de error HTTP de forma controlada
                string errorMsg = $"Cliente HTTP: Falló la petición a '{urlDestino}'. Detalle: {ex.Message}";
                
                // Registrar el incidente en la bitácora local y pasar el aviso a la vista
                MemoriaPlano.Logs.Registrar("ERROR", errorMsg);
                TempData["ErrorMessage"] = errorMsg;
            }

            // Redireccionar al usuario de vuelta al Dashboard principal en HomeController
            return RedirectToAction("Index", "Home");
        }
    }
}
