# Ejemplo 13: TDA Buffer de Mensajes (ABB como Cola de Prioridad)

## Sobre que va el ejemplo

Este ejemplo demuestra la implementacion de un buffer satelital de procesamiento no lineal para almacenar paquetes de datos utilizando un Arbol Binario de Busqueda (ABB) que actua como una Cola de Prioridad. Cada satelite enlazado en la Matriz Dispersa posee su propio buffer interno.

La prioridad de los mensajes se clasifica con valores enteros del 1 al 5. La estructura del ABB se ordena de modo que las prioridades mayores o iguales que el nodo actual se ubiquen en su subarbol derecho, y las prioridades estrictamente menores se ubiquen en el subarbol izquierdo. De esta forma, el paquete con el nivel de prioridad mas alto siempre se localiza en la extrema derecha del arbol. Al desencolar un paquete, se remueve este nodo mas a la derecha y se reestructura el arbol reconectando sus punteros.

Ademas, se implementa una interfaz interactiva en el Dashboard para encolar paquetes en un satelite especifico y observar en tiempo real la cola de prioridad resultante generada a partir de un recorrido recursivo in-orden inverso.

## Explicacion de lo que cambia con respecto al ejemplo anterior

Con respecto al Ejemplo 12, la aplicacion incorpora la gestion dinamica de colas de prioridad en memoria. A continuacion se detallan los cambios estructurales:

### Archivos Nuevos Agregados

#### [AbbNode.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%2013/Models/AbbNode.cs)
* Modela un paquete de datos (nodo de arbol) que contiene propiedades como HexCode, EmisorId, DestIp, Priority (1 al 5), Content y punteros autorreferenciados Left y Right.

#### [BufferMensajesAbb.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%2013/Models/BufferMensajesAbb.cs)
* Estructura del Arbol Binario de Busqueda que maneja las operaciones de red.
* Enqueue: Inserta recursivamente segun la prioridad.
* Dequeue: Extrae y retorna el elemento mas a la derecha. Implementa la eliminacion de un nodo del ABB (puenteando el nodo y subiendo su hijo izquierdo).
* ObtenerMensajesOrdenados: Genera recursivamente un recorrido in-orden inverso para retornar los elementos como un arreglo plano nativo AbbNode[] ordenados de mayor a menor prioridad.

#### [BufferController.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%2013/Controllers/BufferController.cs)
* Controlador que expone las acciones POST para "Encolar" y "Desencolar" mensajes asociados a un satelite de la matriz dispersa. Registra todos los tránsitos y denegaciones en la bitacora de auditoria.

### Archivos Modificados

#### [MatrixNode.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%2013/Models/MatrixNode.cs)
* Se agrego el atributo `Buffer` de tipo `BufferMensajesAbb` para que cada nodo del plano satelital cuente con su propia esclusa de red local en memoria.

#### [HomeController.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%2013/Controllers/HomeController.cs)
* Se actualizo la firma de la accion `Index` para aceptar un parametro opcional `sateliteId` y pasarlo a ViewBag. Esto permite mantener seleccionado el satelite actual en la interfaz tras recargar la pagina.
* Se actualizaron los logs estaticos de inicializacion.

#### [Index.cshtml](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%2013/Views/Home/Index.cshtml)
* Se agrego una nueva Fila 3 en el Dashboard con controles interactivos para seleccionar un satelite, encolar mensajes con diferentes prioridades y desencolar el mensaje prioritario en caliente.

---

## Relacion con el Proyecto Unico

Este ejemplo resuelve la logica de encolamiento y despacho de transacciones en cada uno de los nodos de red:

### Que se esta resolviendo del proyecto
* Seccion 5.3 (TDA BufferMensajes: Arbol Binario de Busqueda como Cola de Prioridad): Desarrollo de la estructura de datos no lineal, logica de encolado, logica de despacho y recorrido recursivo sin colecciones genericas de .NET.

### Como pueden aprovechar este ejemplo
* Despacho por Prioridad: El ejemplo ensena como buscar y extraer fisicamente el nodo de extrema derecha del ABB reconectando su rama izquierda, lo cual corresponde exactamente al metodo Dequeue del proyecto.
* Trazo de la Cola sin Genericos: Los estudiantes ven que para retornar elementos de un arbol recursivo en un arreglo nativo, pueden hacer una primera pasada recursiva para contar la cantidad de elementos y dimensionar el arreglo, y una segunda pasada para poblarlo de forma ordenada.

---

## Como se ejecuta

Para compilar y ejecutar esta aplicacion web localmente:

1. Abra una terminal de comandos y ubiquese en el directorio del proyecto "Ejemplo 13".
2. Compile el proyecto para comprobar que no existan errores:
   dotnet build
3. Ejecute la aplicacion con el comando:
   dotnet run
4. Abra su navegador en la direccion de escucha indicada (usualmente http://localhost:5023).
5. Cargue el archivo XML de prueba `datos_completos.xml` para poblar el plano satelital.
6. En el panel de "Controles del Buffer", seleccione un satelite de la lista (ej. Starlink-Norte-A).
7. Rellene el formulario para encolar varios mensajes:
   * Mensaje 1 (Prioridad 2 - Normal)
   * Mensaje 2 (Prioridad 5 - Alerta Critica)
   * Mensaje 3 (Prioridad 4 - Alta)
8. Verifique en el panel derecho que la cola muestra los mensajes ordenados de forma prioritaria descendente (5, luego 4 y finalmente 2).
9. Haga clic en el boton "Desencolar Maxima Prioridad". Verifique que el primer mensaje extraido es el Mensaje 2 (Prioridad 5), y que al presionar el boton de nuevo se extrae el Mensaje 3 (Prioridad 4).
10. Observe que todas estas operaciones escriben en tiempo real en la bitacora de auditoria de la consola inferior.
