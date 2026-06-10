# Ejemplo 8: Refactorización y Modularización de Controladores en ASP.NET Core MVC

## Sobre qué va el ejemplo

Este ejemplo se centra en la refactorización arquitectónica de la aplicación web construida en el Ejemplo 7. El objetivo principal es modularizar el controlador monolithic HomeController dividiéndolo en varios controladores independientes y especializados (HomeController, XmlController, SateliteController, LogsController) para mantener un diseño limpio, ordenado y extensible.

Para compartir de manera consistente los estados en memoria RAM de la Matriz Dispersa Ortogonal (TDA RedSatelitalPlano) y la bitácora de auditoría (TDA LogAuditoria) entre los distintos controladores, se introduce una clase de almacenamiento estático global (MemoriaPlano). Asimismo, se implementa la transferencia de alertas de éxito y error a través de redirecciones de controladores utilizando TempData de ASP.NET Core.

## Explicación de lo que cambia con respecto al ejemplo anterior

En este ejemplo no se introducen nuevas estructuras de datos abstractas (TDAs). Se realiza una reorganización arquitectónica del código para seguir las mejores prácticas de la división de responsabilidades del patrón MVC. Se elimina la persistencia de datos directa como campos estáticos en el controlador y se traslada a una clase estática de solo lectura compartida.

A continuación se detallan los archivos agregados y modificados con respecto al Ejemplo 7:

### Archivos Nuevos Agregados

#### [MemoriaPlano.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%208/Models/MemoriaPlano.cs)
* **Qué se agregó**: Clase estática global de estado.
* **Cómo**: Almacena de forma estática y de solo lectura la instancia de la Matriz Dispersa (`Matriz`) y de la bitácora de logs (`Logs`).
* **Por qué**: Para permitir que múltiples controladores independientes lean y manipulen el mismo estado físico en memoria RAM.

#### [XmlController.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%208/Controllers/XmlController.cs)
* **Qué se agregó**: Controlador especializado en la carga masiva y parseo de archivos XML.
* **Cómo**: Hereda de `Controller`. Contiene la acción POST `CargarXml`, los patrones sintácticos RegEx oficiales de validación y la lógica de procesamiento transaccional atómica (cabeza temporal, commit, rollback). Guarda el resultado de la transacción en `TempData` y redirige a `Home/Index`.
* **Por qué**: Para remover la lógica pesada de archivos e ingesta XML de HomeController, dejando cada controlador enfocado en una sola responsabilidad.

#### [SateliteController.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%208/Controllers/SateliteController.cs)
* **Qué se agregó**: Controlador especializado en la gestión directa de satélites dentro de la Matriz Dispersa.
* **Cómo**: Hereda de `Controller`. Contiene las acciones POST `InsertarNodo` (validaciones de colisión y ID/IP con RegEx), `EliminarNodo` (reconexión ortogonal de punteros vecinos) y `LimpiarMatriz` (purga del plano en RAM). Usa `TempData` para transferir alertas de retroalimentación y redirige a `Home/Index`.
* **Por qué**: Para aislar la manipulación manual de nodos de la visualización general de la aplicación.

#### [LogsController.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%208/Controllers/LogsController.cs)
* **Qué se agregó**: Controlador especializado en auditoría y logs.
* **Cómo**: Hereda de `Controller`. Contiene la acción POST `LimpiarLogs` que invoca a `MemoriaPlano.Logs.Limpiar()` y registra el evento inicial. Redirige a `Home/Index`.
* **Por qué**: Para encapsular la administración de la bitácora histórica.

### Archivos Modificados

#### [HomeController.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%208/Controllers/HomeController.cs)
* **Qué se cambió**: Reducción drástica del controlador.
* **Cómo**: Se eliminaron los campos estáticos de matriz/logs y todas las acciones POST de inserción, borrado, XML y purgas. Ahora solo expone la acción GET `Index` la cual lee el estado de `MemoriaPlano`, compila el código DOT a SVG, y mapea los mensajes de `TempData` a `ViewBag` para renderizar el panel.
* **Por qué**: Actúa únicamente como el enrutador de presentación del Dashboard principal.

#### [Index.cshtml](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%208/Views/Home/Index.cshtml)
* **Qué se cambió**: Se reconfiguraron las rutas de envío de los formularios.
* **Cómo**: Se agregaron atributos `asp-controller` a cada etiqueta `<form>` para dirigir las peticiones POST a los controladores correspondientes:
  - Formulario XML apunta a `XmlController`
  - Formularios de inserción, eliminación y purga de matriz apuntan a `SateliteController`
  - Formulario de purga de logs apunta a `LogsController`
* **Por qué**: Para sincronizar la interfaz web con la nueva arquitectura modular de controladores.

#### [Ejemplo_8.csproj](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%208/Ejemplo_8.csproj)
* **Qué se cambió**: Nombre del proyecto e inicialización de compilation assembly.
* **Cómo**: Renombrado de Ejemplo_7.csproj a Ejemplo_8.csproj.
* **Por qué**: Para aislar el proyecto y namespaces del compilador de .NET.

#### Resto de Clases (.cs y .cshtml) en `Models/`, `Services/`, `Views/Shared/` y `Program.cs`
* **Qué se cambió**: Actualización del espacio de nombres (Namespace).
* **Cómo**: Reemplazo masivo de `namespace Ejemplo_7` y directivas `using Ejemplo_7` por `Ejemplo_8`.
* **Por qué**: Para mantener la consistencia y correcta resolución de referencias del nuevo ensamblado.

## Cómo se ejecuta

Para compilar e iniciar la aplicación web en su entorno de desarrollo local:

1. Abra una terminal de comandos y ubíquese en el directorio del proyecto `Ejemplo 8`.
2. Restaure y compile el proyecto con el comando:
   dotnet build
3. Ejecute la aplicación con el comando:
   dotnet run --project "Ejemplo_8.csproj"
4. Abra su navegador web y diríjase a la URL de escucha local indicada en la consola (usualmente http://localhost:5022 o http://localhost:5000).
5. Graphviz debe encontrarse instalado en el sistema. El servicio GraphvizCompilador resolverá dinámicamente la ruta en ubicaciones típicas de Windows (`C:\Program Files\Graphviz\bin\dot.exe`), por lo que no es estrictamente obligatorio agregarlo al PATH si se usa una instalación por defecto.
