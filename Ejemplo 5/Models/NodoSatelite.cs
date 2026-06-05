namespace Ejemplo_5.Models
{
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
