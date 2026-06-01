# Introducción a la Programación y Computación 2 — Sección P
## Laboratorio - Vacaciones de Junio 2026

Este repositorio contiene los ejemplos prácticos y progresivos desarrollados a lo largo de las 20 sesiones de aprendizaje para guiar a los estudiantes en la correcta realización del Proyecto Único del laboratorio (**OrbitNet‑NetCore**).

---

## 🛠️ Lenguaje y Tecnologías del Curso

* **Lenguaje de Programación**: C# (moderno, utilizando sintaxis simplificada y *Top-Level Statements*).
* **Entorno de Ejecución**: .NET Core 8.0 o .NET Core 10.0.
* **Paradigma Principal**: Programación Orientada a Objetos (POO) y Estructuras de Datos Abstractas (TDAs) de bajo nivel.
* **Componente Web**: ASP.NET Core MVC (Modelo-Vista-Controlador) y Vistas Razor.
* **Motor de Visualización**: Lenguaje DOT de Graphviz (compilado en memoria RAM en formato vectorial SVG).

---

## 📂 Herramientas y Requisitos de Instalación

Para que los estudiantes puedan replicar y desarrollar adecuadamente los ejemplos del curso, es necesario instalar y configurar las siguientes herramientas en sus computadoras:

### 1. El Entorno .NET SDK (Software Development Kit)
El SDK incluye los compiladores de C#, las herramientas de desarrollo en consola y las librerías base de ejecución.

* **Descarga Oficial**: Visita la página de descargas de .NET: [Descargar .NET](https://dotnet.microsoft.com/es-es/download)
* **Versión Recomendada**: .NET SDK 8.0 LTS o .NET SDK 10.0.
* **Verificación de Instalación**:
  Abre una terminal de comandos (CMD o PowerShell) y ejecuta el siguiente comando:
  ```bash
  dotnet --version
  ```
  Debería retornar una cadena de texto similar a `8.0.xxx` o `10.0.xxx`.

### 2. Editor de Código (IDE)
Se recomienda el uso de uno de los siguientes entornos profesionales de desarrollo:

* **Visual Studio Code (VS Code)** (Recomendado por su ligereza y velocidad).
  * Descarga: [Descargar VS Code](https://code.visualstudio.com/)
* **Visual Studio 2022 Community** (Opción robusta y completa para Windows).
  * Descarga: [Descargar Visual Studio](https://visualstudio.microsoft.com/es/)

### 3. Extensiones Indispensables de VS Code
Si optas por utilizar Visual Studio Code, **debes** instalar el siguiente paquete de extensiones desde el Marketplace de VS Code:

* **C# Dev Kit** (Desarrollado por Microsoft): Proporciona soporte avanzado de autocompletado de código (IntelliSense), análisis de errores sintácticos y ejecución rápida de proyectos.
* **C#** (Desarrollado por Microsoft): El motor principal para compilar y depurar código en C#.
* **Graphviz Preview** (Por *Stefan Goessner* o similar): Permite previsualizar los grafos DOT generados antes de inyectarlos en tu servidor web.
* **XML Tools** (Por *Josh Johnson*): Facilita el análisis, formato y pruebas de consultas XPath dentro del editor de archivos XML.

### 4. Graphviz (Motor de Grafos)
Indispensable para generar los diagramas visuales en memoria de las estructuras de datos (Memory Map y Matrices Dispersas).

* **Instalación rápida en Windows** (vía terminal de comandos PowerShell):
  ```powershell
  winget install Graphviz.Graphviz
  ```
* **Instalación Manual**:
  1. Descarga el instalador ejecutable (`.exe`) desde la página oficial: [Graphviz Downloads](https://graphviz.org/download/)
  2. Al ejecutar el instalador, **asegúrate de marcar la casilla**: *"Add Graphviz to the system PATH for all users"* (esto permite que .NET localice el ejecutable en el sistema de forma global).
* **Verificación de Instalación**:
  Ejecuta en consola:
  ```bash
  dot -V
  ```
  Debería mostrar la versión instalada (ej. `dot - graphviz version 12.0.0`).

---

## 📅 Estructura de las 20 Sesiones Prácticas (Ejemplos)

A continuación se detalla la ruta de aprendizaje progresiva que conecta los fundamentos básicos hasta la arquitectura del proyecto final:

| Sesión | Tema Principal | Conceptos Clave |
| :---: | :--- | :--- |
| **01** | **[Ejemplo 1](./Ejemplo%201/)** | Introducción a POO: Clases, Constructores e Instanciación con campos públicos. |
| **02** | **Ejemplo 2** | Encapsulamiento: Visibilidad privada (`private`) y métodos Get/Set clásicos. |
| **03** | **Ejemplo 3** | Propiedades en C#: Atajos automáticos `{ get; set; }` y descriptores lógicos. |
| **04** | **Ejemplo 4** | Nodos Manuales: Enlace autorreferenciado a nivel físico en memoria. |
| **05** | **Ejemplo 5** | TDA Lista Simple (Inserción): Construcción desde cero de estructuras lineales. |
| **06** | **Ejemplo 6** | TDA Lista Simple (Recorrido): Implementación de iteración manual de punteros. |
| **07** | **Ejemplo 7** | Ingesta XML: Lectura de archivos con `XmlDocument` sin XPath. |
| **08** | **Ejemplo 8** | XPath en .NET: Consultas dirigidas mediante selectores `SelectNodes`. |
| **09** | **Ejemplo 9** | Expresiones Regulares: Validaciones lógicas con `System.Text.RegularExpressions`. |
| **10** | **Ejemplo 10** | XML + RegEx: Validaciones complejas previas a la creación de instancias. |
| **11** | **Ejemplo 11** | Carga Transaccional: Volcado atómico del XML hacia el TDA Lista Simple en RAM. |
| **12** | **Ejemplo 12** | TDA Árbol Binario de Búsqueda: Creación de árboles y colas de prioridad (ABB). |
| **13** | **Ejemplo 13** | TDA Matriz Dispersa Ortogonal: Concepto de punteros bidireccionales. |
| **14** | **Ejemplo 14** | Generador Graphviz: Estructura del lenguaje DOT y formato gráfico. |
| **15** | **Ejemplo 15** | Mapa de Memoria DOT: Renderizado visual de celdas físicas RAM (`shape=record`). |
| **16** | **Ejemplo 16** | ASP.NET Core MVC: Estructura web, rutas, controladores y vistas Razor `.cshtml`. |
| **17** | **Ejemplo 17** | Endpoints REST en .NET: Serialización de modelos a formato dinámico JSON. |
| **18** | **Ejemplo 18** | Servidores Multi-Puerto: Levantamiento de múltiples instancias Kestrel locales. |
| **19** | **Ejemplo 19** | Cliente HTTP en C#: Peticiones síncronas entre servidores mediante `HttpClient`. |
| **20** | **Ejemplo 20** | Simulación Integrada: Enrutamiento cross-instance con Basic Authentication. |
