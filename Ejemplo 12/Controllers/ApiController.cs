using System;
using Microsoft.AspNetCore.Mvc;
using Ejemplo_12.Models;
using Ejemplo_12.Attributes;

namespace Ejemplo_12.Controllers
{
    /// <summary>
    /// Controlador especializado en exponer endpoints REST en formato JSON.
    /// No hereda la visualización de páginas (HTML/Razor), sino que responde serializando 
    /// el estado físico de las estructuras almacenadas en memoria RAM a arreglos estructurados JSON.
    /// </summary>
    [ApiController]
    [Route("api")]
    public class ApiController : Controller
    {
        /// <summary>
        /// Endpoint GET: /api/satelites (Público)
        /// Retorna el listado completo de los satélites registrados en la Matriz Dispersa.
        /// </summary>
        [HttpGet("satelites")]
        public IActionResult ObtenerSatelites()
        {
            // 1. Obtener la lista completa de nodos de datos desde la matriz dispersa compartida en memoria.
            MatrixNode[] nodos = MemoriaPlano.Matriz.ObtenerTodosLosNodos();

            // 2. Instanciar un arreglo nativo del tipo SateliteDto con el mismo tamaño para evitar colecciones genéricas.
            SateliteDto[] dtos = new SateliteDto[nodos.Length];

            // 3. Copiar las propiedades lógicas de cada celda física al DTO plano.
            for (int i = 0; i < nodos.Length; i++)
            {
                dtos[i] = new SateliteDto
                {
                    Fila = nodos[i].Row,
                    Columna = nodos[i].Col,
                    Id = nodos[i].Id,
                    Nombre = nodos[i].Nombre,
                    IpAddress = nodos[i].IpAddress
                };
            }

            return Json(dtos);
        }

        /// <summary>
        /// Endpoint GET: /api/seguro/satelites (Protegido con HTTP Basic Auth)
        /// Retorna el mismo listado de satélites pero requiere credenciales válidas en la cabecera HTTP.
        /// </summary>
        [BasicAuthorize]
        [HttpGet("seguro/satelites")]
        public IActionResult ObtenerSatelitesSeguro()
        {
            // 1. Obtener la lista completa de nodos de datos desde la matriz dispersa compartida en memoria.
            MatrixNode[] nodos = MemoriaPlano.Matriz.ObtenerTodosLosNodos();

            // 2. Instanciar un arreglo nativo de SateliteDto con el mismo tamaño para evitar colecciones genéricas.
            SateliteDto[] dtos = new SateliteDto[nodos.Length];

            // 3. Copiar las propiedades lógicas al DTO para su serialización limpia.
            for (int i = 0; i < nodos.Length; i++)
            {
                dtos[i] = new SateliteDto
                {
                    Fila = nodos[i].Row,
                    Columna = nodos[i].Col,
                    Id = nodos[i].Id,
                    Nombre = nodos[i].Nombre,
                    IpAddress = nodos[i].IpAddress
                };
            }

            // 4. Retornar el listado estructurado de forma segura.
            return Json(dtos);
        }

        /// <summary>
        /// Endpoint GET: /api/logs
        /// Retorna la bitácora de logs históricos de auditoría en formato JSON.
        /// </summary>
        [HttpGet("logs")]
        public IActionResult ObtenerLogs()
        {
            // 1. Obtener los logs de auditoría guardados desde el almacenamiento estático.
            LogRegistro[] registros = MemoriaPlano.Logs.ObtenerTodos();

            // 2. Retornar directamente el arreglo en formato JSON.
            return Json(registros);
        }
    }
}

