# Ejemplo 4: Ingesta XML y Consultas XPath en ASP.NET Core MVC


El objetivo es enseñar a los estudiantes cómo permitir la **carga física de archivos** en una aplicación web MVC, procesar el archivo XML recibido en memoria RAM utilizando `XmlDocument` bajo esquemas de **seguridad contra XXE**, y realizar consultas selectivas de datos usando **XPath** para alimentar un TDA manual enlazado.

---

##  Objetivos de la Sesión

Al finalizar este ejemplo, los estudiantes comprenderán:
1. **Subida de Archivos en MVC**: Cómo diseñar formularios HTML multipart y recibir archivos en el controlador usando la interfaz `IFormFile`.
2. **Mitigación de Inyección XXE (XML External Entity)**: Por qué es importante desactivar la resolución de DTDs externas en `XmlDocument` para evitar brechas de seguridad.
3. **Consultas XPath en .NET**: Uso de los métodos `SelectNodes` y `SelectSingleNode` para ubicar elementos específicos sin necesidad de iterar ciegamente toda la estructura XML.
4. **Validación Transaccional**: Procesar secuencialmente los elementos de un archivo, insertando los nodos que cumplan las reglas lógicas en el TDA manual, y descartando los corruptos sin colapsar el servidor.

---

## 📂 Estructura de Archivos del Ejemplo 4

* 📄 **`Ejemplo_4.csproj`**: Archivo de configuración del SDK de ASP.NET Core Web (.NET 10.0).
* 📁 **`Models/`**:
  * 📄 **`Satelite.cs`**, **`NodoSatelite.cs`**, **`ListaSatelites.cs`**: Clases del TDA enlazado manual con soporte para limpieza de memoria (`Limpiar()`).
* 📁 **`Controllers/`**:
  * 📄 **`HomeController.cs`**: Controlador que gestiona la carga mediante POST, abre el flujo del archivo subido, aplica mitigación XXE y ejecuta consultas XPath.
* 📁 **`Views/Home/`**:
  * 📄 **`Index.cshtml`**: Vista Razor que integra el formulario web de carga masiva (`enctype="multipart/form-data"`), botones de control y la tabla de resultados.
* 📁 **`wwwroot/`**:
  * 📄 **`datos_prueba.xml`**: Archivo XML de muestra descargable desde la propia interfaz web para que los usuarios prueben la ingesta.

---

##  Conceptos Clave Explicados

### 1. Recepción del Archivo (`IFormFile`)

Para enviar archivos al servidor, el formulario HTML debe usar el método `POST` y declarar el atributo de codificación multipart:

```html
<form asp-action="CargarXml" method="post" enctype="multipart/form-data">
    <input type="file" name="archivoXml" />
    <button type="submit">Procesar</button>
</form>
```

En el controlador `HomeController.cs`, capturamos el archivo usando un parámetro de tipo `IFormFile` que coincida exactamente con el atributo `name` del formulario:

```csharp
[HttpPost]
public IActionResult CargarXml(IFormFile archivoXml)
{
    // El archivo está disponible como un Stream de lectura
    using (Stream stream = archivoXml.OpenReadStream())
    {
        // Procesar archivo...
    }
}
```

### 2. Mitigación contra Ataques XXE

Por defecto, los analizadores XML pueden intentar resolver referencias a entidades externas definidas en el archivo, lo que expone al servidor a fugas de archivos locales o ataques SSRF. Mitigamos esto deshabilitando el procesamiento de DTDs externas en `XmlReaderSettings`:

```csharp
XmlReaderSettings settings = new XmlReaderSettings
{
    DtdProcessing = DtdProcessing.Prohibit, // Bloquea procesamiento de DTDs externas
    XmlResolver = null                     // Deshabilita la resolución
};

using (XmlReader reader = XmlReader.Create(stream, settings))
{
    XmlDocument doc = new XmlDocument();
    doc.Load(reader); // Carga segura en memoria
}
```

### 3. Consultas XPath Dinámicas

En lugar de navegar recursivamente por todos los nodos hijos del XML, empleamos **XPath** para realizar una consulta dirigida. Esto nos retorna directamente una lista con los elementos satélite ecuatoriales:

```csharp
XmlNodeList satelitesNodos = doc.SelectNodes("/orbitnet/constelaciones_ecuatoriales/satelite");

foreach (XmlNode nodo in satelitesNodos)
{
    string id = nodo.Attributes["id"].Value;
    string nombre = nodo.SelectSingleNode("nombre").InnerText;
    string enlaceIp = nodo.SelectSingleNode("enlace_ip").InnerText;
}
```

---

##  Instrucciones de Ejecución Local

Para levantar el servidor web de la simulación:

1. Abre tu terminal de comandos en la raíz del repositorio.
2. Ejecuta:

```bash
dotnet run --project ".\Ejemplo 4\Ejemplo_4.csproj"
```

3. Accede en tu navegador a la URL indicada (usualmente `http://localhost:5000` o `http://localhost:5001`).
4. Descarga el archivo de prueba `datos_prueba.xml` usando el enlace de la tarjeta derecha.
5. Sube el archivo utilizando el formulario de la tarjeta izquierda y observa el procesamiento en pantalla: los satélites correctos se agregarán a la tabla de memoria RAM, mientras que los corruptos serán descartados de forma controlada sin detener el servidor.
