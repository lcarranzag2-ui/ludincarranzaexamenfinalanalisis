using EnviosRapidosGT.API.Models;

namespace EnviosRapidosGT.API.Services;

public interface IEnvioService
{
    Task<EnvioResponseDto> CrearEnvioAsync(CrearEnvioDto dto);
    Task<EnvioResponseDto?> ObtenerPorCodigoAsync(string codigo);
    Task<IEnumerable<EnvioResponseDto>> ObtenerTodosAsync();
    Task<(bool exito, string mensaje)> ActualizarEstadoAsync(string codigo, ActualizarEstadoDto dto);
    Task<IEnumerable<HistorialEstado>> ObtenerHistorialAsync(string codigo);
    Task<ReporteEficienciaDto> GenerarReporteEficienciaAsync();
    Task<(bool exito, string mensaje)> RegistrarIntentoFallidoAsync(string codigo, string ubicacion, string? notas);
}
