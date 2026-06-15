using System;
using Microsoft.AspNetCore.Mvc;
using Ejemplo_11.Models;

namespace Ejemplo_11.Controllers
{
    /// <summary>
    /// Controlador especializado en exponer endpoints REST en formato JSON.
    /// No hereda la visualizacion de paginas (HTML/Razor), sino que responde serializando 
    /// el estado fisico de las estructuras almacenadas en memoria RAM a arreglos estructurados JSON.
    /// </summary>
    [ApiController]
    [Route("api")]
    public class ApiController : Controller
    {
        /// <summary>
        /// Endpoint GET: /api/satelites
        /// Retorna el listado completo de los satelites registrados en la Matriz Dispersa.
        /// Recorre los punteros de la estructura ortogonal, los proyecta a un DTO plano
        /// y retorna un arreglo estructurado en formato JSON.
        /// </summary>
        [HttpGet("satelites")]
        public IActionResult ObtenerSatelites()
        {
            // 1. Obtener la lista completa de nodos de datos desde la matriz dispersa compartida en memoria.
            //    Esta funcion recorre los cabezales y extrae los nodos a un arreglo nativo C# (MatrixNode[]).
            MatrixNode[] nodos = MemoriaPlano.Matriz.ObtenerTodosLosNodos();

            // 2. Instanciar un arreglo nativo del tipo SateliteDto con el mismo tamano
            //    para evitar el uso de colecciones genericas de .NET.
            SateliteDto[] dtos = new SateliteDto[nodos.Length];

            // 3. Copiar las propiedades lógicas de cada celda fisica (MatrixNode) al DTO plano (SateliteDto),
            //    descartando las referencias Up/Down/Left/Right para que la serializacion sea lineal.
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

            // 4. Retornar el listado estructurado utilizando la serializacion JSON nativa de ASP.NET Core.
            return Json(dtos);
        }

        /// <summary>
        /// Endpoint GET: /api/logs
        /// Retorna la bitacora de logs historicos de auditoria en formato JSON.
        /// </summary>
        [HttpGet("logs")]
        public IActionResult ObtenerLogs()
        {
            // 1. Obtener los logs de auditoria guardados desde el almacenamiento estatico MemoriaPlano.
            //    La funcion ObtenerTodos() retorna un arreglo nativo C# (LogRegistro[]) calculado a partir
            //    de la lista enlazada simple de logs.
            LogRegistro[] registros = MemoriaPlano.Logs.ObtenerTodos();

            // 2. Retornar directamente el arreglo en formato JSON.
            //    La clase LogRegistro no contiene punteros autorreferenciales, por lo que no requiere DTO.
            return Json(registros);
        }
    }
}

