# Ejemplo 7: Matriz Dispersa Ortogonal y Renderizado SVG In-Memory

## Sobre qué va el ejemplo

Este ejemplo demuestra la implementación y uso de una estructura de datos bidimensional no lineal dispersa (Matriz Dispersa Ortogonal Bidireccional) construida de forma manual mediante referencias y punteros en C# (.NET 10.0) bajo la arquitectura Modelo-Vista-Controlador (MVC). Además, introduce el motor de visualización dinámica en memoria RAM mediante la herramienta Graphviz sin escritura física de archivos en el disco duro.

El sistema simula un Plano Satelital de Coordenadas Terrestres (Latitud/Fila y Longitud/Columna) donde:
1. Se ingestan satélites por lotes de forma transaccional y atómica a través de un archivo XML (con comportamiento de commit y rollback) o de forma manual indicando las coordenadas enteras (Fila, Columna) correspondientes.
2. Se administran los satélites activos mediante una Matriz Dispersa Ortogonal (TDA RedSatelitalPlano) que enlaza cabeceras de filas y columnas, insertando o eliminando nodos y reestructurando quirúrgicamente los cuatro punteros direccionales (Up, Down, Left, Right) para prevenir discontinuidades.
3. Se genera un mapa visual de la memoria física en caliente (Memory Layout Map) compilando instrucciones DOT a formato vectorial SVG redirigiendo asíncronamente los flujos de entrada y salida del subproceso `dot.exe` para evitar deadlocks en el sistema operativo.
4. Se garantiza la ausencia total del uso de la librería de colecciones genéricas de .NET (`System.Collections.Generic` y `System.Collections`), gestionando la iteración de datos a través de arreglos de C# y punteros físicos.
5. Toda la actividad se audita mediante una Lista Enlazada Simple (TDA LogAuditoria) adaptada para operar de forma nativa.

## Explicación de lo que cambia con respecto al ejemplo anterior

En este ejemplo se realiza la transición del Árbol AVL del Ejemplo 6 a una Matriz Dispersa Ortogonal Bidireccional. Se incorpora la generación y renderizado dinámico de diagramas SVG de Graphviz y se remueve por completo el uso de interfaces y colecciones genéricas (`System.Collections.Generic`), adoptando arreglos nativos para transferir datos a las vistas.

A continuación se detallan los archivos agregados y modificados con respecto al Ejemplo 6:

### Archivos Modificados

#### [HomeController.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%207/Controllers/HomeController.cs)
* **Qué se cambió**: Se reescribió el controlador para orquestar la Matriz Dispersa Ortogonal, la ingesta XML con XPath, los logs y el renderizado SVG, reemplazando la persistencia y acciones del Árbol AVL y las Pilas/Colas de Ejemplo 6.
* **Cómo**: Se declararon las acciones POST `CargarXml` (separa satélites en constelaciones ecuatoriales y polares mapeándolas a filas 0 y 1, deduce la columna a partir del ID, valida con Regex, y consolida atómicamente), `InsertarNodo` (valida e inserta en coordenadas libres), `EliminarNodo` (libera el nodo en la coordenada y reconecta los vecinos), `LimpiarMatriz` y `LimpiarLogs`. Llama a `GraphvizCompilador` en cada carga para refrescar el SVG.
* **Por qué**: Para coordinar el flujo bidimensional y asegurar que la interfaz reciba siempre el diagrama SVG actualizado del estado de la memoria RAM.

#### [Index.cshtml](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%207/Views/Home/Index.cshtml)
* **Qué se cambió**: Se rediseñó la vista principal para incorporar los controles bidimensionales y el lienzo del diagrama de memoria.
* **Cómo**: Se reemplazó la visualización del árbol por un visor que renderiza el SVG en crudo mediante `@Html.Raw()`. Se añadieron campos numéricos para ingresar filas y columnas (Y, X) en los formularios manuales de inserción y borrado, y se modificaron las tablas de datos para mostrar las coordenadas bidimensionales de los nodos.
* **Por qué**: Para permitir al usuario interactuar visualmente con la malla y observar cómo se conectan y desconectan los nodos cabecera y de datos.

#### [_Layout.cshtml](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%207/Views/Shared/_Layout.cshtml)
* **Qué se cambió**: Se actualizó la barra de navegación y los metadatos de la plantilla responsiva.
* **Cómo**: Se modificaron los títulos y marcas textuales para referenciar al Ejemplo 7 y a la Matriz Dispersa Ortogonal.
* **Por qué**: Para brindar la información contextual correcta de este hito de aprendizaje.

#### [site.css](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%207/wwwroot/css/site.css)
* **Qué se cambió**: Se mantuvo la hoja de estilos predeterminada de la aplicación basada en el tema claro estándar.
* **Cómo**: Se heredaron las directivas base de fuentes y campos de la plantilla.
* **Por qué**: Mantener la estética limpia y tradicional de los laboratorios del curso.

---

### Nuevos Archivos Agregados

#### [MatrixNode.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%207/Models/MatrixNode.cs)
* **Qué se agregó**: La clase nodo de datos ortogonal para la matriz.
* **Cómo**: Almacena las coordenadas `Row` y `Col`, datos del satélite (`Id`, `Nombre`, `IpAddress`) y las cuatro referencias directas de vecindad: `Up`, `Down`, `Left` y `Right` de tipo `MatrixNode?`.
* **Por qué**: Sirve como la celda física de datos que se interconecta en la red.

#### [HeaderNode.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%207/Models/HeaderNode.cs)
* **Qué se agregó**: La clase nodo cabecera para indexar filas y columnas.
* **Cómo**: Almacena la propiedad `Index` (índice del eje), una referencia `Next` al siguiente cabecera del mismo eje, y un puntero de acceso `Access` al primer nodo de datos asociado.
* **Por qué**: Actúa como el punto de anclaje inicial para el barrido secuencial horizontal o vertical de la matriz.

#### [RedSatelitalPlano.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%207/Models/RedSatelitalPlano.cs)
* **Qué se agregó**: El TDA manual Matriz Dispersa Ortogonal.
* **Cómo**: Administra cabeceras y nodos enlazados. Implementa inserción ordenada en ambos ejes, eliminación física con reconexión de punteros adyacentes y eliminación de cabeceras vacías. Incluye `GenerarCodigoDot` para escribir las instrucciones que grafican cabeceras y celdas como tablas HTML-like (`shape=none`), y exporta arreglos nativos C# para iteración.
* **Por qué**: Encapsula de forma estricta la lógica bidimensional del plano satelital y formatea el código visual para Graphviz sin incurrir en errores sintácticos de alineación.

#### [GraphvizCompilador.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%207/Services/GraphvizCompilador.cs)
* **Qué se agregó**: El compilador dinámico de Graphviz en memoria RAM.
* **Cómo**: Utiliza la clase `System.Diagnostics.Process` para llamar al binario `dot` del sistema operativo. Redirecciona los flujos de entrada, salida y error; escribe en `StandardInput` y cierra el canal de escritura antes de leer la salida para mitigar deadlocks del buffer del sistema operativo.
* **Por qué**: Genera la representación gráfica SVG sin realizar escrituras de archivos en el disco duro local.

#### [LogRegistro.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%207/Models/LogRegistro.cs), [NodoLog.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%207/Models/NodoLog.cs) y [ListaLogs.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%207/Models/ListaLogs.cs)
* **Qué se agregó**: TDA LogAuditoria libre de dependencias genéricas.
* **Cómo**: Implementa inserción simple secuencial. El método `ObtenerTodos()` retorna un arreglo nativo `LogRegistro[]` de tamaño exacto `tamano` para permitir el renderizado de la bitácora sin importar `System.Collections.Generic`.
* **Por qué**: Provee la bitácora de auditoría histórica exigida bajo las restricciones estrictas del curso.

#### [DashboardViewModel.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%207/Models/DashboardViewModel.cs)
* **Qué se agregó**: El ViewModel compuesto para transferir el plano disperso, logs y el SVG a la vista.
* **Cómo**: Encapsula propiedades para `RedSatelitalPlano`, `ListaLogs` y la cadena `SvgDiagrama`.
* **Por qué**: Permite renderizar simultáneamente la bitácora, la tabla de satélites activos y el diagrama en caliente en la página Razor.

#### [datos_validos.xml](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%207/wwwroot/datos_validos.xml) y [datos_invalidos.xml](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%207/wwwroot/datos_invalidos.xml)
* **Qué se agregó**: Archivos XML estructurados de configuración.
* **Cómo**: datos_validos.xml contiene satélites con formato correcto; datos_invalidos.xml contiene un satélite con ID erróneo para simular fallos.
* **Por qué**: Servir como insumos inmediatos para comprobar de forma práctica el comportamiento de Commit y Rollback en la matriz.

## Cómo se ejecuta

Para compilar e iniciar la aplicación web en su entorno de desarrollo local:

1. Abra una terminal de comandos y ubíquese en el directorio del proyecto `Ejemplo 7`.
2. Restaure y compile el proyecto con el comando:
   dotnet build
3. Ejecute la aplicación con el comando:
   dotnet run --project "Ejemplo_7.csproj"
4. Abra su navegador web y diríjase a la URL de escucha local indicada en la consola (usualmente http://localhost:5000 o http://localhost:5200).
5. **Requisitos de Graphviz**:
   * Es indispensable tener instalada la herramienta **Graphviz** en el sistema.
   * El servicio [GraphvizCompilador.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%207/Services/GraphvizCompilador.cs) busca de forma predeterminada el compilador en los directorios típicos de instalación de Windows (`C:\Program Files\Graphviz\bin\dot.exe` y `C:\Program Files (x86)\Graphviz\bin\dot.exe`). 
   * Si su instalación se encuentra en otra ubicación, asegúrese de agregar el directorio `bin` de Graphviz a la variable de entorno `PATH` del sistema operativo para que el ejecutable `dot` pueda ser invocado de forma global.
6. **Nota de Diseño (Tablas HTML-like en DOT)**:
   * Graphviz posee una limitación por la cual arroja advertencias y errores fatales de compilación (código de salida 1) si se intenta alinear nodos en la misma fila utilizando `rank=same` combinados con la propiedad clásica de cajas de registro (`shape=record`). 
   * Para evitar este problema de "flat edges" y garantizar un renderizado robusto del plano bidimensional en caliente, el código DOT de este ejemplo ha sido estructurado utilizando **tablas HTML-like (`shape=none`)**, lo cual es la solución estándar recomendada para mantener la estructura ortogonal y los puertos de conexión físicos (`up`, `down`, `left`, `right`) funcionando simultáneamente.
7. Interactúe con el panel utilizando los siguientes pasos:
   * **Carga XML Transaccional**: Suba el archivo `datos_invalidos.xml` para observar el aborto de la transacción (Rollback) y los errores detallados en consola. Posteriormente, suba el archivo `datos_validos.xml` para realizar la consolidación (Commit) exitosa y ver los satélites insertados en la Matriz Dispersa.
   * **Insertar Nodo Satelital**: Complete el formulario manual indicando una Fila (ej. `5`), una Columna (ej. `10`), ID, Nombre e IP. Presione el botón. Observe cómo se dibuja el nodo en el visor SVG y cómo las cabeceras y referencias ortogonales `Up/Down` y `Left/Right` se conectan con los nodos adyacentes.
   * **Eliminar Nodo Satelital**: Escriba las coordenadas del nodo a borrar y presione el botón "Borrar". El nodo desaparecerá del visor y sus vecinos se reconectarán quirúrgicamente.
