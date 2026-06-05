using System;
using System.Collections.Generic;

namespace Ejemplo_5.Models
{
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
