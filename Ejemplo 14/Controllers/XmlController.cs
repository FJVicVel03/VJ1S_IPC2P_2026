using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ejemplo_14.Models;

namespace Ejemplo_14.Controllers
{
    /// <summary>
    /// Controlador especializado en el procesamiento e ingesta transaccional de archivos XML.
    /// Soporta por completo la estructura oficial del Proyecto Unico.
    /// </summary>
    public class XmlController : Controller
    {
        // Expresiones Regulares Oficiales del Proyecto
        private const string PatronIdSatelite = @"^SAT-(ECU|POL)-\d{4}$";
        private const string PatronIdAntena = @"^ANT-[A-Z]{3}-\d{3,4}$";
        private const string PatronIpv4 = @"^(?:(?:25[0-5]|2[0-4]\d|[01]?\d?\d)\.){3}(?:25[0-5]|2[0-4]\d|[01]?\d?\d)$";
        private const string PatronCoordenadas = @"^-?\d{1,2}\.\d{4,6},-?\d{1,3}\.\d{4,6}$";

        // Estructura lineal temporal para satelites y antenas que iran a la Matriz Dispersa
        private class NodoMatrizTemporal
        {
            public int Fila { get; }
            public int Columna { get; }
            public string Id { get; }
            public string Nombre { get; }
            public string IpAddress { get; }
            public NodoMatrizTemporal? Siguiente { get; set; }

            public NodoMatrizTemporal(int fila, int col, string id, string nombre, string ip)
            {
                Fila = fila;
                Columna = col;
                Id = id;
                Nombre = nombre;
                IpAddress = ip;
                Siguiente = null;
            }
        }

        // Estructura lineal temporal para satelites polares que iran al Catalogo AVL
        private class NodoAvlTemporal
        {
            public string Id { get; }
            public string Nombre { get; }
            public double Frecuencia { get; }
            public NodoAvlTemporal? Siguiente { get; set; }

            public NodoAvlTemporal(string id, string nombre, double frecuencia)
            {
                Id = id;
                Nombre = nombre;
                Frecuencia = frecuencia;
                Siguiente = null;
            }
        }

        [HttpPost]
        public IActionResult CargarXml(IFormFile archivoXml)
        {
            if (archivoXml == null || archivoXml.Length == 0)
            {
                string msgErr = "Por favor, seleccione un archivo XML valido.";
                MemoriaPlano.Logs.Registrar("ALERT", msgErr);
                TempData["ErrorMessage"] = msgErr;
                return RedirectToAction("Index", "Home");
            }

            MemoriaPlano.Logs.Registrar("INFO", $"Iniciando ingesta de archivo XML: '{archivoXml.FileName}'");

            // Cabezas de listas temporales para la transaccion
            NodoMatrizTemporal? cabezaMatriz = null;
            NodoAvlTemporal? cabezaAvl = null;
            
            int insertadosMatriz = 0;
            int insertadosAvl = 0;
            bool transaccionExitosa = true;
            string causaFallo = "";

            try
            {
                // OWASP: Mitigar vulnerabilidad XXE (Inyeccion de Entidades Externas)
                XmlReaderSettings settings = new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Prohibit,
                    XmlResolver = null
                };

                using (Stream stream = archivoXml.OpenReadStream())
                using (XmlReader reader = XmlReader.Create(stream, settings))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(reader);

                    // 1. Procesar Satelites Ecuatoriales (van a la Matriz en Fila = 0)
                    XmlNodeList satEcuatoriales = doc.SelectNodes("//constelaciones_ecuatoriales/satelite")!;
                    foreach (XmlNode sat in satEcuatoriales)
                    {
                        string? id = sat.Attributes?["id"]?.Value;
                        string? nombre = sat.SelectSingleNode("nombre")?.InnerText;
                        string? enlaceIp = sat.SelectSingleNode("enlace_ip")?.InnerText;

                        if (!ValidarYAgregarMatrizTemporal(0, id, nombre, enlaceIp, ref cabezaMatriz, ref insertadosMatriz, ref causaFallo))
                        {
                            transaccionExitosa = false;
                            break;
                        }
                    }

                    // 2. Procesar Satelites Polares (van al Catalogo AVL)
                    if (transaccionExitosa)
                    {
                        XmlNodeList satPolares = doc.SelectNodes("//orbitas_polares/polar/satelite")!;
                        foreach (XmlNode sat in satPolares)
                        {
                            string? id = sat.Attributes?["id"]?.Value;
                            string? nombre = sat.SelectSingleNode("nombre")?.InnerText;
                            string? freqStr = sat.SelectSingleNode("frecuencia")?.InnerText;

                            if (!ValidarYAgregarAvlTemporal(id, nombre, freqStr, ref cabezaAvl, ref insertadosAvl, ref causaFallo))
                            {
                                transaccionExitosa = false;
                                break;
                            }
                        }
                    }

                    // 3. Procesar Antenas Terrestres (van a la Matriz en Fila = Round(Lat), Col = Round(Long))
                    if (transaccionExitosa)
                    {
                        XmlNodeList antenas = doc.SelectNodes("//antenas_terrestres/antena")!;
                        foreach (XmlNode antena in antenas)
                        {
                            string? id = antena.Attributes?["id"]?.Value;
                            string? nombre = antena.SelectSingleNode("nombre")?.InnerText;
                            string? coordsStr = antena.SelectSingleNode("coordenadas")?.InnerText;
                            string? ipNodo = antena.SelectSingleNode("ip_nodo")?.InnerText;

                            if (!ValidarYAgregarAntenaTemporal(id, nombre, coordsStr, ipNodo, ref cabezaMatriz, ref insertadosMatriz, ref causaFallo))
                            {
                                transaccionExitosa = false;
                                break;
                            }
                        }
                    }
                }
            }
            catch (XmlException ex)
            {
                transaccionExitosa = false;
                causaFallo = $"Error de parsing XML: {ex.Message}";
            }
            catch (Exception ex)
            {
                transaccionExitosa = false;
                causaFallo = $"Error de procesamiento: {ex.Message}";
            }

            // Aplicar cambios (Commit) o abortar (Rollback)
            if (transaccionExitosa)
            {
                // COMMIT: Guardar datos a la memoria RAM de manera definitiva
                // A. Insertar nodos en la Matriz Dispersa
                NodoMatrizTemporal? actualMatriz = cabezaMatriz;
                while (actualMatriz != null)
                {
                    MemoriaPlano.Matriz.Insert(actualMatriz.Fila, actualMatriz.Columna, actualMatriz.Id, actualMatriz.Nombre, actualMatriz.IpAddress);
                    actualMatriz = actualMatriz.Siguiente;
                }

                // B. Insertar nodos en el Arbol AVL
                NodoAvlTemporal? actualAvl = cabezaAvl;
                while (actualAvl != null)
                {
                    Satelite nuevoSatelite = new Satelite(actualAvl.Id, actualAvl.Nombre, actualAvl.Frecuencia);
                    MemoriaPlano.Catalogo.Insertar(nuevoSatelite);
                    actualAvl = actualAvl.Siguiente;
                }

                string msgSucc = $"Carga XML exitosa (Commit). Se insertaron {insertadosMatriz} nodos en la Matriz Dispersa y {insertadosAvl} satelites en el Catalogo AVL.";
                MemoriaPlano.Logs.Registrar("INFO", msgSucc);
                TempData["SuccessMessage"] = msgSucc;
            }
            else
            {
                // ROLLBACK: No se altera ningun TDA en RAM
                string msgFallo = $"Transaccion XML abortada (Rollback). Causa: {causaFallo}. El plano y el catalogo permanecen intactos.";
                MemoriaPlano.Logs.Registrar("ERROR", msgFallo);
                TempData["ErrorMessage"] = msgFallo;
            }

            return RedirectToAction("Index", "Home");
        }

        // --- Validaciones y Logica de Transaccion ---

        private bool ValidarYAgregarMatrizTemporal(int fila, string? id, string? nombre, string? enlaceIp, ref NodoMatrizTemporal? cabeza, ref int contador, ref string causaFallo)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(enlaceIp))
            {
                causaFallo = "Atributos o elementos basicos incompletos para satelite ecuatorial.";
                return false;
            }

            id = id.Trim();
            nombre = nombre.Trim();
            enlaceIp = enlaceIp.Trim();

            // A. Validar ID con Regex
            if (!Regex.IsMatch(id, PatronIdSatelite))
            {
                causaFallo = $"El ID de satelite '{id}' no cumple con el formato requerido 'SAT-(ECU|POL)-0000'.";
                return false;
            }

            // B. Validar IP con Regex
            if (!Regex.IsMatch(enlaceIp, PatronIpv4))
            {
                causaFallo = $"La direccion IP '{enlaceIp}' para el satelite [{id}] es invalida.";
                return false;
            }

            // C. Determinar columna a partir del ID
            int columna;
            try
            {
                columna = int.Parse(id.Substring(8));
            }
            catch
            {
                causaFallo = $"No se pudo derivar la columna a partir del ID del satelite '{id}'.";
                return false;
            }

            // D. Comprobar colisiones en la Matriz RAM global
            if (MemoriaPlano.Matriz.Search(fila, columna) != null)
            {
                causaFallo = $"Colision en Matriz RAM: Ya existe un nodo en la posicion ({fila}, {columna}).";
                return false;
            }
            if (MemoriaPlano.Matriz.BuscarPorId(id) != null)
            {
                causaFallo = $"El ID de satelite '{id}' ya se encuentra registrado en el plano.";
                return false;
            }

            // E. Comprobar colisiones internas en la lista temporal de la carga activa
            NodoMatrizTemporal? actual = cabeza;
            while (actual != null)
            {
                if (actual.Fila == fila && actual.Columna == columna)
                {
                    causaFallo = $"Colision interna en XML: La coordenada ({fila}, {columna}) se declara mas de una vez.";
                    return false;
                }
                if (actual.Id.Equals(id, StringComparison.OrdinalIgnoreCase))
                {
                    causaFallo = $"ID duplicado en XML: '{id}' se declara mas de una vez.";
                    return false;
                }
                actual = actual.Siguiente;
            }

            // F. Agregar al buffer temporal
            NodoMatrizTemporal nuevo = new NodoMatrizTemporal(fila, columna, id, nombre, enlaceIp);
            if (cabeza == null)
            {
                cabeza = nuevo;
            }
            else
            {
                NodoMatrizTemporal temp = cabeza;
                while (temp.Siguiente != null)
                {
                    temp = temp.Siguiente;
                }
                temp.Siguiente = nuevo;
            }
            contador++;
            return true;
        }

        private bool ValidarYAgregarAvlTemporal(string? id, string? nombre, string? freqStr, ref NodoAvlTemporal? cabeza, ref int contador, ref string causaFallo)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(freqStr))
            {
                causaFallo = "Atributos o elementos incompletos para satelite polar.";
                return false;
            }

            id = id.Trim();
            nombre = nombre.Trim();
            freqStr = freqStr.Trim();

            // A. Validar ID con Regex
            if (!Regex.IsMatch(id, PatronIdSatelite))
            {
                causaFallo = $"El ID de satelite polar '{id}' no cumple con el formato requerido 'SAT-(ECU|POL)-0000'.";
                return false;
            }

            // B. Validar Frecuencia
            if (!double.TryParse(freqStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double frecuencia) || frecuencia <= 0)
            {
                causaFallo = $"La frecuencia '{freqStr}' del satelite [{id}] debe ser un numero decimal positivo.";
                return false;
            }

            // C. Comprobar duplicidad en el Catalogo AVL global
            if (MemoriaPlano.Catalogo.Buscar(id) != null)
            {
                causaFallo = $"El satelite polar '{id}' ya existe en el Catalogo AVL.";
                return false;
            }

            // D. Comprobar duplicidad interna en la lista temporal de la carga activa
            NodoAvlTemporal? actual = cabeza;
            while (actual != null)
            {
                if (actual.Id.Equals(id, StringComparison.OrdinalIgnoreCase))
                {
                    causaFallo = $"ID polar duplicado en XML: '{id}' se declara mas de una vez.";
                    return false;
                }
                actual = actual.Siguiente;
            }

            // E. Agregar al buffer temporal
            NodoAvlTemporal nuevo = new NodoAvlTemporal(id, nombre, frecuencia);
            if (cabeza == null)
            {
                cabeza = nuevo;
            }
            else
            {
                NodoAvlTemporal temp = cabeza;
                while (temp.Siguiente != null)
                {
                    temp = temp.Siguiente;
                }
                temp.Siguiente = nuevo;
            }
            contador++;
            return true;
        }

        private bool ValidarYAgregarAntenaTemporal(string? id, string? nombre, string? coordsStr, string? ipNodo, ref NodoMatrizTemporal? cabeza, ref int contador, ref string causaFallo)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(coordsStr) || string.IsNullOrWhiteSpace(ipNodo))
            {
                causaFallo = "Atributos o elementos incompletos para antena terrestre.";
                return false;
            }

            id = id.Trim();
            nombre = nombre.Trim();
            coordsStr = coordsStr.Trim();
            ipNodo = ipNodo.Trim();

            // A. Validar ID con Regex
            if (!Regex.IsMatch(id, PatronIdAntena))
            {
                causaFallo = $"El ID de antena '{id}' no cumple con el formato requerido 'ANT-[CODIGO]-[NUMERO]'.";
                return false;
            }

            // B. Validar IP con Regex
            if (!Regex.IsMatch(ipNodo, PatronIpv4))
            {
                causaFallo = $"La direccion IP de nodo '{ipNodo}' para la antena [{id}] es invalida.";
                return false;
            }

            // C. Validar Coordenadas con Regex
            if (!Regex.IsMatch(coordsStr, PatronCoordenadas))
            {
                causaFallo = $"Las coordenadas '{coordsStr}' de la antena [{id}] deben cumplir con el patron 'Latitud,Longitud' (ej. 14.5891,-90.5514).";
                return false;
            }

            // D. Parsear y redondear coordenadas
            int fila;
            int columna;
            try
            {
                string[] partes = coordsStr.Split(',');
                double latitud = double.Parse(partes[0], System.Globalization.CultureInfo.InvariantCulture);
                double longitud = double.Parse(partes[1], System.Globalization.CultureInfo.InvariantCulture);

                // Validacion logica de limites geograficos
                if (latitud < -90.0 || latitud > 90.0 || longitud < -180.0 || longitud > 180.0)
                {
                    causaFallo = $"Valores de coordenadas de la antena [{id}] exceden los limites geograficos validos (Latitud: -90 a 90, Longitud: -180 a 180).";
                    return false;
                }

                fila = (int)Math.Round(latitud);
                columna = (int)Math.Round(longitud);
            }
            catch (Exception ex)
            {
                causaFallo = $"Fallo al parsear las coordenadas de la antena [{id}]: {ex.Message}";
                return false;
            }

            // E. Comprobar colisiones en la Matriz RAM global
            if (MemoriaPlano.Matriz.Search(fila, columna) != null)
            {
                causaFallo = $"Colision en Matriz RAM para antena [{id}]: Ya existe un nodo en la posicion ({fila}, {columna}).";
                return false;
            }
            if (MemoriaPlano.Matriz.BuscarPorId(id) != null)
            {
                causaFallo = $"El ID de antena '{id}' ya se encuentra registrado en el plano.";
                return false;
            }

            // F. Comprobar colisiones internas en la lista temporal de la carga activa
            NodoMatrizTemporal? actual = cabeza;
            while (actual != null)
            {
                if (actual.Fila == fila && actual.Columna == columna)
                {
                    causaFallo = $"Colision interna en XML por antenas: La coordenada redondeada ({fila}, {columna}) se declara mas de una vez.";
                    return false;
                }
                if (actual.Id.Equals(id, StringComparison.OrdinalIgnoreCase))
                {
                    causaFallo = $"ID de antena duplicado en XML: '{id}' se declara mas de una vez.";
                    return false;
                }
                actual = actual.Siguiente;
            }

            // G. Agregar al buffer temporal
            NodoMatrizTemporal nuevo = new NodoMatrizTemporal(fila, columna, id, nombre, ipNodo);
            if (cabeza == null)
            {
                cabeza = nuevo;
            }
            else
            {
                NodoMatrizTemporal temp = cabeza;
                while (temp.Siguiente != null)
                {
                    temp = temp.Siguiente;
                }
                temp.Siguiente = nuevo;
            }
            contador++;
            return true;
        }
    }
}


