# Ejemplo MVC con SQLite en ASP.NET Core

Este proyecto es un ejemplo básico y completo de una arquitectura **Model-View-Controller (MVC)** implementada con **ASP.NET Core** en C# y almacenamiento local usando **SQLite**.

---

##  Tecnologías y Paquetes Utilizados

- **Core**: .NET 10.0 / C# 13
- **Base de Datos**: SQLite (almacenamiento local ligero en archivo `.db`)
- **OR/M**: Entity Framework Core (EF Core) para la interacción orientada a objetos con la base de datos
- **Front-end**: HTML5, Razor Pages, Bootstrap 5 (estilizado premium y responsivo), y Bootstrap Icons

### Paquetes NuGet Instalados
- `Microsoft.EntityFrameworkCore.Sqlite`: Proveedor de base de datos SQLite para EF Core.
- `Microsoft.EntityFrameworkCore.Design`: Herramientas para la configuración y diseño de modelos de base de datos en tiempo de desarrollo.

---

##  Estructura del Proyecto: ¿Por Dónde Empezar a Analizar?

Para comprender el flujo de la aplicación, te recomendamos seguir este orden de análisis:

1. **La Configuración Inicial**:
   -  [Program.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/EjemploMVC/Program.cs): Es el punto de entrada de la aplicación. Aquí se registran los servicios, se asocia el contexto de base de datos con SQLite a partir del archivo de configuración, se configura la ruta predeterminada hacia nuestro controlador y se ejecuta el sembrado (*seeding*) automático de la base de datos si esta no contiene registros.
   -  [appsettings.json](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/EjemploMVC/appsettings.json): Define la cadena de conexión (`Data Source=productos.db`), que indica que la base de datos SQLite se generará localmente en la raíz del proyecto con ese nombre.

2. **El Modelo (Model)**:
   -  [Models/Product.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/EjemploMVC/Models/Product.cs): Define la estructura de los datos del Producto (`Id`, `Nombre`, `Precio`, `Descripcion`, `Stock`) junto con anotaciones de validación (Data Annotations) para garantizar la integridad de los datos en el servidor y en el cliente.
   -  [Data/ApplicationDbContext.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/EjemploMVC/Data/ApplicationDbContext.cs): Clase que hereda de `DbContext`. Representa la sesión con la base de datos SQLite y expone la colección `Products` (tabla).

3. **El Controlador (Controller)**:
   -  [Controllers/ProductosController.cs](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/EjemploMVC/Controllers/ProductosController.cs): Contiene los métodos de acción para responder a las solicitudes web. Implementa todo el flujo CRUD de manera asíncrona:
     - `Index()`: Recupera la lista de productos y la envía a la vista.
     - `Details(id)`: Muestra la información detallada de un producto específico.
     - `Create()`: Recibe y procesa (mediante `[HttpPost]`) el formulario de un nuevo producto con validación de estado (`ModelState.IsValid`).
     - `Edit(id)`: Gestiona las actualizaciones del producto.
     - `Delete(id)`: Confirma y elimina de forma definitiva el registro en la base de datos.

4. **Las Vistas (Views)**:
   -  [Views/Productos/](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/EjemploMVC/Views/Productos): Contiene los archivos `.cshtml` (Razor Pages) correspondientes a cada acción del controlador:
     - `Index.cshtml`: Tabla estilizada con badges condicionales de stock.
     - `Create.cshtml` & `Edit.cshtml`: Formularios de ingreso y modificación de datos con validaciones en tiempo real.
     - `Details.cshtml`: Ficha premium con el desglose del producto.
     - `Delete.cshtml`: Confirmación con alertas de advertencia antes de remover un registro.
   -  [Views/Shared/_Layout.cshtml](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/EjemploMVC/Views/Shared/_Layout.cshtml): El diseño maestro común de la aplicación, configurado con fuentes externas y la CDN de Bootstrap Icons.

5. **El Estilo Personalizado**:
   -  [wwwroot/css/site.css](file:///c:/Users/ferna/Desktop/RepositorioLocal/VJ1S_IPC2P_2026/EjemploMVC/wwwroot/css/site.css): Modificaciones CSS adicionales para lograr un diseño "Premium" (sombras dinámicas en tarjetas, efectos *hover* sutiles y transiciones de carga fluida en las páginas).

---

##  Cómo Ejecutar la Aplicación

1. **Abre una terminal** en el directorio del proyecto `EjemploMVC`.
2. **Ejecuta el siguiente comando** para iniciar el servidor de desarrollo:
   ```bash
   dotnet run
   ```
3. El terminal te indicará las direcciones locales (por ejemplo, `http://localhost:5000` o `https://localhost:5001`). **Abre tu navegador de preferencia** e ingresa a una de ellas.
4. **Al primer inicio**:
   - El sistema detectará que no existe el archivo `productos.db` y lo creará automáticamente.
   - Se inyectarán 3 productos semilla (Laptop, Mouse, Teclado) para que puedas visualizar datos de inmediato.
