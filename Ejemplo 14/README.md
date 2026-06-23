# Ejemplo 14: Enrutamiento Logico de Saltos Ortogonales (DFS Pathfinder en Matriz Dispersa)

## Sobre que va el ejemplo

Este ejemplo demuestra la implementacion de Enrutamiento Logico (Logical Routing) a traves de la malla de la Matriz Dispersa utilizando un algoritmo de busqueda en profundidad (DFS) recursivo con backtracking, sin hacer uso de colecciones genericas de .NET (System.Collections o System.Collections.Generic).

El enrutador fisico busca un camino de conexion entre un nodo de origen y un nodo de destino desplazandose unicamente a traves de los punteros directos del nodo actual (Up, Down, Left, Right). Adicionalmente, el camino resultante es resaltado en verde brillante en el diagrama de Graphviz compilado en caliente, engrosando los bordes/enlaces que forman parte de la ruta.

La interfaz grafica en el Dashboard ahora permite seleccionar un satelite/antena origen y un satelite/antena destino, trazar la ruta en tiempo real, visualizar los saltos secuenciales en un listado detallado y limpiar el trazado para restablecer la vista.

## Explicacion de lo que cambia con respecto al ejemplo anterior

Con respecto al Ejemplo 13, la aplicacion incorpora la busqueda dinamica de rutas logicas y el renderizado condicional de grafos en Graphviz. A continuacion se detallan los cambios estructurales:

### Archivos Nuevos Agregados

#### [EnrutadorOrtogonal.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%2014/Services/EnrutadorOrtogonal.cs)
* Implementa la busqueda del camino mas corto de saltos ortogonales (DFS) usando backtracking.
* Emplea un arreglo estatico para rastrear los nodos ya visitados durante la recursion para evitar ciclos infinitos, eliminando la necesidad de Listas genericas de .NET.
* Retorna un arreglo de tipo MatrixNode[] con la secuencia ordenada de saltos desde el origen hasta el destino si el camino existe.

#### [RouteController.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%2014/Controllers/RouteController.cs)
* Controlador encargado de exponer las acciones POST "Trazar" y "Limpiar".
* Invoca al enrutador ortogonal para calcular la ruta y guarda el resultado en el estado estatico global para su renderizado.
* Registra los resultados de exito o fallas por malla rota en la bitacora de auditoria.

### Archivos Modificados

#### [MemoriaPlano.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%2014/Models/MemoriaPlano.cs)
* Se agrego la propiedad estatica RutaActiva (de tipo MatrixNode[]) para conservar en memoria la ruta calculada actualmente seleccionada.

#### [RedSatelitalPlano.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%2014/Models/RedSatelitalPlano.cs)
* Se sobrecargo/actualizo el metodo GenerarCodigoDot para aceptar un parametro opcional de ruta. Si el parametro esta presente, los nodos pertenecientes a la ruta se pintan de verde brillante (#2ECC71) y los enlaces direccionales activos de la ruta se grafican en verde oscuro con un grosor mayor (penwidth=3.0).

#### [HomeController.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%2014/Controllers/HomeController.cs)
* Inyecta la RutaActiva guardada en MemoriaPlano a la generacion del codigo DOT antes de compilar a SVG, de modo que el mapa de memoria se redibuje con el camino resaltado.

#### [Index.cshtml](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%2014/Views/Home/Index.cshtml)
* Se modifico el Dashboard para integrar un panel de control dedicado al "Enrutamiento de Red".
* Permite seleccionar el origen y el destino de los nodos activos en la Matriz.
* Muestra de forma dinamica la secuencia ordenada de saltos con su respectiva identificacion y coordenadas.

---

## Relacion con el Proyecto Unico

Este ejemplo resuelve directamente la logica de direccionamiento y retransmision de paquetes de datos a traves de la topologia fisica de la red satelital:

### Que se esta resolviendo del proyecto
* Seccion 6 (Enrutamiento de Mensajes): Logica de retransmision y calculo de saltos ortogonales (direccionamiento a traves de punteros directos de la matriz en lugar de calculos geometricos de distancia de coordenadas).
* Seccion de Reportes de Graphviz: Visualizacion del camino recorrido pintando los enlaces y nodos en colores especificos para evidenciar la ruta de transmision fisica.

### Como pueden aprovechar este ejemplo
* Algoritmo de Backtracking: Los estudiantes pueden observar la plantilla limpia de recursion con backtracking para buscar caminos en una matriz dispersa usando punteros (Right, Left, Down, Up) y controlando la no-repeticion de nodos visitados mediante un arreglo nativo.
* Resaltado Dinamico en DOT: El proyecto requiere reportar la ruta de transmision de un mensaje. Este ejemplo provee la logica exacta para condicionar la creacion del string del grafo DOT segun la ruta, modificando atributos de color y grosor de linea.

---

## Como se ejecuta

Para compilar y ejecutar esta aplicacion web localmente:

1. Abra una terminal de comandos y ubiquese en el directorio del proyecto "Ejemplo 14".
2. Compile el proyecto para comprobar que no existan errores:
   dotnet build
3. Ejecute la aplicacion con el comando:
   dotnet run
4. Abra su navegador en la direccion de escucha indicada (usualmente http://localhost:5024).
5. Cargue el archivo XML de prueba `datos_completos.xml` para poblar el plano satelital.
6. En el panel de "Enrutamiento de Red", seleccione un nodo de origen (ej. SAT-ECU-0001) y uno de destino (ej. ANT-GTM-401).
7. Haga clic en el boton "Trazar Ruta Ortogonal".
8. Verifique que en el diagrama de la derecha, el camino que conecta a ambos nodos se resalta de color verde brillante, y los enlaces se vuelven gruesos y de color verde.
9. Observe el desglose detallado de saltos en el panel de enrutamiento con los nombres, identificadores y coordenadas de cada salto del camino.
10. Presione "Limpiar Trazado" para restablecer el diagrama al estado original.
