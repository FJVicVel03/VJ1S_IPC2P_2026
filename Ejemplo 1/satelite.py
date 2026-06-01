class Satelite:
    """
    Representa un satélite básico de la red de telemetría.
    Este primer ejemplo introduce los conceptos de Clase, Objeto y Atributos Públicos.
    """
    def __init__(self, id_satelite: str, nombre: str, enlace_ip: str):
        # Atributos públicos (accesibles y modificables directamente desde fuera de la clase)
        self.id = id_satelite
        self.nombre = nombre
        self.enlace_ip = enlace_ip

    def obtener_descripcion(self) -> str:
        """
        Retorna una cadena con la descripción y estado actual del satélite.
        """
        return f"Satélite: {self.nombre} (ID: {self.id}) -> Conectado a la IP: {self.enlace_ip}"
