using System;
using System.Collections.Generic;

namespace Ejemplo_3.Models
{
    /// <summary>
    /// Colección manual de tipo Lista Enlazada Simple para Satélites.
    /// Respeta la restricción de no utilizar Listas ni Diccionarios nativos de .NET para el almacenamiento.
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

        /// <summary>
        /// Inserta un nuevo satélite al final de la lista manual.
        /// </summary>
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
        /// Generador que recorre secuencialmente la lista por referencias (punteros).
        /// Permite utilizar ciclos foreach en el controlador y vistas.
        /// </summary>
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
