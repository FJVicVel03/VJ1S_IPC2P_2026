using System;
using Ejemplo1;

Console.WriteLine("============================================================");
Console.WriteLine("  EJEMPLO 1 C#: INTRODUCCIÓN A LA POO - CLASES Y OBJETOS");
Console.WriteLine("============================================================");

// 1. Creación e instanciación de objetos de la clase Satelite
// Se crean dos instancias independientes en memoria RAM
Satelite sateliteA = new Satelite("SAT-ECU-0001", "Starlink-Norte-A", "127.0.0.1");
Satelite sateliteB = new Satelite("SAT-ECU-0002", "Starlink-Norte-B", "10.0.0.50");

Console.WriteLine("\n[+] Estado inicial de los satélites creados:");
Console.WriteLine(sateliteA.ObtenerDescripcion());
Console.WriteLine(sateliteB.ObtenerDescripcion());

// 2. Demostración de modificación directa de campos públicos
// Al ser campos públicos, se pueden leer y modificar directamente desde fuera de la clase
Console.WriteLine("\n[+] Modificando la dirección IP de red del satélite A...");
sateliteA.EnlaceIp = "192.168.1.100";

Console.WriteLine("[+] Modificando el nombre asignado al satélite B...");
sateliteB.Nombre = "Starlink-Norte-B-Modificado";

// 3. Mostrar el estado actualizado de los objetos
Console.WriteLine("\n[+] Estado final de los satélites después de las modificaciones:");
Console.WriteLine(sateliteA.ObtenerDescripcion());
Console.WriteLine(sateliteB.ObtenerDescripcion());
Console.WriteLine("============================================================\n");
