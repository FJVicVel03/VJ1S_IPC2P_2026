# Ejemplo 9: API REST básica y Exposición de Estados en JSON

## Sobre qué va el ejemplo

Este ejemplo demuestra la creación e integración de una API REST básica dentro de nuestra aplicación web basada en ASP.NET Core MVC (.NET 10.0). Se habilitan endpoints específicos para exponer el estado en tiempo real de las estructuras de datos almacenadas en la memoria RAM (la Matriz Dispersa Ortogonal y la bitácora de logs) utilizando formato JSON.

Para lograr una serialización limpia y segura en el endpoint de satélites, se introduce un Objeto de Transferencia de Datos plano (SateliteDto) que descarta los punteros autorreferenciados (Up, Down, Left, Right). Esto previene excepciones por referencias circulares (ciclos infinitos) al invocar el motor de serialización JSON.

## Explicación de lo que cambia con respecto al ejemplo anterior

Con respecto al Ejemplo 8, añadimos la capacidad de consumir los datos del simulador mediante peticiones HTTP que devuelven respuestas en formato JSON. Se agrega un controlador exclusivo para la API REST y un modelo DTO plano, y se añade soporte en la vista principal para acceder directamente a estos servicios.

A continuación se detallan los archivos agregados y modificados con respecto al Ejemplo 8:

### Archivos Nuevos Agregados

#### [SateliteDto.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%209/Models/SateliteDto.cs)
* **Qué se agregó**: Objeto de Transferencia de Datos (DTO) plano.
* **Cómo**: Almacena las propiedades básicas del satélite (Fila, Columna, Id, Nombre, IpAddress) utilizando tipos de datos primitivos de C#. Carece de punteros u objetos complejos autorreferenciados.
* **Por qué**: Para servir como estructura de datos lineal que el serializador JSON de .NET puede procesar directamente sin generar excepciones de recursividad infinita por ciclos lógicos de la matriz dispersa.

#### [ApiController.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%209/Controllers/ApiController.cs)
* **Qué se agregó**: Controlador web API REST.
* **Cómo**: Hereda de `Controller` y se le asigna la ruta base `/api`. Contiene la acción GET `ObtenerSatelites` (mapea los nodos físicos de la matriz dispersa a un arreglo nativo de `SateliteDto` y retorna JSON) y la acción GET `ObtenerLogs` (retorna el arreglo nativo de logs de auditoría en JSON).
* **Por qué**: Para desacoplar el canal de datos crudos (JSON) del canal de visualización de interfaz gráfica tradicional (HTML/Razor).

### Archivos Modificados

#### [Index.cshtml](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%209/Views/Home/Index.cshtml)
* **Qué se cambió**: Se agregaron enlaces directos de prueba para los endpoints REST.
* **Cómo**: En el panel lateral de "Administración de Datos", se insertó una sección con dos botones de tipo enlace HTML dirigidos a `/api/satelites` y `/api/logs` con el atributo `target="_blank"`.
* **Por qué**: Permite al usuario y al docente validar la serialización de datos directamente desde el navegador abriendo los recursos JSON en pestañas secundarias.

#### [Ejemplo_9.csproj](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%209/Ejemplo_9.csproj)
* **Qué se cambió**: Nombre del proyecto e inicialización de compilation assembly.
* **Cómo**: Renombrado de Ejemplo_8.csproj a Ejemplo_9.csproj.
* **Por qué**: Para aislar el proyecto y namespaces del compilador de .NET.

#### Resto de Clases (.cs y .cshtml) en `Models/`, `Services/`, `Views/Shared/` y `Program.cs`
* **Qué se cambió**: Actualización del espacio de nombres (Namespace).
* **Cómo**: Reemplazo de `namespace Ejemplo_8` y directivas `using Ejemplo_8` por `Ejemplo_9`.
* **Por qué**: Para mantener la resolución correcta de tipos y dependencias en el nuevo compilado.

## Cómo se ejecuta

Para compilar e iniciar la aplicación web en su entorno de desarrollo local:

1. Abra una terminal de comandos y ubíquese en el directorio del proyecto `Ejemplo 9`.
2. Restaure y compile el proyecto con el comando:
   dotnet build
3. Ejecute la aplicación con el comando:
   dotnet run --project "Ejemplo_9.csproj"
4. Abra su navegador web y diríjase a la URL de escucha local indicada en la consola (usualmente http://localhost:5022 o http://localhost:5000).
5. Interactúe con el simulador (Cargue XML o inserte satélites de forma manual).
6. Presione los botones de API REST en la interfaz o navegue directamente a:
   * **http://localhost:5022/api/satelites** para obtener el JSON de satélites activos en memoria.
   * **http://localhost:5022/api/logs** para obtener el JSON del historial de logs en memoria.
