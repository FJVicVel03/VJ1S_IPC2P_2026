using System;
using System.Collections.Generic;

namespace Ejemplo_4.Models
{
    /// <summary>
    /// Colección manual de tipo Lista Enlazada Simple para Satélites.
    /// </summary>
    public class ListaSatelites
    {
        private NodoSatelite? cabeza;
        private int tamano;

        public ListaSatelites()
        {
            cabeza = null;
            tamano = 0;
        }

        public int Tamano => tamano;
        public bool EstaVacia => cabeza == null;

        public void InsertarAlFinal(Satelite satelite)
        {
            NodoSatelite nuevoNodo = new NodoSatelite(satelite);

            if (EstaVacia)
            {
                cabeza = nuevoNodo;
            }
            else
            {
                NodoSatelite actual = cabeza!;
                while (actual.Siguiente != null)
                {
                    actual = actual.Siguiente;
                }
                actual.Siguiente = nuevoNodo;
            }
            tamano++;
        }

        /// <summary>
        /// Vacía la lista para permitir limpiar el estado en memoria RAM.
        /// </summary>
        public void Limpiar()
        {
            cabeza = null;
            tamano = 0;
        }

        public IEnumerable<Satelite> Recorrer()
        {
            NodoSatelite? actual = cabeza;
            while (actual != null)
            {
                yield return actual.Valor;
                actual = actual.Siguiente;
            }
        }
    }
}
