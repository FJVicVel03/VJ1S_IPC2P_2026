namespace Ejemplo_5.Models
{
    public class NodoLog
    {
        public LogRegistro Valor { get; set; }
        public NodoLog? Siguiente { get; set; }

        public NodoLog(LogRegistro log)
        {
            Valor = log;
            Siguiente = null;
        }
    }
}
