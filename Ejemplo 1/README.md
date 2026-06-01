# Ejemplo 1 C#: Introducción a la Programación Orientada a Objetos (POO)

---

##  Objetivos de la Sesión

Al finalizar este ejemplo, los estudiantes comprenderán:
1. **Qué es una Clase**: El plano estructural o plantilla lógica (`Satelite`) que define las características y el comportamiento de un objeto.
2. **Qué es un Objeto / Instancia**: La materialización real en memoria RAM del plano lógico definido por la clase.
3. **El Constructor**: El método especial (`public Satelite(...)`) ejecutado automáticamente para inicializar el estado del objeto en el momento de su creación.
4. **Campos Públicos**: Variables de instancia accesibles y modificables de forma directa desde cualquier script externo mediante el operador de punto (`objeto.campo`).

---

##  Estructura de Archivos del Proyecto

Esta aplicación de consola en .NET 8 consta de los siguientes archivos:

* 📄 **`Ejemplo_1.csproj`**: Archivo de configuración del proyecto .NET que define que es un ejecutable (`Exe`) bajo el entorno `net8.0`.
* 📄 **`Satelite.cs`**: Declaración de la clase `Satelite` con campos públicos y su respectivo constructor.
* 📄 **`Program.cs`**: Punto de entrada del programa. Utiliza la sintaxis simplificada *Top-Level Statements* para instanciar, modificar e imprimir los objetos satélites.

---

##  Conceptos Clave Explicados

### 1. Declaración de la Clase y Constructor (`Satelite.cs`)

En C#, declaramos la clase con la palabra clave `class`. Los atributos públicos en su nivel más básico se declaran directamente como variables con el modificador de acceso `public`:

```csharp
namespace Ejemplo1
{
    public class Satelite
    {
        // Campos públicos
        public string Id;
        public string Nombre;
        public string EnlaceIp;

        // Constructor
        public Satelite(string id, string nombre, string enlaceIp)
        {
            Id = id;
            Nombre = nombre;
            EnlaceIp = enlaceIp;
        }

        public string ObtenerDescripcion()
        {
            return $"Satélite: {Nombre} (ID: {Id}) -> Conectado a la IP: {EnlaceIp}";
        }
    }
}
```

### 2. Instanciación y Manipulación de Objetos (`Program.cs`)

Usamos la palabra clave `new` para llamar al constructor y crear el objeto en memoria RAM. Luego, podemos leer o reescribir sus variables directamente:

```csharp
using Ejemplo1;

// 1. Instanciación
Satelite satA = new Satelite("SAT-ECU-0001", "Starlink-Norte-A", "127.0.0.1");

// 2. Lectura
Console.WriteLine(satA.Nombre);  // Muestra: Starlink-Norte-A

// 3. Modificación directa
satA.EnlaceIp = "192.168.1.100";
```

---

##  Instrucciones de Ejecución Local

Para compilar y ejecutar este ejemplo en tu computadora local:

1. Abre tu terminal de comandos (PowerShell, Git Bash o CMD).
2. Asegúrate de estar posicionado en la carpeta raíz del repositorio.
3. Ejecuta el comando de compilación y ejecución de la CLI de .NET:

```bash
dotnet run --project ".\Ejemplo 1\Ejemplo_1.csproj"
```

### Salida Esperada en Pantalla

```text
============================================================
  EJEMPLO 1 C#: INTRODUCCIÓN A LA POO - CLASES Y OBJETOS
============================================================

[+] Estado inicial de los satélites creados:
Satélite: Starlink-Norte-A (ID: SAT-ECU-0001) -> Conectado a la IP: 127.0.0.1
Satélite: Starlink-Norte-B (ID: SAT-ECU-0002) -> Conectado a la IP: 10.0.0.50

[+] Modificando la dirección IP de red del satélite A...
[+] Modificando el nombre asignado al satélite B...

[+] Estado final de los satélites después de las modificaciones:
Satélite: Starlink-Norte-A (ID: SAT-ECU-0001) -> Conectado a la IP: 192.168.1.100
Satélite: Starlink-Norte-B-Modificado (ID: SAT-ECU-0002) -> Conectado a la IP: 10.0.0.50
============================================================
```

