# Ejemplo 6: Catálogo de Satélites con Árbol AVL Auto-balanceado

## Sobre qué va el ejemplo

Este ejemplo demuestra la implementación y uso de una estructura de datos no lineal auto-balanceada (Árbol AVL) construida de forma manual mediante referencias y punteros en C# (.NET 10.0) bajo la arquitectura Modelo-Vista-Controlador (MVC).

El sistema simula un Catálogo de Satélites donde:
1. Se ingestan satélites por lotes de forma transaccional y atómica a través de un archivo XML (con comportamiento de commit y rollback) o de forma manual unitaria.
2. Se administran los satélites activos mediante un Árbol AVL (TDA RegistroSatelites) que se mantiene balanceado por altura en cada inserción a través de rotaciones simples y dobles (LL, RR, LR, RL).
3. Se dibuja dinámicamente en pantalla la estructura del Árbol AVL en formato ASCII, mostrando las alturas y el factor de balanceo de cada nodo para demostrar los efectos de las rotaciones en memoria.
4. Toda la actividad se audita mediante una Lista Enlazada Simple (TDA LogAuditoria) que despliega los logs en una tabla integrada en tiempo real.

## Explicación de lo que cambia con respecto al ejemplo anterior

En este ejemplo se realiza una transición de las listas enlazadas simples del Ejemplo 5 a una estructura de Árbol AVL auto-balanceado por altura implementada de forma manual en memoria RAM, conservando la ingesta XML transaccional como mecanismo de prueba.

A continuación se detallan los archivos agregados y modificados con respecto al Ejemplo 5:

### Archivos Modificados

#### [HomeController.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%206/Controllers/HomeController.cs)
* **Qué se cambió**: Se reescribió el controlador para orquestar las operaciones del Árbol AVL, la ingesta XML y la bitácora de logs, sustituyendo la persistencia lineal de satélites previa.
* **Cómo**: Se adaptaron las acciones POST: `CargarXml` (realiza la ingesta transaccional por lotes validando cada elemento con XPath y expresiones regulares, insertando al AVL en caso de Commit) y `AgregarSatelite` (valida mediante expresiones regulares e inserta el satélite unitario en el árbol AVL).
* **Por qué**: Para servir como el controlador lógico que coordina la persistencia no lineal y las validaciones de ingesta.

#### [Index.cshtml](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%206/Views/Home/Index.cshtml)
* **Qué se cambió**: Se rediseñó la vista principal para incorporar los controles de carga masiva y visuales de la estructura no lineal.
* **Cómo**: Se modificaron las tablas de datos para iterar sobre el Árbol AVL en inorden y se agregó un panel que muestra de forma jerárquica la estructura del árbol AVL en ASCII con su altura y balance.
* **Por qué**: Para permitir al usuario interactuar directamente con el catálogo y presenciar los cambios dinámicos del balanceo AVL.

#### [_Layout.cshtml](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%206/Views/Shared/_Layout.cshtml)
* **Qué se cambió**: Se modificó la cabecera de la plantilla de diseño común.
* **Cómo**: Se actualizó el título del sitio y el navbar para adaptarlos al ejemplo del árbol AVL, conservando el tema claro original de Bootstrap.
* **Por qué**: Para mantener una presentación limpia, responsiva y consistente con el curso.

#### [site.css](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%206/wwwroot/css/site.css)
* **Qué se cambió**: Se reestablecieron las hojas de estilo al estándar predeterminado de la plantilla de desarrollo.
* **Cómo**: Se eliminaron los estilos y fondos oscuros aplicados previamente para retornar al tema claro y limpio.
* **Por qué**: Para mantener la coherencia con los esquemas de color de los ejemplos precedentes.

---

### Nuevos Archivos Agregados

#### [SateliteAvlNode.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%206/Models/SateliteAvlNode.cs)
* **Qué se agregó**: La clase nodo para la construcción física del árbol AVL.
* **Cómo**: Define la propiedad de datos `Valor` de tipo `Satelite`, punteros de tipo `SateliteAvlNode` para los subárboles `Izquierdo` y `Derecho`, y una propiedad entera `Altura` (inicializada en 1).
* **Por qué**: Para servir como la unidad o bloque de almacenamiento enlazado sobre el cual opera el árbol balanceado.

#### [ArbolSatelitesAvl.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%206/Models/ArbolSatelitesAvl.cs)
* **Qué se agregó**: El TDA manual de tipo Árbol AVL auto-balanceable.
* **Cómo**: Implementa recursión para inserción (`Insertar`) y eliminación (`Eliminar`). En cada operación se actualiza la altura y se calcula el factor de balance; si es necesario, se ejecutan rotaciones simples (`RotarIzquierda`, `RotarDerecha`) o dobles a través de las anteriores (LL, RR, LR, RL). Incluye `ObtenerEstructuraVisual` para diagramar la jerarquía en texto.
* **Por qué**: Para garantizar un tiempo de búsqueda y almacenamiento de orden O(log n) mediante auto-balance.

#### [datos_validos.xml](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%206/wwwroot/datos_validos.xml) y [datos_invalidos.xml](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%206/wwwroot/datos_invalidos.xml)
* **Qué se agregó**: Archivos XML estructurados de configuración.
* **Cómo**: datos_validos.xml contiene satélites con formato correcto; datos_invalidos.xml contiene un satélite con ID erróneo para simular fallos.
* **Por qué**: Servir como insumos inmediatos para comprobar de forma práctica el comportamiento de Commit y Rollback sobre el árbol AVL.

## Cómo se ejecuta

Para compilar e iniciar la aplicación web en su entorno de desarrollo local:

1. Abra una terminal de comandos y ubíquese en el directorio del proyecto `Ejemplo 6`.
2. Restaure y compile el proyecto con el comando:
   dotnet build
3. Ejecute la aplicación con el comando:
   dotnet run --project "Ejemplo_6.csproj"
4. Abra su navegador web y diríjase a la URL de escucha local indicada en la consola (usualmente http://localhost:5000 o http://localhost:5200).
5. Interactúe con el panel utilizando los siguientes pasos:
   * **Carga XML Transaccional**: Suba el archivo `datos_invalidos.xml` para observar el aborto de la transacción (Rollback) y los errores detallados en consola. Posteriormente, suba el archivo `datos_validos.xml` para realizar la consolidación (Commit) exitosa y ver los satélites insertados en el catálogo AVL.
   * **Insertar Satélite en AVL**: Complete el formulario respetando el formato Regex del ID (ej. `SAT-ECU-2024`) y de la IP (ej. `192.168.1.1`). Presione el botón. Observe cómo el árbol AVL se reordena alfabéticamente por su ID y cómo se actualiza la jerarquía física en el bloque ASCII.
