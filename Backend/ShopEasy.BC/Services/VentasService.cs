using ShopEasy.BC.Common;
using ShopEasy.BW.Dashboard;
using ShopEasy.DA.Data;

namespace ShopEasy.BC.Services;

public class VentasService
{
    private const int PeriodoMinimo = 1;
    private const int PeriodoMaximo = 365;

    private readonly VentasDA _ventasDA;

    public VentasService(VentasDA ventasDA)
    {
        _ventasDA = ventasDA;
    }

    public async Task<VentasResponse> ObtenerVentasAsync(int dias)
    {
        if (dias < PeriodoMinimo || dias > PeriodoMaximo)
        {
            throw new ApiException(400, "El periodo debe estar entre 1 y 365 dias.");
        }

        var hoy = DateTime.Today;
        var desde = hoy.AddDays(-(dias - 1));
        var hastaExclusivo = hoy.AddDays(1);

        var ventasPorDia = await _ventasDA.ObtenerVentasPorDiaAsync(desde, hastaExclusivo);
        var ventasUltimos7Dias = await _ventasDA.SumarVentasEntreAsync(hoy.AddDays(-6), hastaExclusivo);
        var ventasUltimos30Dias = await _ventasDA.SumarVentasEntreAsync(hoy.AddDays(-29), hastaExclusivo);

        var mapa = ventasPorDia.ToDictionary(
            venta => DateOnly.FromDateTime(venta.Fecha),
            venta => venta);

        var serie = new List<VentaPorDiaDto>(dias);

        for (var fecha = desde; fecha <= hoy; fecha = fecha.AddDays(1))
        {
            var dia = DateOnly.FromDateTime(fecha);

            serie.Add(mapa.TryGetValue(dia, out var venta)
                ? new VentaPorDiaDto(dia, venta.CantidadPedidos, venta.MontoTotal)
                : new VentaPorDiaDto(dia, 0, 0m));
        }

        return new VentasResponse(
            dias,
            DateOnly.FromDateTime(desde),
            DateOnly.FromDateTime(hoy),
            serie.Sum(dia => dia.MontoTotal),
            serie.Sum(dia => dia.CantidadPedidos),
            ventasUltimos7Dias,
            ventasUltimos30Dias,
            serie);
    }
}
