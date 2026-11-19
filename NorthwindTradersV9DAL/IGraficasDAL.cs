using NorthwindTradersV9Entities.DTOs;
using System.Data;

namespace NorthwindTradersV9DAL
{
    public interface IGraficasDAL
    {
        DataTable ObtenerTop10AñosDeVentas();
        List<DtoVentasMensuales> ObtenerVentasMensuales(int year);
        int ObtenerTotalAñosConVentas();
        List<DtoVentasMensualesPorAños> ObtenerVentasMensualesPorAños(int years);
        DataTable ObtenerTopProductos(int cantidad, int anio);
        List<(string Vendedor, decimal TotalVentas)> ObtenerVentasPorVendedores(int anio);
        DataTable ObtenerVentasMensualesPorVendedoresPorAño(int anio);
    }
}
