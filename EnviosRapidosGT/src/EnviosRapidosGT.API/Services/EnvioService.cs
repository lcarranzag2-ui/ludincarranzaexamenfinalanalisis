using Dapper;
using Microsoft.Data.Sqlite;
using EnviosRapidosGT.API.Models;
using EnviosRapidosGT.API.Helpers;

namespace EnviosRapidosGT.API.Services;

public class EnvioService : IEnvioService
{
    private readonly string _connectionString;

    public EnvioService(string connectionString)
    {
        _connectionString = connectionString;
    }

    private SqliteConnection GetConnection() => new SqliteConnection(_connectionString);

    public async Task<EnvioResponseDto> CrearEnvioAsync(CrearEnvioDto dto)
    {
        var tarifa = TarifaCalculator.Calcular(dto.PesoKg);

        bool tieneDescuento = TarifaCalculator.TieneNitValido(dto.NitRemitente)
                           || TarifaCalculator.TieneNitValido(dto.NitDestinatario);

        if (tieneDescuento)
            tarifa = TarifaCalculator.AplicarDescuentoNit(tarifa);

        var codigo = CodigoRastreoGenerator.Generar();

        var envio = new Envio
        {
            CodigoRastreo = codigo,
            Remitente = dto.Remitente,
            Destinatario = dto.Destinatario,
            NitRemitente = dto.NitRemitente,
            NitDestinatario = dto.NitDestinatario,
            PesoKg = dto.PesoKg,
            Tarifa = tarifa,
            Estado = EstadoEnvio.Registrado,
            OficinaActual = dto.OficinaOrigen,
            FechaCreacion = DateTime.UtcNow
        };

        using var conn = GetConnection();
        var sql = @"INSERT INTO Envios 
                    (CodigoRastreo, Remitente, Destinatario, NitRemitente, NitDestinatario, PesoKg, Tarifa, Estado, OficinaActual, IntentosFallidos, FechaCreacion)
                    VALUES (@CodigoRastreo, @Remitente, @Destinatario, @NitRemitente, @NitDestinatario, @PesoKg, @Tarifa, @Estado, @OficinaActual, @IntentosFallidos, @FechaCreacion);
                    SELECT last_insert_rowid();";

        envio.Id = await conn.ExecuteScalarAsync<int>(sql, envio);

        await RegistrarHistorialAsync(conn, envio.Id, "-", EstadoEnvio.Registrado, dto.OficinaOrigen, "Envio registrado en el sistema");

        return MapToDto(envio);
    }

    public async Task<EnvioResponseDto?> ObtenerPorCodigoAsync(string codigo)
    {
        using var conn = GetConnection();
        var envio = await conn.QueryFirstOrDefaultAsync<Envio>(
            "SELECT * FROM Envios WHERE CodigoRastreo = @codigo", new { codigo });

        return envio == null ? null : MapToDto(envio);
    }

    public async Task<IEnumerable<EnvioResponseDto>> ObtenerTodosAsync()
    {
        using var conn = GetConnection();
        var envios = await conn.QueryAsync<Envio>("SELECT * FROM Envios ORDER BY FechaCreacion DESC");
        return envios.Select(MapToDto);
    }

    public async Task<(bool exito, string mensaje)> ActualizarEstadoAsync(string codigo, ActualizarEstadoDto dto)
    {
        using var conn = GetConnection();
        var envio = await conn.QueryFirstOrDefaultAsync<Envio>(
            "SELECT * FROM Envios WHERE CodigoRastreo = @codigo", new { codigo });

        if (envio == null)
            return (false, "Envio no encontrado.");

        var error = EstadoValidator.ValidarCambioEstado(envio, dto.NuevoEstado);
        if (error != null)
            return (false, error);

        var estadoAnterior = envio.Estado;
        await conn.ExecuteAsync(
            "UPDATE Envios SET Estado = @estado, OficinaActual = @oficina WHERE CodigoRastreo = @codigo",
            new { estado = dto.NuevoEstado, oficina = dto.Ubicacion, codigo });

        await RegistrarHistorialAsync(conn, envio.Id, estadoAnterior, dto.NuevoEstado, dto.Ubicacion, dto.Notas);

        return (true, $"Estado actualizado a '{dto.NuevoEstado}' correctamente.");
    }

    public async Task<(bool exito, string mensaje)> RegistrarIntentoFallidoAsync(string codigo, string ubicacion, string? notas)
    {
        using var conn = GetConnection();
        var envio = await conn.QueryFirstOrDefaultAsync<Envio>(
            "SELECT * FROM Envios WHERE CodigoRastreo = @codigo", new { codigo });

        if (envio == null)
            return (false, "Envio no encontrado.");

        if (envio.Estado != EstadoEnvio.EnReparto)
            return (false, "Solo se pueden registrar intentos fallidos cuando el envio esta EnReparto.");

        envio.IntentosFallidos++;

        if (envio.IntentosFallidos >= 3)
        {
            await conn.ExecuteAsync(
                "UPDATE Envios SET IntentosFallidos = @intentos, Estado = @estado WHERE CodigoRastreo = @codigo",
                new { intentos = envio.IntentosFallidos, estado = EstadoEnvio.EnDevolucion, codigo });

            await RegistrarHistorialAsync(conn, envio.Id, EstadoEnvio.EnReparto, EstadoEnvio.EnDevolucion, ubicacion,
                $"Tercer intento fallido. {notas}");

            return (true, "Tercer intento fallido. El envio cambia automaticamente a EnDevolucion.");
        }

        await conn.ExecuteAsync(
            "UPDATE Envios SET IntentosFallidos = @intentos WHERE CodigoRastreo = @codigo",
            new { intentos = envio.IntentosFallidos, codigo });

        await RegistrarHistorialAsync(conn, envio.Id, EstadoEnvio.EnReparto, EstadoEnvio.EnReparto, ubicacion,
            $"Intento fallido #{envio.IntentosFallidos}. {notas}");

        return (true, $"Intento fallido #{envio.IntentosFallidos} registrado.");
    }

    public async Task<IEnumerable<HistorialEstado>> ObtenerHistorialAsync(string codigo)
    {
        using var conn = GetConnection();
        var envio = await conn.QueryFirstOrDefaultAsync<Envio>(
            "SELECT Id FROM Envios WHERE CodigoRastreo = @codigo", new { codigo });

        if (envio == null) return Enumerable.Empty<HistorialEstado>();

        return await conn.QueryAsync<HistorialEstado>(
            "SELECT * FROM HistorialEstados WHERE EnvioId = @id ORDER BY Timestamp ASC",
            new { id = envio.Id });
    }

    public async Task<ReporteEficienciaDto> GenerarReporteEficienciaAsync()
    {
        using var conn = GetConnection();

        var total = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Envios");
        var entregados = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Envios WHERE Estado = 'Entregado'");
        var devueltos = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Envios WHERE Estado = 'Devuelto'");
        var ingresos = await conn.ExecuteScalarAsync<decimal>("SELECT COALESCE(SUM(Tarifa), 0) FROM Envios");

        return new ReporteEficienciaDto
        {
            TotalEnvios = total,
            TotalEntregados = entregados,
            TotalDevueltos = devueltos,
            TotalEnProceso = total - entregados - devueltos,
            PorcentajeEficiencia = total > 0 ? Math.Round((decimal)entregados / total * 100, 2) : 0,
            IngresosTotales = ingresos
        };
    }

    private static async Task RegistrarHistorialAsync(SqliteConnection conn, int envioId,
        string estadoAnterior, string estadoNuevo, string ubicacion, string? notas)
    {
        var historial = new HistorialEstado
        {
            EnvioId = envioId,
            EstadoAnterior = estadoAnterior,
            EstadoNuevo = estadoNuevo,
            Ubicacion = ubicacion,
            Timestamp = DateTime.UtcNow,
            Notas = notas
        };

        await conn.ExecuteAsync(@"
            INSERT INTO HistorialEstados (EnvioId, EstadoAnterior, EstadoNuevo, Ubicacion, Timestamp, Notas)
            VALUES (@EnvioId, @EstadoAnterior, @EstadoNuevo, @Ubicacion, @Timestamp, @Notas)", historial);
    }

    private static EnvioResponseDto MapToDto(Envio e) => new()
    {
        Id = e.Id,
        CodigoRastreo = e.CodigoRastreo,
        Remitente = e.Remitente,
        Destinatario = e.Destinatario,
        PesoKg = e.PesoKg,
        Tarifa = e.Tarifa,
        Estado = e.Estado,
        OficinaActual = e.OficinaActual,
        IntentosFallidos = e.IntentosFallidos,
        FechaCreacion = e.FechaCreacion
    };
}
