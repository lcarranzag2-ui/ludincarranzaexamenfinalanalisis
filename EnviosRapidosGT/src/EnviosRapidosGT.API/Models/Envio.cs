namespace EnviosRapidosGT.API.Models;

public class Envio
{
    public int Id { get; set; }
    public string CodigoRastreo { get; set; } = string.Empty;
    public string Remitente { get; set; } = string.Empty;
    public string Destinatario { get; set; } = string.Empty;
    public string? NitRemitente { get; set; }
    public string? NitDestinatario { get; set; }
    public decimal PesoKg { get; set; }
    public decimal Tarifa { get; set; }
    public string Estado { get; set; } = EstadoEnvio.Registrado;
    public string OficinaActual { get; set; } = string.Empty;
    public int IntentosFallidos { get; set; } = 0;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}

public static class EstadoEnvio
{
    public const string Registrado = "Registrado";
    public const string EnTransito = "EnTransito";
    public const string EnReparto = "EnReparto";
    public const string Entregado = "Entregado";
    public const string Devuelto = "Devuelto";
    public const string EnDevolucion = "EnDevolucion";
}
