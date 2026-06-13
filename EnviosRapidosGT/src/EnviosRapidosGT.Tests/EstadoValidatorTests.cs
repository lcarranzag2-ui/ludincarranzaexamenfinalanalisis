using Xunit;
using EnviosRapidosGT.API.Helpers;
using EnviosRapidosGT.API.Models;

namespace EnviosRapidosGT.Tests;

public class EstadoValidatorTests
{
    [Fact]
    public void EsTransicionValida_RegistradoAEnTransito_RetornaTrue()
    {
        var resultado = EstadoValidator.EsTransicionValida(EstadoEnvio.Registrado, EstadoEnvio.EnTransito);
        Assert.True(resultado);
    }

    [Fact]
    public void EsTransicionValida_RegistradoAEntregado_RetornaFalse()
    {
        var resultado = EstadoValidator.EsTransicionValida(EstadoEnvio.Registrado, EstadoEnvio.Entregado);
        Assert.False(resultado);
    }

    [Fact]
    public void EsTransicionValida_EnTransitoAEnReparto_RetornaTrue()
    {
        var resultado = EstadoValidator.EsTransicionValida(EstadoEnvio.EnTransito, EstadoEnvio.EnReparto);
        Assert.True(resultado);
    }

    [Fact]
    public void EsTransicionValida_EnRepartoAEntregado_RetornaTrue()
    {
        var resultado = EstadoValidator.EsTransicionValida(EstadoEnvio.EnReparto, EstadoEnvio.Entregado);
        Assert.True(resultado);
    }

    [Fact]
    public void EsTransicionValida_EntregadoACualquierEstado_RetornaFalse()
    {
        Assert.False(EstadoValidator.EsTransicionValida(EstadoEnvio.Entregado, EstadoEnvio.EnTransito));
        Assert.False(EstadoValidator.EsTransicionValida(EstadoEnvio.Entregado, EstadoEnvio.EnReparto));
    }

    [Fact]
    public void EsTransicionValida_DevueltoACualquierEstado_RetornaFalse()
    {
        Assert.False(EstadoValidator.EsTransicionValida(EstadoEnvio.Devuelto, EstadoEnvio.Registrado));
    }

    [Fact]
    public void ValidarCambioEstado_EnDevolucionSinIntentosSuficientes_RetornaError()
    {
        var envio = new Envio { Estado = EstadoEnvio.EnReparto, IntentosFallidos = 1 };
        var error = EstadoValidator.ValidarCambioEstado(envio, EstadoEnvio.EnDevolucion);
        Assert.NotNull(error);
        Assert.Contains("3 intentos", error);
    }

    [Fact]
    public void ValidarCambioEstado_TransicionInvalida_RetornaError()
    {
        var envio = new Envio { Estado = EstadoEnvio.Registrado, IntentosFallidos = 0 };
        var error = EstadoValidator.ValidarCambioEstado(envio, EstadoEnvio.Entregado);
        Assert.NotNull(error);
    }

    [Fact]
    public void ValidarCambioEstado_TransicionValida_RetornaNull()
    {
        var envio = new Envio { Estado = EstadoEnvio.Registrado, IntentosFallidos = 0 };
        var error = EstadoValidator.ValidarCambioEstado(envio, EstadoEnvio.EnTransito);
        Assert.Null(error);
    }
}
