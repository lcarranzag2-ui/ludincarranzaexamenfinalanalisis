using Microsoft.AspNetCore.Mvc;
using EnviosRapidosGT.API.Models;
using EnviosRapidosGT.API.Services;

namespace EnviosRapidosGT.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnviosController : ControllerBase
{
    private readonly IEnvioService _envioService;

    public EnviosController(IEnvioService envioService)
    {
        _envioService = envioService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(EnvioResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CrearEnvio([FromBody] CrearEnvioDto dto)
    {
        if (dto.PesoKg <= 0)
            return BadRequest("El peso debe ser mayor a 0 kg.");

        if (string.IsNullOrWhiteSpace(dto.Remitente) || string.IsNullOrWhiteSpace(dto.Destinatario))
            return BadRequest("Remitente y destinatario son obligatorios.");

        var resultado = await _envioService.CrearEnvioAsync(dto);
        return CreatedAtAction(nameof(ObtenerPorCodigo), new { codigo = resultado.CodigoRastreo }, resultado);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EnvioResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerTodos()
    {
        var envios = await _envioService.ObtenerTodosAsync();
        return Ok(envios);
    }

    [HttpGet("{codigo}")]
    [ProducesResponseType(typeof(EnvioResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorCodigo(string codigo)
    {
        var envio = await _envioService.ObtenerPorCodigoAsync(codigo);
        if (envio == null) return NotFound($"No se encontro el envio con codigo '{codigo}'.");
        return Ok(envio);
    }

    [HttpPut("{codigo}/estado")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActualizarEstado(string codigo, [FromBody] ActualizarEstadoDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.NuevoEstado) || string.IsNullOrWhiteSpace(dto.Ubicacion))
            return BadRequest("NuevoEstado y Ubicacion son obligatorios.");

        var (exito, mensaje) = await _envioService.ActualizarEstadoAsync(codigo, dto);

        if (!exito)
        {
            if (mensaje.Contains("no encontrado")) return NotFound(mensaje);
            return BadRequest(mensaje);
        }

        return Ok(new { mensaje });
    }

    [HttpPost("{codigo}/intento-fallido")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RegistrarIntentoFallido(string codigo, [FromBody] IntentoFallidoDto dto)
    {
        var (exito, mensaje) = await _envioService.RegistrarIntentoFallidoAsync(codigo, dto.Ubicacion, dto.Notas);

        if (!exito)
        {
            if (mensaje.Contains("no encontrado")) return NotFound(mensaje);
            return BadRequest(mensaje);
        }

        return Ok(new { mensaje });
    }

    [HttpGet("{codigo}/historial")]
    [ProducesResponseType(typeof(IEnumerable<HistorialEstado>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerHistorial(string codigo)
    {
        var historial = await _envioService.ObtenerHistorialAsync(codigo);
        return Ok(historial);
    }

    [HttpGet("reporte/eficiencia")]
    [ProducesResponseType(typeof(ReporteEficienciaDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerReporteEficiencia()
    {
        var reporte = await _envioService.GenerarReporteEficienciaAsync();
        return Ok(reporte);
    }
}

public class IntentoFallidoDto
{
    public string Ubicacion { get; set; } = string.Empty;
    public string? Notas { get; set; }
}
