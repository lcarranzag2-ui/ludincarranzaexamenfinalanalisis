using EnviosRapidosGT.API.Models;

namespace EnviosRapidosGT.API.Helpers;

public static class EstadoValidator
{
    private static readonly Dictionary<string, List<string>> TransicionesValidas = new()
    {
        { EstadoEnvio.Registrado, new List<string> { EstadoEnvio.EnTransito } },
        { EstadoEnvio.EnTransito, new List<string> { EstadoEnvio.EnReparto, EstadoEnvio.Devuelto } },
        { EstadoEnvio.EnReparto, new List<string> { EstadoEnvio.Entregado, EstadoEnvio.EnDevolucion } },
        { EstadoEnvio.EnDevolucion, new List<string> { EstadoEnvio.Devuelto } },
        { EstadoEnvio.Entregado, new List<string>() },
        { EstadoEnvio.Devuelto, new List<string>() }
    };

    public static bool EsTransicionValida(string estadoActual, string estadoNuevo)
    {
        if (!TransicionesValidas.TryGetValue(estadoActual, out var permitidos))
            return false;
        return permitidos.Contains(estadoNuevo);
    }

    public static string? ValidarCambioEstado(Envio envio, string nuevoEstado)
    {
        if (!EsTransicionValida(envio.Estado, nuevoEstado))
            return $"No se puede cambiar de '{envio.Estado}' a '{nuevoEstado}'.";

        if (nuevoEstado == EstadoEnvio.EnDevolucion && envio.IntentosFallidos < 3)
            return "Solo se puede marcar EnDevolucion despues de 3 intentos fallidos.";

        return null;
    }
}
