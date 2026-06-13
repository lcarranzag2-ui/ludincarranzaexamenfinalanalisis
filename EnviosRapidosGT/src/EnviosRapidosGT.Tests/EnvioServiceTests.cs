using Xunit;
using EnviosRapidosGT.API.Services;
using EnviosRapidosGT.API.Models;
using EnviosRapidosGT.API.Data;

namespace EnviosRapidosGT.Tests;

public class EnvioServiceTests
{
    private readonly EnvioService _service;
    private readonly string _connectionString;

    public EnvioServiceTests()
    {
        _connectionString = $"Data Source=file:test_{Guid.NewGuid()}?mode=memory&cache=shared";
        var dbInit = new DatabaseInitializer(_connectionString);
        dbInit.Initialize();
        _service = new EnvioService(_connectionString);
    }

    private CrearEnvioDto CrearDtoBasico(decimal peso = 2m) => new()
    {
        Remitente = "Juan Perez",
        Destinatario = "Maria Lopez",
        PesoKg = peso,
        OficinaOrigen = "Oficina Central Guatemala"
    };

    [Fact]
    public async Task CrearEnvio_DatosValidos_RetornaEnvioConCodigoRastreo()
    {
        var dto = CrearDtoBasico();
        var resultado = await _service.CrearEnvioAsync(dto);

        Assert.NotNull(resultado);
        Assert.StartsWith("ENV-", resultado.CodigoRastreo);
        Assert.Equal(EstadoEnvio.Registrado, resultado.Estado);
    }

    [Fact]
    public async Task CrearEnvio_Peso2Kg_TarifaQ45()
    {
        var dto = CrearDtoBasico(2m);
        var resultado = await _service.CrearEnvioAsync(dto);
        Assert.Equal(45.00m, resultado.Tarifa);
    }

    [Fact]
    public async Task CrearEnvio_ConNitValido_AplicaDescuentoDel5Porciento()
    {
        var dto = CrearDtoBasico(2m);
        dto.NitRemitente = "1234567-8";

        var resultado = await _service.CrearEnvioAsync(dto);
        Assert.Equal(42.75m, resultado.Tarifa);
    }

    [Fact]
    public async Task ObtenerPorCodigo_CodigoExistente_RetornaEnvio()
    {
        var creado = await _service.CrearEnvioAsync(CrearDtoBasico());
        var encontrado = await _service.ObtenerPorCodigoAsync(creado.CodigoRastreo);

        Assert.NotNull(encontrado);
        Assert.Equal(creado.CodigoRastreo, encontrado.CodigoRastreo);
    }

    [Fact]
    public async Task ObtenerPorCodigo_CodigoInexistente_RetornaNull()
    {
        var resultado = await _service.ObtenerPorCodigoAsync("ENV-00000000-0000");
        Assert.Null(resultado);
    }

    [Fact]
    public async Task ActualizarEstado_TransicionValida_RetornaExito()
    {
        var creado = await _service.CrearEnvioAsync(CrearDtoBasico());

        var (exito, mensaje) = await _service.ActualizarEstadoAsync(creado.CodigoRastreo, new ActualizarEstadoDto
        {
            NuevoEstado = EstadoEnvio.EnTransito,
            Ubicacion = "Bodega Central"
        });

        Assert.True(exito);
        Assert.Contains("EnTransito", mensaje);
    }

    [Fact]
    public async Task ActualizarEstado_TransicionInvalida_RetornaError()
    {
        var creado = await _service.CrearEnvioAsync(CrearDtoBasico());

        var (exito, mensaje) = await _service.ActualizarEstadoAsync(creado.CodigoRastreo, new ActualizarEstadoDto
        {
            NuevoEstado = EstadoEnvio.Entregado,
            Ubicacion = "Oficina Jalapa"
        });

        Assert.False(exito);
        Assert.NotEmpty(mensaje);
    }

    [Fact]
    public async Task RegistrarIntentoFallido_TresIntentos_CambiaAEnDevolucion()
    {
        var creado = await _service.CrearEnvioAsync(CrearDtoBasico());

        await _service.ActualizarEstadoAsync(creado.CodigoRastreo, new ActualizarEstadoDto
        { NuevoEstado = EstadoEnvio.EnTransito, Ubicacion = "Bodega" });

        await _service.ActualizarEstadoAsync(creado.CodigoRastreo, new ActualizarEstadoDto
        { NuevoEstado = EstadoEnvio.EnReparto, Ubicacion = "Zona 1" });

        await _service.RegistrarIntentoFallidoAsync(creado.CodigoRastreo, "Zona 1", "No habia nadie");
        await _service.RegistrarIntentoFallidoAsync(creado.CodigoRastreo, "Zona 1", "Direccion incorrecta");
        await _service.RegistrarIntentoFallidoAsync(creado.CodigoRastreo, "Zona 1", "Tercero fallido");

        var envio = await _service.ObtenerPorCodigoAsync(creado.CodigoRastreo);
        Assert.Equal(EstadoEnvio.EnDevolucion, envio!.Estado);
    }

    [Fact]
    public async Task ObtenerHistorial_EnvioConCambios_RetornaHistorialCompleto()
    {
        var creado = await _service.CrearEnvioAsync(CrearDtoBasico());

        await _service.ActualizarEstadoAsync(creado.CodigoRastreo, new ActualizarEstadoDto
        { NuevoEstado = EstadoEnvio.EnTransito, Ubicacion = "Bodega Central" });

        var historial = (await _service.ObtenerHistorialAsync(creado.CodigoRastreo)).ToList();

        Assert.True(historial.Count >= 2);
    }

    [Fact]
    public async Task GenerarReporteEficiencia_SinEnvios_RetornaCerosPorcentaje()
    {
        var reporte = await _service.GenerarReporteEficienciaAsync();
        Assert.Equal(0, reporte.PorcentajeEficiencia);
        Assert.Equal(0, reporte.TotalEnvios);
    }
}
