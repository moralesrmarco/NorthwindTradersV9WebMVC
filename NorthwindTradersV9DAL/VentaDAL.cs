using Microsoft.Data.SqlClient;
using NorthwindTradersV9DAL.Infrastructure;
using NorthwindTradersV9Entities;
using NorthwindTradersV9Entities.DTOs;
using System.Data;

namespace NorthwindTradersV9DAL
{
    public class VentaDAL : IVentaDAL
    {
        private readonly IDbConnectionFactory _connectionFactory;
        public VentaDAL(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public VentaPaginadoDto ObtenerVentasPaginadasConBusqueda(
            int pageIndex,
            int pageSize,
            VentasBuscarDto filtro)
        {
            VentaPaginadoDto resultado = new VentaPaginadoDto();
            try
            {
                using(SqlConnection cn = _connectionFactory.CreateConnection())
                {
                    using (SqlCommand cmd = new SqlCommand("SpVentasBuscarConPaginacion", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        // Parámetros de paginación
                        cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                        cmd.Parameters.AddWithValue("@PageSize", pageSize);

                        // Identificadores
                        cmd.Parameters.AddWithValue(
                            "@IdIni", (object?)filtro.IdIni ?? DBNull.Value);

                        cmd.Parameters.AddWithValue(
                            "@IdFin", (object?)filtro.IdFin ?? DBNull.Value);

                        // Cliente
                        cmd.Parameters.AddWithValue(
                            "@Cliente", (object?)filtro.Cliente ?? "");

                        // Fecha de venta
                        cmd.Parameters.AddWithValue("@FVenta", filtro.FVenta);

                        cmd.Parameters.AddWithValue(
                            "@FVentaIni",
                            (object?)filtro.FVentaIni ?? DBNull.Value);

                        cmd.Parameters.AddWithValue(
                            "@FVentaFin",
                            filtro.FVentaFin.HasValue
                                ? filtro.FVentaFin.Value.Date.AddDays(1)
                                : DBNull.Value);

                        cmd.Parameters.AddWithValue(
                            "@FVentaNull", filtro.FVentaNull);

                        // Fecha requerida
                        cmd.Parameters.AddWithValue(
                            "@FRequerido", filtro.FRequerido);

                        cmd.Parameters.AddWithValue(
                            "@FRequeridoIni",
                            (object?)filtro.FRequeridoIni ?? DBNull.Value);

                        cmd.Parameters.AddWithValue(
                            "@FRequeridoFin",
                            filtro.FRequeridoFin.HasValue
                                ? filtro.FRequeridoFin.Value.Date.AddDays(1)
                                : DBNull.Value);

                        cmd.Parameters.AddWithValue(
                            "@FRequeridoNull", filtro.FRequeridoNull);

                        // Fecha de envío
                        cmd.Parameters.AddWithValue(
                            "@FEnvio", filtro.FEnvio);

                        cmd.Parameters.AddWithValue(
                            "@FEnvioIni",
                            (object?)filtro.FEnvioIni ?? DBNull.Value);

                        cmd.Parameters.AddWithValue(
                            "@FEnvioFin",
                            filtro.FEnvioFin.HasValue
                                ? filtro.FEnvioFin.Value.Date.AddDays(1)
                                : DBNull.Value);

                        cmd.Parameters.AddWithValue(
                            "@FEnvioNull", filtro.FEnvioNull);

                        // Empleado
                        cmd.Parameters.AddWithValue(
                            "@Empleado", (object?)filtro.Empleado ?? "");

                        // Compañía transportista
                        cmd.Parameters.AddWithValue(
                            "@CompañiaT", (object?)filtro.CompañiaT ?? "");

                        // Dirigido a
                        cmd.Parameters.AddWithValue(
                            "@DirigidoA", (object?)filtro.DirigidoA ?? "");
                        cn.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                resultado.TotalRegistros = dr.GetInt32(dr.GetOrdinal("TotalRegistros"));
                                resultado.PageIndex = dr.GetInt32(dr.GetOrdinal("PageIndex"));
                            }
                            if (dr.NextResult())
                            {
                                while (dr.Read())
                                {
                                    VentaDto venta = new VentaDto
                                    {
                                        OrderID = dr.GetInt32(
                                            dr.GetOrdinal("OrderID")),
                                        CustomerCompanyName = dr.IsDBNull(
                                            dr.GetOrdinal("CustomerCompanyName"))
                                                ? null
                                                : dr.GetString(
                                                    dr.GetOrdinal("CustomerCompanyName")),
                                        CustomerContactName = dr.IsDBNull(
                                            dr.GetOrdinal("ContactName"))
                                                ? null
                                                : dr.GetString(
                                                    dr.GetOrdinal("ContactName")),
                                        OrderDate = dr.IsDBNull(
                                            dr.GetOrdinal("OrderDate"))
                                                ? null
                                                : dr.GetDateTime(
                                                    dr.GetOrdinal("OrderDate")),
                                        RequiredDate = dr.IsDBNull(
                                            dr.GetOrdinal("RequiredDate"))
                                                ? null
                                                : dr.GetDateTime(
                                                    dr.GetOrdinal("RequiredDate")),
                                        ShippedDate = dr.IsDBNull(
                                            dr.GetOrdinal("ShippedDate"))
                                                ? null
                                                : dr.GetDateTime(
                                                    dr.GetOrdinal("ShippedDate")),
                                        RowVersion = dr.IsDBNull(
                                            dr.GetOrdinal("RowVersion"))
                                                ? null
                                                : (byte[])dr["RowVersion"]
                                    };
                                    resultado.Ventas.Add(venta);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                "Error al obtener las ventas paginadas " + ex.Message);
            }
            return resultado;
        }
        public VentaDto? ObtenerVentaPorId(int id)
        {
            VentaDto? venta = null;
            try
            {
                using (var con = _connectionFactory.CreateConnection())
                using (var cmd = new SqlCommand("SpVentaObtenerPorId", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@OrderID", id);
                    con.Open();
                    using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (rdr.Read())
                        {
                            venta = new VentaDto()
                            {
                                OrderID = rdr.GetInt32(rdr.GetOrdinal("OrderID")),
                                CustomerCompanyName = rdr.IsDBNull(rdr.GetOrdinal("CustomerCompanyName")) ? null : rdr.GetString(rdr.GetOrdinal("CustomerCompanyName")),
                                OrderDate = rdr.IsDBNull(rdr.GetOrdinal("OrderDate")) ? (DateTime?)null : rdr.GetDateTime(rdr.GetOrdinal("OrderDate")),
                                RequiredDate = rdr.IsDBNull(rdr.GetOrdinal("RequiredDate")) ? (DateTime?)null : rdr.GetDateTime(rdr.GetOrdinal("RequiredDate")),
                                ShippedDate = rdr.IsDBNull(rdr.GetOrdinal("ShippedDate")) ? (DateTime?)null : rdr.GetDateTime(rdr.GetOrdinal("ShippedDate")),
                                EmployeeName =
                                    (rdr.IsDBNull(rdr.GetOrdinal("LastName")) ? "" : rdr.GetString(rdr.GetOrdinal("LastName")))
                                    + ", " +
                                    (rdr.IsDBNull(rdr.GetOrdinal("FirstName")) ? "" : rdr.GetString(rdr.GetOrdinal("FirstName"))),
                                ShipperCompanyName = rdr["ShipperCompanyName"] == DBNull.Value
                                    ? string.Empty
                                    : rdr["ShipperCompanyName"].ToString(),
                                ShipName = rdr.IsDBNull(rdr.GetOrdinal("ShipName")) ? null : rdr.GetString(rdr.GetOrdinal("ShipName")),
                                ShipAddress = rdr.IsDBNull(rdr.GetOrdinal("ShipAddress")) ? null : rdr.GetString(rdr.GetOrdinal("ShipAddress")),
                                ShipCity = rdr.IsDBNull(rdr.GetOrdinal("ShipCity")) ? null : rdr.GetString(rdr.GetOrdinal("ShipCity")),
                                ShipRegion = rdr.IsDBNull(rdr.GetOrdinal("ShipRegion")) ? null : rdr.GetString(rdr.GetOrdinal("ShipRegion")),
                                ShipPostalCode = rdr.IsDBNull(rdr.GetOrdinal("ShipPostalCode")) ? null : rdr.GetString(rdr.GetOrdinal("ShipPostalCode")),
                                ShipCountry = rdr.IsDBNull(rdr.GetOrdinal("ShipCountry")) ? null : rdr.GetString(rdr.GetOrdinal("ShipCountry")),
                                Freight = rdr.IsDBNull(rdr.GetOrdinal("Freight")) ? (decimal?)null : rdr.GetDecimal(rdr.GetOrdinal("Freight")),
                                RowVersion = rdr.IsDBNull(rdr.GetOrdinal("RowVersion"))
                                    ? null
                                    : (byte[])rdr["RowVersion"]
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la venta por ID: " + ex.Message);
            }
            return venta;
        }
        public Venta ObtenerVentaPorIdRpt(int orderId)
        {
            Venta venta = null;
            try
            {
                using (var con = _connectionFactory.CreateConnection())
                using (var cmd = new SqlCommand("SpVentaObtenerPorId", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@OrderID", orderId);
                    con.Open();
                    using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (!rdr.HasRows)
                            throw new Exception($"SpVentaObtenerPorId no encontró la venta {orderId}");
                        if (rdr.Read())
                        {
                            venta = new Venta()
                            {
                                OrderID = rdr.GetInt32(rdr.GetOrdinal("OrderID")),
                                Cliente = new Cliente
                                {
                                    CustomerId = rdr.IsDBNull(rdr.GetOrdinal("CustomerID")) ? null : rdr.GetString(rdr.GetOrdinal("CustomerID")),
                                    CompanyName = rdr.IsDBNull(rdr.GetOrdinal("CustomerCompanyName")) ? null : rdr.GetString(rdr.GetOrdinal("CustomerCompanyName"))
                                },
                                Empleado = new Empleado
                                {
                                    EmployeeID = rdr.GetInt32(rdr.GetOrdinal("EmployeeID")),
                                    LastName = rdr["LastName"].ToString(),
                                    FirstName = rdr["FirstName"].ToString()
                                },
                                OrderDate = rdr.IsDBNull(rdr.GetOrdinal("OrderDate")) ? (DateTime?)null : rdr.GetDateTime(rdr.GetOrdinal("OrderDate")),
                                RequiredDate = rdr.IsDBNull(rdr.GetOrdinal("RequiredDate")) ? (DateTime?)null : rdr.GetDateTime(rdr.GetOrdinal("RequiredDate")),
                                ShippedDate = rdr.IsDBNull(rdr.GetOrdinal("ShippedDate")) ? (DateTime?)null : rdr.GetDateTime(rdr.GetOrdinal("ShippedDate")),
                                Transportista = new Transportista
                                {
                                    ShipperID = rdr.GetInt32(rdr.GetOrdinal("ShipVia")),
                                    CompanyName = rdr["ShipperCompanyName"] == DBNull.Value
                                      ? string.Empty
                                      : rdr["ShipperCompanyName"].ToString()
                                },
                                Freight = rdr.IsDBNull(rdr.GetOrdinal("Freight")) ? (decimal?)null : rdr.GetDecimal(rdr.GetOrdinal("Freight")),
                                ShipName = rdr.IsDBNull(rdr.GetOrdinal("ShipName")) ? null : rdr.GetString(rdr.GetOrdinal("ShipName")),
                                ShipAddress = rdr.IsDBNull(rdr.GetOrdinal("ShipAddress")) ? null : rdr.GetString(rdr.GetOrdinal("ShipAddress")),
                                ShipCity = rdr.IsDBNull(rdr.GetOrdinal("ShipCity")) ? null : rdr.GetString(rdr.GetOrdinal("ShipCity")),
                                ShipRegion = rdr.IsDBNull(rdr.GetOrdinal("ShipRegion")) ? null : rdr.GetString(rdr.GetOrdinal("ShipRegion")),
                                ShipPostalCode = rdr.IsDBNull(rdr.GetOrdinal("ShipPostalCode")) ? null : rdr.GetString(rdr.GetOrdinal("ShipPostalCode")),
                                ShipCountry = rdr.IsDBNull(rdr.GetOrdinal("ShipCountry")) ? null : rdr.GetString(rdr.GetOrdinal("ShipCountry")),
                                RowVersion = rdr.IsDBNull(rdr.GetOrdinal("RowVersion")) ? null : (byte[])rdr["RowVersion"]
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la venta por ID: " + ex.Message);
            }
            return venta;
        }
    }
}
