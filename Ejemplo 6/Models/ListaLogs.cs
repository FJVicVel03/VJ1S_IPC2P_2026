using System.Collections.Generic;

namespace Ejemplo_6.Models
{
    /// <summary>
    /// Lista enlazada manual para almacenar el historial de auditoría de logs.
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

        public IEnumerable<LogRegistro> Recorrer()
        {
            NodoLog? actual = cabeza;
            while (actual != null)
            {
                yield return actual.Valor;
                actual = actual.Siguiente;
            }
        }
    }
}
