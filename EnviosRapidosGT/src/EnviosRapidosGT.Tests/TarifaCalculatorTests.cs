using Xunit;
using EnviosRapidosGT.API.Helpers;

namespace EnviosRapidosGT.Tests;

public class TarifaCalculatorTests
{
    [Fact]
    public void Calcular_PesoMenorOIgualA1Kg_RetornaQ25()
    {
        var tarifa = TarifaCalculator.Calcular(1m);
        Assert.Equal(25.00m, tarifa);
    }

    [Fact]
    public void Calcular_Peso0_5Kg_RetornaQ25()
    {
        var tarifa = TarifaCalculator.Calcular(0.5m);
        Assert.Equal(25.00m, tarifa);
    }

    [Fact]
    public void Calcular_Peso3Kg_RetornaQ45()
    {
        var tarifa = TarifaCalculator.Calcular(3m);
        Assert.Equal(45.00m, tarifa);
    }

    [Fact]
    public void Calcular_Peso5Kg_RetornaQ45()
    {
        var tarifa = TarifaCalculator.Calcular(5m);
        Assert.Equal(45.00m, tarifa);
    }

    [Fact]
    public void Calcular_Peso7Kg_RetornaQ75()
    {
        var tarifa = TarifaCalculator.Calcular(7m);
        Assert.Equal(75.00m, tarifa);
    }

    [Fact]
    public void Calcular_Peso10Kg_RetornaQ75()
    {
        var tarifa = TarifaCalculator.Calcular(10m);
        Assert.Equal(75.00m, tarifa);
    }

    [Fact]
    public void Calcular_Peso15Kg_RetornaQ100()
    {
        var tarifa = TarifaCalculator.Calcular(15m);
        Assert.Equal(100.00m, tarifa);
    }

    [Fact]
    public void AplicarDescuentoNit_TarifaQ100_RetornaQ95()
    {
        var tarifaConDescuento = TarifaCalculator.AplicarDescuentoNit(100m);
        Assert.Equal(95.00m, tarifaConDescuento);
    }

    [Fact]
    public void TieneNitValido_NitNulo_RetornaFalse()
    {
        Assert.False(TarifaCalculator.TieneNitValido(null));
    }

    [Fact]
    public void TieneNitValido_NitVacio_RetornaFalse()
    {
        Assert.False(TarifaCalculator.TieneNitValido(""));
    }

    [Fact]
    public void TieneNitValido_NitValido_RetornaTrue()
    {
        Assert.True(TarifaCalculator.TieneNitValido("1234567"));
    }

    [Fact]
    public void TieneNitValido_NitCorto_RetornaFalse()
    {
        Assert.False(TarifaCalculator.TieneNitValido("123"));
    }
}
