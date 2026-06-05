# Ejemplo 5: Validaciones RegEx y Carga Transaccional Atómica

## Sobre qué va el ejemplo

Este ejemplo muestra cómo implementar validaciones de datos utilizando expresiones regulares en C# y cómo programar un cargador de datos transaccional y atómico (con comportamiento de commit y rollback) en una aplicación web de ASP.NET Core MVC. El sistema lee un archivo XML y valida los identificadores de los satélites y sus direcciones IP mediante expresiones regulares. Si todos los registros son conformes, se consolidan en la memoria RAM del servidor; si al menos uno falla la validación, la carga completa se aborta, registrando los eventos correspondientes en un TDA de auditoría (TDA LogAuditoria) implementado de forma manual.

## Explicación de lo que cambia con respecto al ejemplo anterior

A continuación se detallan las modificaciones y adiciones de archivos con respecto al Ejemplo 4, detallando qué se cambió/agregó, cómo, por qué y en dónde se ubican:

### Archivos Modificados

#### [HomeController.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%205/Controllers/HomeController.cs)
* **Qué se cambió**: Se agregaron constantes de expresiones regulares, validaciones lógicas de formatos y el control transaccional atómico.
* **Cómo**: Se declararon expresiones regulares para verificar el ID (`^SAT-(ECU|POL)-\d{4}$`) y la IP. En el método CargarXml, los satélites validados con Regex.IsMatch se acumulan en un TDA temporal (ListaSatelites). Si no ocurren fallos, se vuelcan a la base principal (Commit) y se registra un log INFO; si algún nodo falla, la carga se aborta (Rollback), la base principal no se modifica y se registra un log de tipo ERROR.
* **Por qué**: Para asegurar la consistencia física y lógica del simulador en memoria RAM, impidiendo que datos inválidos o cargas parciales corrompan el sistema.

#### [Index.cshtml](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%205/Views/Home/Index.cshtml)
* **Qué se cambió**: Se actualizó el modelo de datos receptor y la estructura de la página principal.
* **Cómo**: Se modificó la directiva de modelo para apuntar a DashboardViewModel. Se agregaron alertas de mensajes del controlador y un nuevo bloque de tabla HTML responsivo para iterar y mostrar en pantalla los eventos almacenados en el TDA de auditoría.
* **Por qué**: Para permitir al usuario observar en tiempo real el resultado detallado de las transacciones (Commit/Rollback) y auditar los fallos.

---

### Nuevos Archivos Agregados

#### [LogRegistro.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%205/Models/LogRegistro.cs)
* **Qué se agregó**: La clase modelo que encapsula un único registro o entrada de auditoría.
* **Cómo**: Se crearon propiedades de sólo lectura para almacenar el Timestamp, el Tipo de evento (INFO, ERROR) y el Mensaje descriptivo, asignadas únicamente a través de su constructor.
* **Por qué**: Para servir como la estructura de datos base e inmutable que almacena la información de cada evento en la bitácora.

#### [NodoLog.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%205/Models/NodoLog.cs)
* **Qué se agregó**: La clase nodo autorreferenciada para la estructura física de la bitácora.
* **Cómo**: Se definió un campo para almacenar el valor de tipo LogRegistro y una propiedad de referencia "Siguiente" que apunta al próximo NodoLog en memoria RAM.
* **Por qué**: Para conformar los eslabones individuales enlazados por referencias que estructuran la lista simple manual.

#### [ListaLogs.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%205/Models/ListaLogs.cs)
* **Qué se agregó**: El TDA manual LogAuditoria que gestiona la secuencia dinámica de logs en memoria RAM.
* **Cómo**: Se programaron los métodos InsertarAlFinal, Registrar, Limpiar y el generador Recorrer (usando yield return) para iterar las referencias de forma secuencial sin colecciones nativas de .NET.
* **Por qué**: Para dar soporte persistente a la bitácora de auditoría histórica requerida por la especificación del proyecto.

#### [DashboardViewModel.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%205/Models/DashboardViewModel.cs)
* **Qué se agregó**: Modelo de vista compuesto para unificar datos.
* **Cómo**: Se definieron dos propiedades públicas automáticas: Satelites (de tipo ListaSatelites) y Logs (de tipo ListaLogs).
* **Por qué**: Para transferir simultáneamente ambos TDAs desde el controlador hacia la misma vista Razor de inicio de manera estructurada.

#### [datos_validos.xml](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%205/wwwroot/datos_validos.xml) y [datos_invalidos.xml](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%205/wwwroot/datos_invalidos.xml)
* **Qué se agregó**: Archivos XML estructurados de configuración.
* **Cómo**: datos_validos.xml contiene satélites con formato correcto; datos_invalidos.xml contiene un satélite con ID erróneo (SAT-ECU-XYZ1).
* **Por qué**: Servir como insumos inmediatos para comprobar de forma práctica el comportamiento de Commit y Rollback en la simulación.

---

## Cómo se ejecuta

Para compilar y ejecutar el servidor web local de la aplicación:

1. Abra una terminal de comandos en el directorio raíz del repositorio.
2. Ejecute el siguiente comando de la interfaz de línea de comandos de .NET:
   dotnet run --project ".\Ejemplo 5\Ejemplo_5.csproj"
3. Abra su navegador web e ingrese a la dirección IP local indicada por la terminal (usualmente http://localhost:5000).
4. Utilice la interfaz web para descargar los archivos de prueba "datos_validos.xml" y "datos_invalidos.xml".
5. Suba el archivo "datos_invalidos.xml" para comprobar el comportamiento de aborto de carga (Rollback): se observará que la lista de satélites no cambia y se añade un registro de error detallado en la bitácora de auditoría.
6. Suba el archivo "datos_validos.xml" para observar la inserción de datos (Commit) y el registro del evento de éxito.
