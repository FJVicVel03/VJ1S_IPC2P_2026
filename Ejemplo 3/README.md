# Ejemplo 3: Introducción a ASP.NET Core MVC y TDA Manual (Lista Simple)


En esta lección, damos el salto de aplicaciones de consola a la **web** e introducimos el patrón arquitectónico **Modelo-Vista-Controlador (MVC)** utilizando ASP.NET Core, integrándolo con nuestra primera estructura de datos manual (**Lista Enlazada Simple**) construida con referencias en memoria RAM (sin colecciones nativas de .NET).

---

##  Objetivos de la Sesión

Al finalizar este ejemplo, los estudiantes comprenderán:
1. **El Patrón MVC**: La separación de responsabilidades en una aplicación web:
   * **Modelo (Model)**: Contiene los datos y la lógica estructural (TDA).
   * **Controlador (Controller)**: Administra las solicitudes HTTP, procesa las acciones e interactúa con el modelo para alimentar a la vista.
   * **Vista (View)**: Renderiza la interfaz gráfica final del usuario a partir de plantillas Razor (`.cshtml`).
2. **Uso de TDAs en Razor**: Cómo declarar e iterar de forma directa una estructura enlazada personalizada dentro de una vista web para dibujar tablas responsivas.
3. **Persistencia Estática en Memoria RAM**: Cómo simular el almacenamiento no volátil de la constelación usando variables estáticas a lo largo de las peticiones web.

---

## 📂 Estructura de Archivos del Proyecto

La estructura del proyecto web contiene los directorios del patrón MVC:

* 📄 **`Ejemplo_3.csproj`**: Archivo de configuración del SDK de ASP.NET Core (`Microsoft.NET.Sdk.Web`).
* 📁 **`Models/`**:
  * 📄 **`Satelite.cs`**: Clase modelo que encapsula la información de telemetría de un satélite.
  * 📄 **`NodoSatelite.cs`**: Clase que representa un nodo enlazado (contiene un `Satelite` y la referencia al nodo `Siguiente`).
  * 📄 **`ListaSatelites.cs`**: El TDA manual de tipo Lista Enlazada Simple que implementa el método iterador `Recorrer()`.
* 📁 **`Controllers/`**:
  * 📄 **`HomeController.cs`**: Controlador que gestiona la solicitud HTTP a la página de inicio, inicializa el TDA estático y lo pasa como modelo.
* 📁 **`Views/Home/`**:
  * 📄 **`Index.cshtml`**: Vista Razor que recibe el modelo `ListaSatelites` y genera la tabla HTML dinámica.

---

###  ¿Cómo Crear esta Estructura Automáticamente?

En lugar de crear manualmente cada una de las carpetas (`Controllers`, `Models`, `Views`) y los archivos de configuración, el estándar en .NET es generar la plantilla base utilizando la interfaz de línea de comandos de .NET (CLI). 

Para recrear esta estructura desde cero, ejecuta el siguiente comando en tu terminal:

```bash
dotnet new mvc -o "Ejemplo 3"
```

**¿Qué hace este comando?**
1. Crea un nuevo directorio llamado `Ejemplo 3`.
2. Genera automáticamente las carpetas estructurales del patrón: `Controllers/`, `Models/`, `Views/` y `wwwroot/` (para archivos estáticos como CSS/JS).
3. Configura los archivos del sistema web (`Program.cs`, `appsettings.json` y el archivo de proyecto `.csproj`).
4. Provee controladores y vistas por defecto, los cuales puedes modificar con el código provisto en este ejemplo.

---

##  Conceptos Clave Explicados

### 1. El TDA Enlazado como Modelo de Vista

Para pasar los elementos de nuestra estructura manual a la vista Razor, el TDA `ListaSatelites` expone el método iterador `Recorrer()`. Este método utiliza `yield return` para retornar secuencialmente cada satélite siguiendo las referencias de los nodos, sin necesidad de volcar los elementos a una lista nativa de C#:

```csharp
public IEnumerable<Satelite> Recorrer()
{
    NodoSatelite? actual = cabeza;
    while (actual != null)
    {
        yield return actual.Valor;
        actual = actual.Siguiente;
    }
}
```

### 2. Recorrido de Punteros en Razor (`Index.cshtml`)

En la cabecera de la vista Razor, declaramos la clase del TDA manual como el modelo oficial de datos utilizando la directiva `@model`:

```razor
@model Ejemplo_3.Models.ListaSatelites
```

Luego, en el cuerpo del HTML, podemos iterar los satélites llamando al método `Recorrer()` usando un bucle standard `foreach`:

```razor
@foreach (var satelite in Model.Recorrer())
{
    <tr>
        <td>@satelite.Id</td>
        <td>@satelite.Nombre</td>
        <td>@satelite.EnlaceIP</td>
    </tr>
}
```

---

## 🛠️ Instrucciones de Ejecución Local

Para compilar y levantar el servidor web local:

1. Ubícate en la raíz del repositorio.
2. Ejecuta en tu terminal el siguiente comando de la CLI de .NET:

```bash
dotnet run --project ".\Ejemplo 3\Ejemplo_3.csproj"
```

3. El comando compilará el proyecto e iniciará el servidor web Kestrel. Verás una salida en consola indicando la dirección local activa (usualmente `http://localhost:5000` o `http://localhost:5001`).
4. Abre tu navegador web e ingresa a la dirección mostrada para observar el renderizado web de la lista enlazada manual.
