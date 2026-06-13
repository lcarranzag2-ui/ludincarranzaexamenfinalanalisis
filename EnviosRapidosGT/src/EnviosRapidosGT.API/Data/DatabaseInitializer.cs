using Microsoft.Data.Sqlite;

namespace EnviosRapidosGT.API.Data;

public class DatabaseInitializer
{
    private readonly string _connectionString;

    public DatabaseInitializer(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Initialize()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var createEnvios = @"
            CREATE TABLE IF NOT EXISTS Envios (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                CodigoRastreo TEXT NOT NULL UNIQUE,
                Remitente TEXT NOT NULL,
                Destinatario TEXT NOT NULL,
                NitRemitente TEXT,
                NitDestinatario TEXT,
                PesoKg REAL NOT NULL,
                Tarifa REAL NOT NULL,
                Estado TEXT NOT NULL DEFAULT 'Registrado',
                OficinaActual TEXT NOT NULL,
                IntentosFallidos INTEGER NOT NULL DEFAULT 0,
                FechaCreacion TEXT NOT NULL
            );";

        var createHistorial = @"
            CREATE TABLE IF NOT EXISTS HistorialEstados (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                EnvioId INTEGER NOT NULL,
                EstadoAnterior TEXT NOT NULL,
                EstadoNuevo TEXT NOT NULL,
                Ubicacion TEXT NOT NULL,
                Timestamp TEXT NOT NULL,
                Notas TEXT,
                FOREIGN KEY (EnvioId) REFERENCES Envios(Id)
            );";

        using var cmd1 = new SqliteCommand(createEnvios, connection);
        cmd1.ExecuteNonQuery();

        using var cmd2 = new SqliteCommand(createHistorial, connection);
        cmd2.ExecuteNonQuery();
    }
}
