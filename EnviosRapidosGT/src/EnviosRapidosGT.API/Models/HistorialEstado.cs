namespace EnviosRapidosGT.API.Models;

public class HistorialEstado
{
    public int Id { get; set; }
    public int EnvioId { get; set; }
    public string EstadoAnterior { get; set; } = string.Empty;
    public string EstadoNuevo { get; set; } = string.Empty;
    public string Ubicacion { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? Notas { get; set; }
}
