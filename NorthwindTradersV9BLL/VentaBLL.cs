using Microsoft.Extensions.Options;
using NorthwindTradersV9Common;
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
        public ResultadoOperacion Eliminar(VentaDto venta)
        {
            var resultado = new ResultadoOperacion();
            string productoExcede = "";
            int numRegs = _ventaDAL.Eliminar(venta, out productoExcede);
            resultado.Codigo = numRegs;
            if (numRegs > 0)
                resultado.Exito = true;
            else if (numRegs == -1)
                resultado.Mensaje = StringsCommons.Nfefe;
            else if (numRegs == -2)
                resultado.Mensaje = StringsCommons.Nfefm;
            else if (numRegs == -7)
                resultado.Mensaje = $"No fue eliminada de la base de datos, el nuevo inventario del producto {productoExcede}, excedió el límite máximo que se puede almacenar en la base de datos (32,767 unidades)";
            else if (numRegs == -8)
                resultado.Mensaje = $"No fue eliminada de la base de datos, el nuevo inventario del producto {productoExcede}, sería invalido (negativo)";
            else
                resultado.Mensaje = StringsCommons.Nfemd;
            if (_appSettings.EjecutarTiempoDemora)
                Thread.Sleep(_appSettings.TiempoDemora);
            return resultado;
        }
        public decimal? ObtenerTasaVigente(DateTime fecha)
        {
            return _ventaDAL.ObtenerTasaVigente(fecha);
        }
        public EnvioInformacionDto? ObtenerUltimaInformacionDeEnvio(string customerId)
        {
            return _ventaDAL.ObtenerUltimaInformacionDeEnvio(customerId);
        }
        public List<EnvioInformacionDto> ObtenerFormasEnvio(string customerId)
        {
            return _ventaDAL.ObtenerFormasEnvio(customerId);
        }
        public ResultadoOperacion InsertarVentaCompleta(Venta venta, out int orderId, out byte[] rowVersion)
        {
            var resultado = new ResultadoOperacion();
            int numRegs = _ventaDAL.InsertarVentaCompleta(venta, out orderId, out rowVersion);
            resultado.Codigo = numRegs;
            if (numRegs > 0)
                resultado.Exito = true;
            else
                resultado.Mensaje = StringsCommons.Nfrs;
            if (_appSettings.EjecutarTiempoDemora)
                Thread.Sleep(_appSettings.TiempoDemora);
            return resultado;
        }
        public Venta? ObtenerVentaPorId2(int orderId)
        {
            if (_appSettings.EjecutarTiempoDemora)
                Thread.Sleep(_appSettings.TiempoDemora);
            return _ventaDAL.ObtenerVentaPorId2(orderId);
        }
        public (int Codigo, byte[]? RowVersion) ActualizarEncabezado2(ActualizarEncabezadoRequest request)
        {
            var rowVersion = Convert.FromBase64String(request.RowVersion);
            DateTime? orderDate = CombinarFechaHora(
                request.OrderDate,
                request.OrderTime);
            DateTime? requiredDate = CombinarFechaHora(
                request.RequiredDate,
                request.RequiredTime);
            DateTime? shippedDate = CombinarFechaHora(
                request.ShippedDate,
                request.ShippedTime);
            return _ventaDAL.ActualizarEncabezado2(
                request.OrderID,
                request.CustomerID,
                request.EmployeeID,
                orderDate,
                requiredDate,
                shippedDate,
                rowVersion);
        }
        private DateTime? CombinarFechaHora(
            DateTime? fecha,
            string? hora)
        {
            if (!fecha.HasValue)
                return null;
            if (TimeSpan.TryParse(hora, out TimeSpan tiempo))
            {
                return fecha.Value.Date.Add(tiempo);
            }
            return fecha.Value.Date;
        }
        public (int Codigo, byte[]? RowVersion) ActualizarEnvio2(
            ActualizarEnvioRequest request)
        {
            var rowVersion =
                Convert.FromBase64String(request.RowVersion);
            return _ventaDAL.ActualizarEnvio2(
                request.OrderID,
                request.ShipName,
                request.ShipAddress,
                request.ShipCity,
                request.ShipRegion,
                request.ShipPostalCode,
                request.ShipCountry,
                request.ShipVia,
                request.Freight,
                rowVersion);
        }
        public List<VentaRptDto> ObtenerVentasRpt(bool selectorRealizaBusqueda, VentasBuscarDto criterios)
        {
            List<Venta> ventas = _ventaDAL.ObtenerVentas(selectorRealizaBusqueda, criterios);
            var ventasRpt = ventas.Select(v => new VentaRptDto
            {
                OrderID = v.OrderID,
                Cliente = v.Cliente.CompanyName,
                Vendedor = v.Empleado.NameByLastName,
                FechaDePedido = v.OrderDate,
                FechaRequerido = v.RequiredDate,
                FechaDeEnvio = v.ShippedDate,
                CompaniaTransportista = v.Transportista.CompanyName,
                DirigidoA = v.ShipName,
                Domicilio = v.ShipAddress,
                Ciudad = v.ShipCity,
                Region = v.ShipRegion,
                CodigoPostal = v.ShipPostalCode,
                Pais = v.ShipCountry,
                Flete = v.Freight ?? 0m
            }).ToList();
            return ventasRpt;
        }
    }
}
