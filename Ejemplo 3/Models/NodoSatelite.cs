namespace Ejemplo_3.Models
{
    /// <summary>
    /// Representa un nodo enlazado que encapsula un Satelite y una referencia al siguiente nodo.
    /// </summary>
    public class NodoSatelite
    {
        public Satelite Valor { get; set; }
        public NodoSatelite? Siguiente { get; set; }

        public NodoSatelite(Satelite satelite)
        {
            Valor = satelite;
            Siguiente = null;
        }
    }
}
