namespace EnviosRapidosGT.API.Models;

public class CrearEnvioDto
{
    public string Remitente { get; set; } = string.Empty;
    public string Destinatario { get; set; } = string.Empty;
    public string? NitRemitente { get; set; }
    public string? NitDestinatario { get; set; }
    public decimal PesoKg { get; set; }
    public string OficinaOrigen { get; set; } = string.Empty;
}

public class ActualizarEstadoDto
{
    public string NuevoEstado { get; set; } = string.Empty;
    public string Ubicacion { get; set; } = string.Empty;
    public string? Notas { get; set; }
}

public class EnvioResponseDto
{
    public int Id { get; set; }
    public string CodigoRastreo { get; set; } = string.Empty;
    public string Remitente { get; set; } = string.Empty;
    public string Destinatario { get; set; } = string.Empty;
    public decimal PesoKg { get; set; }
    public decimal Tarifa { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string OficinaActual { get; set; } = string.Empty;
    public int IntentosFallidos { get; set; }
    public DateTime FechaCreacion { get; set; }
}

public class ReporteEficienciaDto
{
    public int TotalEnvios { get; set; }
    public int TotalEntregados { get; set; }
    public int TotalDevueltos { get; set; }
    public int TotalEnProceso { get; set; }
    public decimal PorcentajeEficiencia { get; set; }
    public decimal IngresosTotales { get; set; }
}
