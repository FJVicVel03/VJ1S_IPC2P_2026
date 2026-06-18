namespace Ejemplo_13.Models
{
    /// <summary>
    /// Modelo de vista unificado que expone la matriz dispersa, logs y el diagrama SVG compilado.
    /// </summary>
    public class DashboardViewModel
    {
        public RedSatelitalPlano Matriz { get; set; } = null!;
        public ListaLogs Logs { get; set; } = null!;
        public ArbolSatelitesAvl Catalogo { get; set; } = null!;
        public string SvgDiagrama { get; set; } = "";
    }
}




