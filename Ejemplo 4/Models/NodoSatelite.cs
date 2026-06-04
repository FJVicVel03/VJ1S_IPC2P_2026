namespace Ejemplo_4.Models
{
    /// <summary>
    /// Representa un nodo enlazado para la lista simple.
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
