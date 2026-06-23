using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ejemplo_14.Models;

namespace Ejemplo_14.Controllers
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
        /// Registra las etapas de la petición en la bitácora física, inyecta las credenciales Basic Auth
        /// si se proporcionan, maneja respuestas HTTP 401/200, y redirecciona el resultado a la vista.
        /// </summary>
        /// <param name="urlDestino">Dirección web completa a consultar.</param>
        /// <param name="usuario">Nombre de usuario para la autenticación básica (opcional).</param>
        /// <param name="contrasena">Contraseña para la autenticación básica (opcional).</param>
        [HttpPost]
        public async Task<IActionResult> ConsultarApi(string urlDestino, string? usuario, string? contrasena)
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
                // Instanciar un HttpRequestMessage específico para esta solicitud.
                // Esto es más seguro que usar clienteHttp.DefaultRequestHeaders en un HttpClient estático y compartido
                // para evitar colisiones de cabeceras entre múltiples peticiones en paralelo.
                using (var peticion = new HttpRequestMessage(HttpMethod.Get, urlDestino))
                {
                    // Verificar si se ingresó usuario o contraseña para realizar la autenticación básica HTTP.
                    if (!string.IsNullOrEmpty(usuario) || !string.IsNullOrEmpty(contrasena))
                    {
                        // Asegurar valores no nulos para usuario y contraseña.
                        string userVal = usuario ?? string.Empty;
                        string passVal = contrasena ?? string.Empty;

                        // 1. Concatenar el usuario y la contraseña en formato 'usuario:contrasena'
                        string credencialesSeparadas = $"{userVal}:{passVal}";

                        // 2. Codificar en Base64 utilizando la codificación UTF-8
                        string base64Credenciales = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(credencialesSeparadas));

                        // 3. Configurar la cabecera 'Authorization' con el esquema 'Basic' y las credenciales codificadas
                        peticion.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", base64Credenciales);

                        // Registrar en la bitácora la inyección de credenciales sin comprometer la seguridad (no mostrar contraseña)
                        MemoriaPlano.Logs.Registrar("INFO", $"Cliente HTTP: Inyectando cabecera de autenticacion Basic para el usuario '{userVal}'");
                    }

                    // Enviar la petición HTTP asíncronamente
                    using (HttpResponseMessage response = await clienteHttp.SendAsync(peticion))
                    {
                        // Comprobar si el servidor respondió con un código 401 (Unauthorized)
                        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            // Leer el cuerpo de la respuesta en formato JSON (payload de fallo de la API)
                            string rawJson = await response.Content.ReadAsStringAsync();

                            // Registrar la alerta de denegación en la bitácora
                            MemoriaPlano.Logs.Registrar("ALERT", $"Cliente HTTP: Solicitud denegada (401 Unauthorized) en '{urlDestino}'");

                            // Almacenar el resultado y el mensaje de error para mostrarlos en la UI
                            TempData["ResultadoHttp"] = rawJson;
                            TempData["ErrorMessage"] = "Peticion Rechazada: 401 No Autorizado. El servidor de destino rechazo las credenciales.";
                        }
                        else
                        {
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
                    }
                }
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


