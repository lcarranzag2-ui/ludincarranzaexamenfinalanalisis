namespace EnviosRapidosGT.API.Helpers;

public static class TarifaCalculator
{
    public static decimal Calcular(decimal pesoKg)
    {
        if (pesoKg <= 1) return 25.00m;
        if (pesoKg <= 5) return 45.00m;
        if (pesoKg <= 10) return 75.00m;
        return 100.00m;
    }

    public static decimal AplicarDescuentoNit(decimal tarifa)
    {
        return tarifa * 0.95m;
    }

    public static bool TieneNitValido(string? nit)
    {
        if (string.IsNullOrWhiteSpace(nit)) return false;
        return nit.Length >= 7;
    }
}

public static class CodigoRastreoGenerator
{
    public static string Generar()
    {
        var fecha = DateTime.UtcNow.ToString("yyyyMMdd");
        var secuencia = new Random().Next(1000, 9999);
        return $"ENV-{fecha}-{secuencia}";
    }
}
