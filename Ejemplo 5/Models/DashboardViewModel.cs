namespace Ejemplo_5.Models
{
    /// <summary>
    /// Modelo de vista unificado que expone tanto los satélites activos como la bitácora de logs.
    /// </summary>
    public class DashboardViewModel
    {
        public ListaSatelites Satelites { get; set; } = null!;
        public ListaLogs Logs { get; set; } = null!;
    }
}
