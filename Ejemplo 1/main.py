from satelite import Satelite

def principal():
    print("=" * 60)
    print("  EJEMPLO 1: INTRODUCCIÓN A LA POO - CLASES Y OBJETOS BÁSICOS")
    print("=" * 60)

    # 1. Creación e instanciación de objetos de la clase Satelite
    # Se crean dos instancias independientes en memoria
    satelite_a = Satelite("SAT-ECU-0001", "Starlink-Norte-A", "127.0.0.1")
    satelite_b = Satelite("SAT-ECU-0002", "Starlink-Norte-B", "10.0.0.50")

    print("\n[+] Estado inicial de los satélites creados:")
    print(satelite_a.obtener_descripcion())
    print(satelite_b.obtener_descripcion())

    # 2. Demostración de modificación directa de atributos públicos
    # Al ser atributos públicos, se pueden leer y modificar directamente desde fuera de la clase
    print("\n[+] Modificando la dirección IP de red del satélite A...")
    satelite_a.enlace_ip = "192.168.1.100"

    print("[+] Modificando el nombre asignado al satélite B...")
    satelite_b.nombre = "Starlink-Norte-B-Modificado"

    # 3. Mostrar el estado actualizado de los objetos
    print("\n[+] Estado final de los satélites después de las modificaciones:")
    print(satelite_a.obtener_descripcion())
    print(satelite_b.obtener_descripcion())
    print("=" * 60 + "\n")


if __name__ == "__main__":
    principal()
