# Ejemplo 11: Ingesta Completa XML y Catalogo AVL

## Sobre que va el ejemplo

Este ejemplo demuestra la implementacion de un motor de ingesta masiva y parseo XML transaccional que cubre por completo el esquema oficial del simulador satelital. Se realiza la lectura estructurada de satelites ecuatoriales, satelites polares y antenas terrestres utilizando consultas XPath directas con la clase nativa `XmlDocument`.

Se introduce un Catalogo global para satelites polares en la memoria RAM utilizando un Arbol AVL auto-balanceado implementado de forma manual (sin depender de `System.Collections.Generic`). Ademas, se elimina la restriccion de indices no negativos en la Matriz Dispersa para dar soporte a coordenadas de latitud y longitud reales (tanto positivas como negativas), redondeandolas e insertando antenas terrestres en sus posiciones geograficas correspondientes.

## Explicacion de lo que cambia con respecto al ejemplo anterior

Con respecto al Ejemplo 10, la aplicacion incorpora la lectura de nodos complejos y multiples estructuras de datos. A continuacion se detallan los cambios estructurales:

### Archivos Nuevos Agregados

#### [SateliteAvlNode.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%2011/Models/SateliteAvlNode.cs)
* **Que se agrego**: Clase nodo para la estructura del Arbol AVL.
* **Como**: Encapsula un objeto `Satelite`, referencias a hijos `Izquierdo` y `Derecho`, y la altura del nodo (`Altura`) para calculos de balanceo.
* **Por que**: Es la celda basica de construccion del catalogo de satelites de forma manual en el Heap.

#### [ArbolSatelitesAvl.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%2011/Models/ArbolSatelitesAvl.cs)
* **Que se agrego**: Estructura de Arbol AVL Auto-balanceado.
* **Como**: Implementa rotaciones simples y dobles (LL, RR, LR, RL) de manera manual. Exporta sus elementos en in-orden como un arreglo nativo (`Satelite[]`) en lugar de usar interfaces genericas como `List` o `IEnumerable`.
* **Por que**: Actua como el catalogo global de satelites historicos y activos del simulador, garantizando tiempos de busqueda de O(log n).

### Archivos Modificados

#### [Satelite.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%2011/Models/Satelite.cs)
* **Que se cambio**: Soporte de propiedades y constructores.
* **Como**: Se agrego la propiedad `Frecuencia` (double) y se implemento un constructor sobrecargado para inicializar satelites polares sin direccion IP. Se permitio que `EnlaceIP` acepte cadenas vacias.
* **Por que**: Modela adecuadamente tanto los satelites ecuatoriales (con IP) como los polares (con frecuencia).

#### [RedSatelitalPlano.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%2011/Models/RedSatelitalPlano.cs)
* **Que se cambio**: Eliminacion de limites en coordenadas.
* **Como**: Se quito la excepcion de coordenadas no negativas (`row < 0 || col < 0`) en la insercion.
* **Por que**: Permite el enlazado ortogonal en cuadrantes cartesianos negativos para representar latitudes y longitudes geograficas reales de antenas terrestres.

#### [XmlController.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%2011/Controllers/XmlController.cs)
* **Que se cambio**: Motor de Ingesta XML Completo.
* **Como**: Se modifico para parsear `//constelaciones_ecuatoriales/satelite`, `//orbitas_polares/polar/satelite` y `//antenas_terrestres/antena`. Extrae las coordenadas geograficas de las antenas, las valida mediante la expresion regular `^-?\d{1,2}\.\d{4,6},-?\d{1,3}\.\d{4,6}$`, las redondea a enteros (`Math.Round`) y las procesa de forma transaccional.
* **Por que**: Asegura que el archivo sea procesado de forma atomica (Commit o Rollback) en todos sus elementos constituyentes.

#### [Index.cshtml](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/Ejemplo%2011/Views/Home/Index.cshtml)
* **Que se cambio**: Interfaz Grafica.
* **Como**: Se agrego la tabla para renderizar el Catalogo AVL de satelites polares y se ajustaron los formularios manuales para aceptar valores negativos.
* **Por que**: Permite validar visualmente el estado del catalogo en memoria RAM y probar inserciones manuales de coordenadas de antenas reales.

---

## Relacion con el Proyecto Unico

Este ejemplo resuelve directamente los requerimientos criticos de dos fases del proyecto:

### Que se esta resolviendo del proyecto
* **Seccion 6 (Motor de Ingesta, Carga Masiva y Validaciones RegEx)**: Ingesta XPath con `XmlDocument` para procesar el XML completo, validando ID, IP y coordenadas con expresiones regulares dinamicas.
* **Seccion 5.2 (TDA RegistroSatelites: Arbol AVL)**: Estructura de balanceo dinamico auto-balanceado de satelites polares.

### ¿Como pueden aprovechar este ejemplo?
* **Validacion Transaccional de Lotes**: Muestra como acumular elementos en listas enlazadas temporales personalizadas (`NodoMatrizTemporal`, `NodoAvlTemporal`) para verificar que no haya colisiones de identificadores o coordenadas en el archivo XML antes de escribir definitivamente en las estructuras reales de la RAM.
* **Conversion de Coordenadas**: Enseña como parsear y redondear las coordenadas decimales del XML (`14.5891,-90.5514`) para convertirlas en coordenadas de matriz enteras (`15,-91`) compatibles con la Matriz Dispersa.
* **TDA AVL sin Genericos**: Los estudiantes pueden estudiar la logica de rotaciones y actualizacion de alturas utilizando punteros puros y su exportacion en arreglos nativos.

---

## Como se ejecuta

Para compilar y ejecutar esta aplicacion web localmente:

1. Abra una terminal de comandos y ubiquese en el directorio del proyecto "Ejemplo 11".
2. Compile el proyecto para comprobar que no existan errores:
   dotnet build
3. Ejecute la aplicacion con el comando:
   dotnet run --project "Ejemplo_11.csproj"
4. Abra su navegador en la direccion de escucha indicada (usualmente http://localhost:5022 o http://localhost:5000).
5. Cargue el archivo de prueba oficial `datos_completos.xml` para poblar simultaneamente la matriz dispersa (satelites ecuatoriales y antenas terrestres en coordenadas reales) y el arbol AVL (satelites polares con frecuencia).
