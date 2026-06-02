using System;

namespace Ejemplo2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("============================================================");
            Console.WriteLine("  EJEMPLO NUMERO 2: ENCAPSULACIÓN Y BITÁCORA EN MEMORIA");
            Console.WriteLine("============================================================");

            // 1. Creación e instanciación de objetos de la clase Satelite
            Satelite satelite1 = new Satelite("S001", "Satelite 1", "127.0.0.1");
            Satelite satelite2 = new Satelite("S002", "Satelite 2", "192.168.20.1");
            Satelite satelite3 = new Satelite("S003", "Satelite 3", "124.123.54.2");

            Console.WriteLine("\n[+] Estado Inicial de los satélites creados:");
            Console.WriteLine(satelite1.ObtenerDescripcion());
            Console.WriteLine(satelite2.ObtenerDescripcion());
            Console.WriteLine(satelite3.ObtenerDescripcion());
            Console.WriteLine();

            // 2. Registro histórico en memoria (TDA LogAuditoria básico)
            // Se utiliza un arreglo de tamaño fijo para respetar la prohibición de usar List<T> u otras colecciones dinámicas nativas
            LogRegistro[] bitacora = new LogRegistro[10];
            int contadorLogs = 0;

            // Función auxiliar local para agregar logs de forma segura al arreglo de tamaño fijo
            void RegistrarEvento(string tipo, string mensaje)
            {
                if (contadorLogs < bitacora.Length)
                {
                    bitacora[contadorLogs] = new LogRegistro(tipo, mensaje);
                    contadorLogs++;
                }
            }

            RegistrarEvento("INFO", "El sistema de simulación ha iniciado correctamente.");
            RegistrarEvento("INFO", "Se instanciaron 3 satélites base en memoria RAM.");

            // 3. Simulación de modificaciones y validaciones controladas por encapsulamiento
            Console.WriteLine("[+] Iniciando transacciones y cambios de estado...");

            // Transacción 1: Modificación Válida de Satélite 1
            try
            {
                satelite1.Nombre = "Satelite 1 Modificado";
                satelite1.EnlaceIP = "127.10.0.1";
                RegistrarEvento("INFO", $"Satélite [{satelite1.Id}] actualizado a: {satelite1.Nombre}, IP: {satelite1.EnlaceIP}");
            }
            catch (Exception ex)
            {
                RegistrarEvento("ERROR", $"Fallo al actualizar Satélite [{satelite1.Id}]: {ex.Message}");
            }

            // Transacción 2: Modificación Inválida (Nombre vacío) en Satélite 2
            try
            {
                Console.WriteLine("    [!] Intentando asignar un nombre vacío al Satélite 2...");
                satelite2.Nombre = "   "; // Generará ArgumentException
                RegistrarEvento("INFO", $"Satélite [{satelite2.Id}] actualizado.");
            }
            catch (ArgumentException ex)
            {
                RegistrarEvento("ERROR", $"Fallo al actualizar Satélite [{satelite2.Id}]: {ex.Message}");
            }

            // Transacción 3: Modificación Válida de Satélite 2 (IP válida)
            try
            {
                satelite2.Nombre = "Satelite 2 Modificado";
                satelite2.EnlaceIP = "192.24.20.250";
                RegistrarEvento("INFO", $"Satélite [{satelite2.Id}] actualizado a: {satelite2.Nombre}, IP: {satelite2.EnlaceIP}");
            }
            catch (Exception ex)
            {
                RegistrarEvento("ERROR", $"Fallo al actualizar Satélite [{satelite2.Id}]: {ex.Message}");
            }

            // Transacción 4: Modificación Inválida (IP sin puntos) en Satélite 3
            try
            {
                Console.WriteLine("    [!] Intentando asignar una dirección IP sin puntos al Satélite 3...");
                satelite3.EnlaceIP = "124-000-00"; // Generará ArgumentException
                RegistrarEvento("INFO", $"Satélite [{satelite3.Id}] actualizado.");
            }
            catch (ArgumentException ex)
            {
                RegistrarEvento("ERROR", $"Fallo al actualizar Satélite [{satelite3.Id}]: {ex.Message}");
            }

            // Transacción 5: Modificación Válida de Satélite 3
            try
            {
                satelite3.Nombre = "Satelite 3 Modificado";
                satelite3.EnlaceIP = "124.0.0.99";
                RegistrarEvento("INFO", $"Satélite [{satelite3.Id}] actualizado a: {satelite3.Nombre}, IP: {satelite3.EnlaceIP}");
            }
            catch (Exception ex)
            {
                RegistrarEvento("ERROR", $"Fallo al actualizar Satélite [{satelite3.Id}]: {ex.Message}");
            }

            // 4. Mostrar el estado final de los satélites
            Console.WriteLine("\n[+] Estado Final de los satélites modificados:");
            Console.WriteLine(satelite1.ObtenerDescripcion());
            Console.WriteLine(satelite2.ObtenerDescripcion());
            Console.WriteLine(satelite3.ObtenerDescripcion());
            Console.WriteLine();

            // 5. Imprimir la bitácora de auditoría
            Console.WriteLine("============================================================");
            Console.WriteLine("               BITÁCORA DE AUDITORÍA GLOBAL");
            Console.WriteLine("============================================================");
            Console.WriteLine($"{"FECHA Y HORA",-19} | {"TIPO",-5} | {"DESCRIPCIÓN DEL EVENTO"}");
            Console.WriteLine("------------------------------------------------------------");
            for (int i = 0; i < contadorLogs; i++)
            {
                Console.WriteLine(bitacora[i].ObtenerLineaFormateada());
            }
            Console.WriteLine("============================================================\n");
        }
    }
}
