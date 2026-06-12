# Ejemplo 10: Cliente HTTP y Comunicacion Inter-Proceso

## Sobre que va el ejemplo

Este ejemplo demuestra la implementacion y el consumo de servicios web tipo REST utilizando un cliente HTTP nativo (HttpClient) dentro de una aplicacion ASP.NET Core MVC (.NET 10.0). Se habilita un panel de control interactivo que permite realizar solicitudes HTTP GET de forma asincrona a endpoints que retornan datos en formato JSON, procesar la respuesta y presentar el resultado de forma directa en la interfaz grafica, ademas de auditar el suceso en la bitacora de auditoria.

El objetivo principal es ensenar a los estudiantes a establecer comunicacion inter-proceso (IPC) en red local, simulando la consulta o el envio de datos entre diferentes servidores o microservicios que formaran parte del ecosistema de su proyecto.

## Explicacion de lo que cambia con respecto al ejemplo anterior

Con respecto al Ejemplo 9 (que unicamente exponia endpoints de API REST), el Ejemplo 10 incorpora el rol de "Consumidor". Ahora la aplicacion no solo actua como servidor de API, sino que tambien es capaz de consumir APIs de forma dinamica mediante peticiones HTTP salientes controladas desde el backend.

A continuacion se detallan los archivos agregados y modificados:

### Archivos Nuevos Agregados

#### [HttpClienteController.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%2010/Controllers/HttpClienteController.cs)
* **Que se agrego**: Controlador especifico para gestionar peticiones HTTP salientes.
* **Como**: Define un campo estatico y de solo lectura de tipo HttpClient para evitar el agotamiento de sockets (socket exhaustion) y define la accion asincrona POST ConsultarApi(string urlDestino) para realizar llamadas asincronas GET.
* **Por que**: Centraliza y aisla la logica de peticiones de red salientes, registrando el progreso en la bitacora de auditoria y retornando la respuesta JSON cruda a traves de TempData.

### Archivos Modificados

#### [HomeController.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%2010/Controllers/HomeController.cs)
* **Que se cambio**: Recepcion de datos de respuesta del cliente HTTP.
* **Como**: Se modifico la accion Index para leer la variable TempData["ResultadoHttp"] y asignarla a ViewBag.ResultadoHttp para su renderizado en la interfaz.
* **Por que**: Permite persistir la respuesta JSON cruda a traves de la redireccion generada por el HttpClienteController al finalizar la peticion.

#### [Index.cshtml](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%2010/Views/Home/Index.cshtml)
* **Que se cambio**: Integracion de interfaz de usuario para el cliente HTTP.
* **Como**: Se agrego una tarjeta de formulario con un campo de texto para la URL de destino y un boton para invocar la consulta, ademas de una caja preformateada (pre) para renderizar el JSON de respuesta crudo cuando exista.
* **Por que**: Proporciona un mecanismo visual intuitivo para interactuar con el cliente HTTP y validar las respuestas sin salir de la aplicacion principal.

#### [Ejemplo_10.csproj](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%2010/Ejemplo_10.csproj)
* **Que se cambio**: Renombrado de assembly name y namespaces.
* **Como**: Se modificaron las referencias y nombres del proyecto para aislarlo como Ejemplo_10.
* **Por que**: Permite la compilacion independiente del compilador de .NET.

#### Resto de Clases (.cs y .cshtml) en `Models/`, `Services/`, `Views/Shared/` y `Program.cs`
* **Que se cambio**: Actualizacion del namespace.
* **Como**: Reemplazo de Ejemplo_9 por Ejemplo_10.
* **Por que**: Asegura la correcta vinculacion de tipos entre los archivos compilados del nuevo proyecto.

## Relacion con el Proyecto Unico 

Este ejemplo practico se relaciona de forma directa con los requerimientos de la **Fase 3** del proyecto:

### Que se esta resolviendo del proyecto
* **Seccion 7.1 (Comunicacion REST mediante HttpClient)**: Se demuestra como instanciar y utilizar un cliente de red asincrono en ASP.NET Core para consumir recursos web expuestos de forma local en otros puertos.
* **Seccion 9 (Guia Detallada de los Endpoints de la API)**: Se asientan las bases para el flujo transaccional donde una peticion entrante de API debe ser redirigida y procesada en caliente.

### ¿Como pueden aprovechar este ejemplo?
* **Arquitectura Multi-Puerto**: Los estudiantes deben desplegar dos servidores en los puertos 5000 y 5001. La logica implementada en el `HttpClienteController` les ensena como realizar peticiones salientes (`GetAsync` o `PostAsync`) desde el backend de una instancia para conectarse con la otra.
* **Serializacion y Deserializacion en Red**: Muestra como capturar la cadena JSON cruda de respuesta utilizando `ReadAsStringAsync` para luego procesarla o presentarla.
* **Manejo de Redireccion y Mensajes**: Explica como usar `TempData` para transferir respuestas de red asincronas a traves de redirecciones de controladores a vistas Razor para no perder la informacion ante un refresco de pantalla.

## Como se ejecuta

Para compilar y ejecutar esta aplicacion web localmente:

1. Abra una terminal de comandos y ubiquese en el directorio del proyecto "Ejemplo 10".
2. Compile el proyecto para comprobar que no existan errores:
   dotnet build
3. Ejecute la aplicacion con el comando:
   dotnet run --project "Ejemplo_10.csproj"
4. Abra su navegador en la direccion de escucha indicada (usualmente http://localhost:5022 o http://localhost:5000).
5. En la seccion "Cliente HTTP (Consumidor REST)", puede ingresar la URL de su propio endpoint de satelites para probar la integracion:
   http://localhost:5022/api/satelites
6. Presione "Consultar Endpoint". La aplicacion solicitara de forma asincrona el recurso, registrara la peticion en la bitacora y mostrara la respuesta JSON en la caja de texto inferior.
