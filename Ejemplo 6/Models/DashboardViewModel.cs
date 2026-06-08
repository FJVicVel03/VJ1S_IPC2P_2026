namespace Ejemplo_6.Models
{
    /// <summary>
    /// Modelo de vista unificado que expone únicamente el árbol AVL y la bitácora de logs.
    /// </summary>
    public class DashboardViewModel
    {
        public ArbolSatelitesAvl Satelites { get; set; } = null!;
        public ListaLogs Logs { get; set; } = null!;
    }
}
