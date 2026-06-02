# Ejemplo 2: Encapsulamiento y Relación de Objetos en C#


 Se expande sobre los conceptos del Ejemplo 1 al introducir **Encapsulamiento**, **Propiedades con Validación de Entrada**, y el modelado de una bitácora transaccional simple mediante la interacción entre múltiples clases.

---

##  Objetivos de la Sesión

Al finalizar este ejemplo, los estudiantes comprenderán:
1. **El Principio de Encapsulamiento**: Por qué ocultar el estado físico de los objetos declarando campos privados (`private`) en lugar de públicos (`public`).
2. **Propiedades de C# (`get` y `set`)**: La diferencia entre campos y propiedades, y cómo utilizar las propiedades para interceptar accesos y escrituras de datos.
3. **Validación de Datos en Descriptores de Acceso**: Cómo bloquear y rechazar asignaciones corruptas o vacías lanzando excepciones (`ArgumentException`) en los setters.
4. **Relación e Interacción de Objetos**: Cómo un objeto de la clase `Satelite` interactúa con el sistema principal de auditoría para generar reportes estructurados de eventos en memoria RAM.

---

##  Estructura del Proyecto

Esta aplicación de consola consta de los siguientes archivos principales:

* 📄 **`Ejemplo_2.csproj`**: Archivo de configuración del proyecto .NET Core 10.0.
* 📄 **`Satelite.cs`**: Clase satélite refactorizada con campos privados y propiedades lógicas que controlan y validan la asignación de nombres y direcciones IP.
* 📄 **`LogRegistro.cs`**: Clase que representa un nodo de auditoría en memoria (germen del TDA `LogAuditoria`).
* 📄 **`Program.cs`**: Orquestador que realiza simulaciones de transacciones de cambio de estado de los satélites (intentando inyectar tanto valores válidos como inválidos) y captura los errores para generar la bitácora de ejecución.

---

##  Conceptos Clave Explicados

### 1. ¿Por qué Encapsular?

En el Ejemplo 1, cualquier script podía modificar el nombre de un satélite a una cadena vacía o una dirección IP inválida simplemente haciendo `satelite.nombre = ""`.
El **encapsulamiento** protege la integridad del objeto restringiendo el acceso directo a sus variables internas y canalizándolo a través de propiedades públicas con reglas de negocio:

```csharp
public class Satelite
{
    // Campos privados: Nadie fuera de esta clase puede verlos o alterarlos directamente
    private string id;
    private string nombre = "";
    private string enlaceIP = "";

    // Propiedad Id: De sólo lectura (no posee descriptor 'set')
    public string Id 
    {
        get { return id; }
    }

    // Propiedad Nombre: Controla y valida la asignación
    public string Nombre
    {
        get { return nombre; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("El nombre del satélite no puede estar vacío.");
            }
            nombre = value;
        }
    }
}
```

Si un código externo intenta hacer `satelite.Nombre = ""`, el programa lanzará una excepción del tipo `ArgumentException`, impidiendo que el objeto quede en un estado corrupto o inconsistente.

### 2. La Clase de Auditoría (`LogRegistro.cs`)

Esta clase representa un evento ocurrido en el simulador. Para demostrar otra faceta del encapsulamiento, declaramos sus propiedades como **propiedades automáticas de sólo lectura** (`{ get; }`). De esta manera, una vez que el objeto `LogRegistro` se crea en el constructor, se vuelve completamente **inmutable** (no puede ser modificado posteriormente):

```csharp
public class LogRegistro
{
    public string Timestamp { get; }
    public string Tipo { get; }
    public string Mensaje { get; }

    public LogRegistro(string tipo, string mensaje)
    {
        Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        Tipo = tipo;
        Mensaje = mensaje;
    }
}
```

---

##  Instrucciones de Ejecución Local

Para compilar y ejecutar este ejemplo:

1. Ubícate en la raíz del repositorio.
2. Ejecuta en tu terminal el siguiente comando de .NET:

```bash
dotnet run --project ".\Ejemplo 2\Ejemplo_2.csproj"
```

### Salida Esperada en Pantalla

```text
============================================================
  EJEMPLO NUMERO 2: ENCAPSULACIÓN Y BITÁCORA EN MEMORIA
============================================================

[+] Estado Inicial de los satélites creados:
Satélite: Satelite 1 (ID: S001) -> Conectado a la IP: 127.0.0.1
Satélite: Satelite 2 (ID: S002) -> Conectado a la IP: 192.168.20.1
Satélite: Satelite 3 (ID: S003) -> Conectado a la IP: 124.123.54.2

[+] Iniciando transacciones y cambios de estado...
    [!] Intentando asignar un nombre vacío al Satélite 2...
    [!] Intentando asignar una dirección IP sin puntos al Satélite 3...

[+] Estado Final de los satélites modificados:
Satélite: Satelite 1 Modificado (ID: S001) -> Conectado a la IP: 127.10.0.1
Satélite: Satelite 2 Modificado (ID: S002) -> Conectado a la IP: 192.24.20.250
Satélite: Satelite 3 Modificado (ID: S003) -> Conectado a la IP: 124.0.0.99

============================================================
               BITÁCORA DE AUDITORÍA GLOBAL
============================================================
FECHA Y HORA        | TIPO  | DESCRIPCIÓN DEL EVENTO
------------------------------------------------------------
2026-06-02 12:03:24 | INFO  (OK)   | El sistema de simulación ha iniciado correctamente.
2026-06-02 12:03:24 | INFO  (OK)   | Se instanciaron 3 satélites base en memoria RAM.
2026-06-02 12:03:24 | INFO  (OK)   | Satélite [S001] actualizado a: Satelite 1 Modificado, IP: 127.10.0.1
2026-06-02 12:03:24 | ERROR (FAIL) | Fallo al actualizar Satélite [S002]: El nombre del satélite no puede estar vacío o contener solo espacios.
2026-06-02 12:03:24 | INFO  (OK)   | Satélite [S002] actualizado a: Satelite 2 Modificado, IP: 192.24.20.250
2026-06-02 12:03:24 | ERROR (FAIL) | Fallo al actualizar Satélite [S003]: La IP '124-000-00' es inválida. Debe contener formato IPv4 (ej. 192.168.1.1).
2026-06-02 12:03:24 | INFO  (OK)   | Satélite [S003] actualizado a: Satelite 3 Modificado, IP: 124.0.0.99
============================================================
```

Como se observa:
* El intento de asignar un nombre vacío a `S002` fue interceptado y rechazado por la propiedad, registrándose un log de tipo `ERROR`.
* El intento de asignar una IP sin puntos a `S003` fue rechazado de la misma manera, protegiendo el estado de los objetos.
* A pesar de los fallos, el simulador continuó y aplicó exitosamente el resto de transacciones correctas.
