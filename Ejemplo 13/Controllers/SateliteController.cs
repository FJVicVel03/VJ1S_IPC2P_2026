using System;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Ejemplo_13.Models;

namespace Ejemplo_13.Controllers
{
    /// <summary>
    /// Controlador especializado en operaciones manuales sobre los nodos de satélite en la Matriz Dispersa.
    /// </summary>
    public class SateliteController : Controller
    {
        private const string PatronIdSatelite = @"^SAT-(ECU|POL)-\d{4}$";
        private const string PatronIpv4 = @"^(?:(?:25[0-5]|2[0-4]\d|[01]?\d?\d)\.){3}(?:25[0-5]|2[0-4]\d|[01]?\d?\d)$";

        [HttpPost]
        public IActionResult InsertarNodo(int row, int col, string id, string nombre, string ip)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(ip))
            {
                string msgErr = "Error al insertar: Todos los campos del satélite son obligatorios.";
                MemoriaPlano.Logs.Registrar("ALERT", msgErr);
                TempData["ErrorMessage"] = msgErr;
                return RedirectToAction("Index", "Home");
            }

            id = id.Trim();
            nombre = nombre.Trim();
            ip = ip.Trim();


            // 1. Validar ID con Regex
            if (!Regex.IsMatch(id, PatronIdSatelite))
            {
                string msgErr = $"Error sintáctico en ID '{id}': Debe cumplir con el formato 'SAT-(ECU|POL)-0000'.";
                MemoriaPlano.Logs.Registrar("ALERT", msgErr);
                TempData["ErrorMessage"] = msgErr;
                return RedirectToAction("Index", "Home");
            }

            // 2. Validar IP con Regex
            if (!Regex.IsMatch(ip, PatronIpv4))
            {
                string msgErr = $"Error sintáctico en IP '{ip}': Debe ser una dirección IPv4 válida.";
                MemoriaPlano.Logs.Registrar("ALERT", msgErr);
                TempData["ErrorMessage"] = msgErr;
                return RedirectToAction("Index", "Home");
            }

            // 3. Validar Colisión de Coordenadas
            if (MemoriaPlano.Matriz.Search(row, col) != null)
            {
                string msgErr = $"Colisión detectada: ya existe un nodo en las coordenadas ({row}, {col}).";
                MemoriaPlano.Logs.Registrar("ERROR", msgErr);
                TempData["ErrorMessage"] = msgErr;
                return RedirectToAction("Index", "Home");
            }

            // 4. Validar Identificador Duplicado
            if (MemoriaPlano.Matriz.BuscarPorId(id) != null)
            {
                string msgErr = $"El identificador de satélite '{id}' ya existe en el plano.";
                MemoriaPlano.Logs.Registrar("ERROR", msgErr);
                TempData["ErrorMessage"] = msgErr;
                return RedirectToAction("Index", "Home");
            }

            try
            {
                MemoriaPlano.Matriz.Insert(row, col, id, nombre, ip);
                string msgSucc = $"Nodo satelital [{id}] insertado con éxito en ({row}, {col}). Enlaces ortogonales actualizados.";
                MemoriaPlano.Logs.Registrar("INFO", msgSucc);
                TempData["SuccessMessage"] = msgSucc;
            }
            catch (Exception ex)
            {
                string msgErr = $"Error de inserción física: {ex.Message}";
                MemoriaPlano.Logs.Registrar("ERROR", msgErr);
                TempData["ErrorMessage"] = msgErr;
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult EliminarNodo(int row, int col)
        {

            MatrixNode? target = MemoriaPlano.Matriz.Search(row, col);
            if (target == null)
            {
                string msgErr = $"No existe ningún nodo en la coordenada ({row}, {col}) para eliminar.";
                MemoriaPlano.Logs.Registrar("ALERT", msgErr);
                TempData["ErrorMessage"] = msgErr;
                return RedirectToAction("Index", "Home");
            }

            try
            {
                string idEliminado = target.Id;
                MemoriaPlano.Matriz.Delete(row, col);
                string msgSucc = $"Nodo [{idEliminado}] eliminado de ({row}, {col}). Enlaces vecinos reconectados.";
                MemoriaPlano.Logs.Registrar("INFO", msgSucc);
                TempData["SuccessMessage"] = msgSucc;
            }
            catch (Exception ex)
            {
                string msgErr = $"Error al eliminar nodo: {ex.Message}";
                MemoriaPlano.Logs.Registrar("ERROR", msgErr);
                TempData["ErrorMessage"] = msgErr;
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult LimpiarMatriz()
        {
            MemoriaPlano.Matriz.Clear();
            MemoriaPlano.Catalogo.Limpiar(); // Limpiar el catalogo AVL de satelites polares
            MemoriaPlano.Logs.Registrar("INFO", "Se purgaron todos los nodos de la Matriz Dispersa y el catalogo AVL.");
            TempData["SuccessMessage"] = "Se ha limpiado el plano espacial (Matriz y AVL).";
            return RedirectToAction("Index", "Home");
        }
    }
}



