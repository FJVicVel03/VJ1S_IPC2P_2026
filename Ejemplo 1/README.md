# Ejemplo 1: Introducción a la Programación Orientada a Objetos (POO)

¡Bienvenido al **Ejemplo 1** del currículo progresivo diseñado para la clase!

---

##  Objetivos de la Sesión

Al finalizar este ejemplo, los estudiantes comprenderán:
1. **Qué es una Clase**: La plantilla o plano lógico (`Satelite`) que define la estructura y comportamiento de una entidad.
2. **Qué es un Objeto / Instancia**: La materialización concreta en memoria RAM de una clase.
3. **El Constructor (`__init__`)**: El método especial para inicializar el estado del objeto.
4. **Atributos Públicos**: Variables internas del objeto a las cuales se puede acceder y modificar directamente desde cualquier parte del código externo.

---

##  Estructura del Código

El ejemplo consta de dos archivos sumamente sencillos y acoplados lógicamente:

1. **`satelite.py`**: Contiene la definición de la clase `Satelite`.
2. **`main.py`**: Contiene la inicialización de los objetos, impresión de sus estados y la modificación de sus atributos en consola.

---

##  Conceptos Clave Explicados

### 1. Definición de la Clase (`satelite.py`)

Una clase en Python se declara con la palabra reservada `class`. El constructor se define mediante la función especial `__init__`, la cual recibe la palabra clave `self` como primer parámetro (que representa la instancia actual del objeto):

```python
class Satelite:
    def __init__(self, id_satelite: str, nombre: str, enlace_ip: str):
        # Atributos públicos
        self.id = id_satelite
        self.nombre = nombre
        self.enlace_ip = enlace_ip
```

* **Atributos Públicos**: Variables declaradas con la estructura `self.nombre_atributo = valor`. Cualquier script que importe la clase `Satelite` puede leer o reescribir estas variables de forma directa (por ejemplo, haciendo `objeto.enlace_ip = "NUEVA_IP"`).

### 2. Instanciación y Modificación (`main.py`)

Para "fabricar" u obtener una instancia de nuestra clase, la llamamos como si fuera una función común, pasándole los argumentos requeridos por el constructor `__init__`:

```python
# Instanciación
satelite_a = Satelite("SAT-ECU-0001", "Starlink-Norte-A", "127.0.0.1")

# Lectura directa de atributos
print(satelite_a.nombre)  # Imprime: Starlink-Norte-A

# Modificación directa de atributos
satelite_a.enlace_ip = "192.168.1.100"
```

---

##  Instrucciones de Ejecución

Para ejecutar este ejemplo en la terminal de comandos:

1. Ubícate en la raíz de tu espacio de trabajo.
2. Ejecuta el script principal con Python:

```bash
python "./Ejemplo 1/main.py"
```

### Salida Esperada en Pantalla

```text
============================================================
  EJEMPLO 1: INTRODUCCIÓN A LA POO - CLASES Y OBJETOS BÁSICOS
============================================================

[+] Estado inicial de los satélites creados:
Satélite: Starlink-Norte-A (ID: SAT-ECU-0001) -> Conectado a la IP: 127.0.0.1
Satélite: Starlink-Norte-B (ID: SAT-ECU-0002) -> Conectado a la IP: 10.0.0.50

[+] Modificando la dirección IP de red del satélite A...
[+] Modificando el nombre asignado al satélite B...

[+] Estado final de los satélites después de las modificaciones:
Satélite: Starlink-Norte-A (ID: SAT-ECU-0001) -> Conectado a la IP: 192.168.1.100
Satélite: Starlink-Norte-B-Modificado (ID: SAT-ECU-0002) -> Conectado a la IP: 10.0.0.50
============================================================
```

Este simple ejemplo sienta las bases para la sesión de la siguiente hora (Ejemplo 2), donde enseñaremos a los estudiantes por qué la modificación directa de atributos (como hicimos hoy) puede ser peligrosa y cómo resolverlo utilizando **Encapsulamiento Básico** (atributos protegidos y privados).
