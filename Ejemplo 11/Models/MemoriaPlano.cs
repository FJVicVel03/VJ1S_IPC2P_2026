using System;

namespace Ejemplo_11.Models
{
    /// <summary>
    /// Clase estática global que almacena en memoria el estado del plano satelital (Matriz Dispersa)
    /// y de la bitácora de auditoría. Permite a los controladores acceder de manera unificada.
    /// </summary>
    public static class MemoriaPlano
    {
        public static RedSatelitalPlano Matriz { get; } = new RedSatelitalPlano();
        public static ListaLogs Logs { get; } = new ListaLogs();
        public static ArbolSatelitesAvl Catalogo { get; } = new ArbolSatelitesAvl();
    }
}


