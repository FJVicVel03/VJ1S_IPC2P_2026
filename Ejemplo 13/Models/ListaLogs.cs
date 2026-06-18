namespace Ejemplo_13.Models
{
    /// <summary>
    /// Lista enlazada manual para almacenar la bitácora de logs sin usar System.Collections.Generic.
    /// </summary>
    public class ListaLogs
    {
        private NodoLog? cabeza;
        private int tamano;

        public ListaLogs()
        {
            cabeza = null;
            tamano = 0;
        }

        public int Tamano => tamano;
        public bool EstaVacia => cabeza == null;

        public void InsertarAlFinal(LogRegistro log)
        {
            NodoLog nuevoNodo = new NodoLog(log);

            if (EstaVacia)
            {
                cabeza = nuevoNodo;
            }
            else
            {
                NodoLog actual = cabeza!;
                while (actual.Siguiente != null)
                {
                    actual = actual.Siguiente;
                }
                actual.Siguiente = nuevoNodo;
            }
            tamano++;
        }

        public void Registrar(string tipo, string mensaje)
        {
            LogRegistro nuevoLog = new LogRegistro(tipo, mensaje);
            InsertarAlFinal(nuevoLog);
        }

        public void Limpiar()
        {
            cabeza = null;
            tamano = 0;
        }

        // Retorna un arreglo nativo de C# para iteración limpia en la vista
        public LogRegistro[] ObtenerTodos()
        {
            LogRegistro[] arr = new LogRegistro[tamano];
            NodoLog? actual = cabeza;
            int i = 0;
            while (actual != null && i < tamano)
            {
                arr[i] = actual.Valor;
                actual = actual.Siguiente;
                i++;
            }
            return arr;
        }
    }
}




