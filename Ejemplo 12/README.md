# Ejemplo 12: Seguridad de Red y Autenticacion HTTP Basic Authentication Manual

## Sobre que va el ejemplo

Este ejemplo demuestra la implementacion manual de la capa de seguridad de red requerida en la Seccion 7.2 del proyecto: Autenticacion HTTP Basica (HTTP Basic Authentication). Se construye un filtro de autorizacion personalizado a bajo nivel que intercepta las peticiones entrantes, lee la cabecera "Authorization", decodifica su contenido en Base64 utilizando metodos nativos de C# y valida las credenciales requeridas (usuario: orbitnet_admin, contrasena: USAC_ECYS_2026). 

Si la validacion es correcta, se permite el acceso al recurso y se registra el evento en la bitacora de auditoria. De lo contrario, se interrumpe la peticion de forma inmediata respondiendo con un codigo de estado HTTP 401 Unauthorized y un cuerpo JSON con el detalle de error. Ademas, se actualiza el panel del cliente HTTP (REST Consumer) para poder ingresar, codificar e inyectar estas credenciales de forma dinamica desde la interfaz grafica.

## Explicacion de lo que cambia con respecto al ejemplo anterior

Con respecto al Ejemplo 11, la aplicacion incorpora la capa de interceptacion de solicitudes y decodificacion de cabeceras de red. A continuacion se detallan los cambios estructurales:

### Archivos Nuevos Agregados

#### [BasicAuthorizeAttribute.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%2012/Attributes/BasicAuthorizeAttribute.cs)
* Atributo personalizado que hereda de Attribute e implementa IAuthorizationFilter de ASP.NET Core.
* Realiza la lectura del header "Authorization", verifica que empiece con la palabra "Basic ", remueve el prefijo, decodifica los bytes Base64 a string UTF-8, divide las credenciales usando el delimitador ":" y las valida contra los valores requeridos.
* Registra alertas en la bitacora manual (MemoriaPlano.Logs) para auditoria de seguridad y retorna un estado 401 con JsonResult si falla.

### Archivos Modificados

#### [ApiController.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%2012/Controllers/ApiController.cs)
* Se agrego el endpoint protegido "GET /api/seguro/satelites" decorado con el nuevo filtro "[BasicAuthorize]".
* Este endpoint permite a los estudiantes probar la diferencia entre consultar un endpoint publico (/api/satelites) y uno privado seguro.

#### [HttpClienteController.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%2012/Controllers/HttpClienteController.cs)
* Se modifico el metodo ConsultarApi para aceptar de forma opcional los parametros "usuario" y "contrasena".
* Utiliza un objeto local HttpRequestMessage para configurar de forma segura el header de autorizacion Basic sin causar colisiones en hilos concurrentes del cliente compartido.
* Captura explicitamente respuestas con codigo 401 Unauthorized para extraer su JSON de error y renderizar el payload de rechazo en la pantalla.

#### [HomeController.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%2012/Controllers/HomeController.cs)
* Se actualizaron los logs estaticos de inicializacion de memoria para reflejar que se encuentra activo el modulo de seguridad de red en la bitacora de auditoria.

#### [Index.cshtml](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%2012/Views/Home/Index.cshtml)
* Se cambio el panel del cliente HTTP REST agregando dos campos de texto ("Usuario REST" y "Contrasena REST") para simular peticiones con o sin credenciales, y con credenciales validas o invalidas.
* Se incluyo un enlace rapido al nuevo endpoint seguro en la lista de recursos REST del dashboard.

---

## Relacion con el Proyecto Unico

Este ejemplo resuelve y ensena a implementar la capa de seguridad requerida para la comunicacion entre nodos satelitales y estaciones terrestres:

### Que se esta resolviendo del proyecto
* Seccion 7.2 (Seguridad - Autenticacion HTTP Basica): Mecanismo manual para validar que las solicitudes externas de otros satelites/estaciones vengan autorizadas con cabeceras de red Base64 validas.

### Como pueden aprovechar este ejemplo
* Interceptacion de Headers: Enseña a interactuar con HttpContext.Request.Headers para inspeccionar metadatos de red antes de ejecutar las acciones de los controladores.
* Decodificacion Base64 Manual: Los estudiantes ven que no se requiere ninguna libreria pesada de autenticacion (como Identity o JWT) y que pueden utilizar System.Convert y System.Text.Encoding de forma rapida y pura en memoria RAM.
* Respuesta JSON de Error: Muestra como construir respuestas personalizadas usando JsonResult asignado a context.Result para interrumpir el ciclo de vida de una solicitud HTTP en ASP.NET Core de manera limpia.

---

## Como se ejecuta

Para compilar y ejecutar esta aplicacion web localmente:

1. Abra una terminal de comandos y ubiquese en el directorio del proyecto "Ejemplo 12".
2. Compile el proyecto para comprobar que no existan errores:
   dotnet build
3. Ejecute la aplicacion con el comando:
   dotnet run
4. Abra su navegador en la direccion de escucha indicada (usualmente http://localhost:5022).
5. En el panel "Cliente HTTP", intente consultar la URL "http://localhost:5022/api/seguro/satelites" sin ingresar usuario ni contrasena. Verifique que la bitacora registra una alerta de seguridad (ALERT) y que la pantalla muestra el JSON de error 401.
6. Intente la consulta ingresando credenciales invalidas.
7. Realice la consulta ingresando el usuario "orbitnet_admin" y la contrasena "USAC_ECYS_2026". Verifique que la peticion tiene exito, devuelve el arreglo JSON de satelites y se registra un log de exito (INFO).
