using Microsoft.Extensions.Options;
using NorthwindTradersV9DAL;
using NorthwindTradersV9Entities;
using NorthwindTradersV9Entities.DTOs;
using System.Data;

namespace NorthwindTradersV9BLL
{
    public class VentaBLL
    {
        private readonly IVentaDAL _ventaDAL;
        private readonly AppSettings _appSettings;
        public VentaBLL(IVentaDAL ventaDAL, IOptions<AppSettings> appSettings)
        {
            _ventaDAL = ventaDAL;
            _appSettings = appSettings.Value;
        }
        public VentaPaginadoDto ObtenerVentasPaginadasConBusqueda(
            int pageIndex,
            int pageSize,
            VentasBuscarDto filtro)
        {
            if (_appSettings.EjecutarTiempoDemora)
                Thread.Sleep(_appSettings.TiempoDemora);
            return _ventaDAL.ObtenerVentasPaginadasConBusqueda(
                pageIndex,
                pageSize,
                filtro);
        }
        public VentaDto? ObtenerVentaPorId(int id)
        {
            if (_appSettings.EjecutarTiempoDemora)
                Thread.Sleep(_appSettings.TiempoDemora);
            return _ventaDAL.ObtenerVentaPorId(id);
        }
        public DataTable ObtenerVentaPorIdDt(int orderId)
        {
            Venta venta = _ventaDAL.ObtenerVentaPorIdRpt(orderId);
            DataTable dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("Cliente", typeof(string));
            dt.Columns.Add("Vendedor", typeof(string));
            dt.Columns.Add("FechaDePedido", typeof(DateTime));
            dt.Columns.Add("FechaRequerido", typeof(DateTime));
            dt.Columns.Add("FechaDeEnvio", typeof(DateTime));
            dt.Columns.Add("CompaniaTransportista", typeof(string));
            dt.Columns.Add("DirigidoA", typeof(string));
            dt.Columns.Add("Domicilio", typeof(string));
            dt.Columns.Add("Ciudad", typeof(string));
            dt.Columns.Add("Region", typeof(string));
            dt.Columns.Add("CodigoPostal", typeof(string));
            dt.Columns.Add("Pais", typeof(string));
            dt.Columns.Add("Flete", typeof(decimal));
            DataRow dr = dt.NewRow();
            dr["Id"] = venta.OrderID;
            dr["Cliente"] = venta.Cliente.CompanyName;
            dr["Vendedor"] = venta.Empleado.NameByLastName;
            dr["FechaDePedido"] = venta.OrderDate ?? (object)DBNull.Value;
            dr["FechaRequerido"] = venta.RequiredDate ?? (object)DBNull.Value;
            dr["FechaDeEnvio"] = venta.ShippedDate ?? (object)DBNull.Value;
            dr["CompaniaTransportista"] = venta.Transportista.CompanyName;
            dr["DirigidoA"] = venta.ShipName;
            dr["Domicilio"] = venta.ShipAddress;
            dr["Ciudad"] = venta.ShipCity;
            dr["Region"] = venta.ShipRegion;
            dr["CodigoPostal"] = venta.ShipPostalCode;
            dr["Pais"] = venta.ShipCountry;
            dr["Flete"] = venta.Freight;
            dt.Rows.Add(dr);
            return dt;
        }
    }
}
